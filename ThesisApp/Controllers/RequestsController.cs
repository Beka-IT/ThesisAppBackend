using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municipality.Data;
using ThesisApp.Entities;
using ThesisApp.Enums;
using ThesisApp.Helpers;
using ThesisApp.Models;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class RequestsController : BaseController
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public RequestsController(AppDbContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get(long id)
    {
        var user = Account;
        var request = await _db.RequestToTheses.FindAsync(id);
        if (request.CuratorId != user.Id && request.StudentId != user.Id)
        {
            throw new AppException("Permission denied!");
        }
        var response = _mapper.Map<GetRequestResponse>(request);
        var curator = await _db.Users.FindAsync(request.CuratorId);
        var student = await _db.Users.FindAsync(request.StudentId);
        response.CuratorFirstname = curator.Firstname;
        response.CuratorLastname = curator.Lastname;
        response.CuratorPatronomyc = curator.Patronomyc;
        response.StudentFirstname = student.Firstname;
        response.StudentLastname = student.Lastname;
        response.StudentPatronomyc = student.Patronomyc;

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = Account;
        List<GetAllRequestResponseItem> requests = new();
        if (user.Role == UserType.Student)
        {
            requests = await _db.RequestToTheses.Where(r => r.StudentId == user.Id)
                .Select(r => new GetAllRequestResponseItem
                {
                    Id = r.Id,
                    TitleKg = r.TitleKg,
                    TitleTr = r.TitleTr,
                    CreatedAt = r.CreatedAt,
                    StudentFirstname = user.Firstname,
                    StudentLastname = user.Lastname,
                    StudentPatronomyc = user.Patronomyc
                })
                .ToListAsync();
        }
        else if (user.Role == UserType.Teacher)
        {
            requests = await _db.RequestToTheses.Where(r => r.CuratorId == user.Id)
                .Select(r => new GetAllRequestResponseItem
                {
                    Id = r.Id,
                    TitleKg = r.TitleKg,
                    TitleTr = r.TitleTr,
                    CreatedAt = r.CreatedAt,
                    StudentFirstname = _db.Users.FirstOrDefault(s => s.Id == r.StudentId).Firstname,
                    StudentLastname = _db.Users.FirstOrDefault(s => s.Id == r.StudentId).Lastname,
                    StudentPatronomyc = _db.Users.FirstOrDefault(s => s.Id == r.StudentId).Patronomyc
                })
                .ToListAsync();
        }

        return Ok(requests);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRequestToThesisRequest req)
    {
        var user = Account;

        if (user.Role != UserType.Student)
        {
            throw new AppException("Permission denied!");
        }

        var newRequestToThesis = _mapper.Map<RequestToThesis>(req);
        newRequestToThesis.StudentId = user.Id;
        newRequestToThesis.CreatedAt = DateTime.Now;

        await _db.AddAsync(newRequestToThesis);
        var notification = new Notification()
        {
            CreatedAt = DateTime.Now,
            IsRead = false,
            TitleTr = $"{user.Lastname} {user.Lastname} size bir tez konusu teklif etti!",
            TitleKg = $"{user.Lastname} {user.Lastname} сизге дипломдук-иш боюнча тема сунуштады!",
            SentToId = newRequestToThesis.CuratorId
        };
        await _db.AddAsync(notification);
        await _db.SaveChangesAsync();

        return Ok(newRequestToThesis);
    }

    [HttpPut]
    public async Task<IActionResult> Apply(long id)
    {
        var user = Account;
        var request = await _db.RequestToTheses.FindAsync(id);
        if (request.CuratorId != user.Id)
        {
            throw new AppException("Permission denied!");
        }

        var newThesis = new Thesis
        {
            TitleKg = request.TitleKg,
            TitleTr = request.TitleTr,
            DepartmentId = user.DepartmentId,
            DescriptionKg = request.DescriptionKg,
            DescriptionTr = request.DescriptionTr,
            CuratorId = user.Id,
            IsChosen = false,
            CreatedAt = DateTime.Now
        };
        await _db.AddAsync(newThesis);
        _db.Remove(request);
        var notification = new Notification()
        {
            CreatedAt = DateTime.Now,
            IsRead = false,
            TitleTr = $"{user.Lastname} {user.Lastname} siz teklif ettiğiniz tez konusunu kabul etti!",
            TitleKg = $"{user.Lastname} {user.Lastname} сиз сунуштаган дипломдук-иш боюнча теманы кабылдады!",
            SentToId = request.StudentId
        };
        await _db.AddAsync(notification);
        await _db.SaveChangesAsync();
        if (request.IsMyTheme)
        {
            var student = await _db.Users.FindAsync(request.StudentId);
            student.ChosenThesisId = newThesis.Id;
            await _db.SaveChangesAsync();
        }
        
        return Ok(newThesis);
    }
    [HttpPut]
    public async Task<IActionResult> Decline(long id)
    {
        var user = Account;
        var request = await _db.RequestToTheses.FindAsync(id);
        if (request.CuratorId != user.Id)
        {
            throw new AppException("Permission denied!");
        }

        _db.Remove(request);
        var notification = new Notification()
        {
            CreatedAt = DateTime.Now,
            IsRead = false,
            TitleTr = $"{user.Lastname} {user.Lastname} siz teklif ettiğiniz tez konusunu kabul etmedi!",
            TitleKg = $"{user.Lastname} {user.Lastname} сиз сунуштаган дипломдук-иш боюнча теманы кабыл алган жок!",
            SentToId = request.StudentId
        };
        await _db.AddAsync(notification);
        await _db.SaveChangesAsync();

        return Ok(request);
    }
    [HttpPut]
    public async Task<IActionResult> Update(UpdateRequestToThesisRequest req)
    {
        var user = Account;

        var request = await _db.RequestToTheses.FindAsync(req.Id);
        if (request is null)
        {
            throw new AppException("Request not found!");
        }

        if (request.StudentId != user.Id)
        {
            throw new AppException("Permission denied!");
        }

        request.CuratorId = req.CuratorId;
        request.TitleTr = req.TitleTr;
        request.TitleKg = req.TitleKg;
        request.DescriptionKg = req.DescriptionKg;
        request.DescriptionTr = req.DescriptionTr;
        request.IsMyTheme = req.IsMyTheme;
        request.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return Ok(request);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(long id)
    {
        var user = Account;
        if (user.Role != UserType.Student && user.Role != UserType.Teacher)
        {
            throw new AppException("Permission denied!");
        }

        var request = await _db.RequestToTheses.FindAsync(id);
        
        if (request.CuratorId != user.Id && request.StudentId != user.Id)
        {
            throw new AppException("The request is not created by you!");
        }

        _db.Remove(request);
        await _db.SaveChangesAsync();
        
        return Ok(request);
    }
}