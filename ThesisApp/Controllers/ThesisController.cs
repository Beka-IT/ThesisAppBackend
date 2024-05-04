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
public class ThesisController : BaseController
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public ThesisController(AppDbContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = await _db.Users.FindAsync(Account.Id);

        var result = await _db.Theses.Where(t => t.DepartmentId == user.DepartmentId)
            .Select(d => new GetAllThesesResponseItem
            {
                Id = d.Id,
                TitleKg = d.TitleKg,
                TitleTr = d.TitleTr,
                CreatedAt = d.CreatedAt,
                IsChosen = d.IsChosen,
                CuratorFirstname = _db.Users.FirstOrDefault(t => t.Id == d.CuratorId).Firstname,
                CuratorLastname = _db.Users.FirstOrDefault(t => t.Id == d.CuratorId).Lastname,
                CuratorPatronomyc = _db.Users.FirstOrDefault(t => t.Id == d.CuratorId).Patronomyc
            })
            .ToListAsync();

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> Get(long id)
    {
        var user = await _db.Users.FindAsync(Account.Id);

        var result = await _db.Theses.Where(t => t.Id == id)
            .Select(d => new GetThesisResponse
            {
                Id = d.Id,
                TitleKg = d.TitleKg,
                TitleTr = d.TitleTr,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                IsChosen = d.IsChosen,
                DescriptionKg = d.DescriptionKg,
                DescriptionTr = d.DescriptionTr,
                CuratorFirstname = _db.Users.FirstOrDefault(t => t.Id == d.CuratorId).Firstname,
                CuratorLastname = _db.Users.FirstOrDefault(t => t.Id == d.CuratorId).Lastname,
                CuratorPatronomyc = _db.Users.FirstOrDefault(t => t.Id == d.CuratorId).Patronomyc
            })
            .FirstOrDefaultAsync();

        var students = await _db.Users.Where(u => u.ChosenThesisId == id)
            .Select(s => new StudentResponse
            {
                Id = s.Id,
                Firstname = s.Firstname,
                Lastname = s.Lastname,
                Patronomyc = s.Patronomyc
            }).ToListAsync();

        result.ChosenBy = students;

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateThesisRequest req)
    {
        var user = Account;
        
        if (user.Role != UserType.Teacher)
        {
            throw new AppException("Permission denied!");
        }

        var newThesis = _mapper.Map<Thesis>(req);
        newThesis.CreatedAt = DateTime.Now;
        newThesis.CuratorId = user.Id;
        newThesis.DepartmentId = user.DepartmentId;
        newThesis.IsChosen = false;

        await _db.AddAsync(newThesis);
        await _db.SaveChangesAsync();

        return Ok(newThesis);
    }
    [HttpPut]
    public async Task<IActionResult> Update(UpdateThesisRequest req)
    {
        var user = Account;
        
        if (user.Role != UserType.Teacher)
        {
            throw new AppException("Permission denied!");
        }

        var thesis = await _db.Theses.FindAsync(req.Id);
        if (thesis.CuratorId != user.Id)
        {
            throw new AppException("The thesis is not created by you!");
        }

        thesis.TitleKg = req.TitleKg;
        thesis.TitleTr = req.TitleTr;
        thesis.DescriptionKg = req.DescriptionKg;
        thesis.DescriptionTr = req.DescriptionTr;
        thesis.UpdatedAt = DateTime.Now;
        
        await _db.SaveChangesAsync();

        return Ok(thesis);
    }

    [HttpPut]
    public async Task<IActionResult> Choose(long id)
    {
        var user = await _db.Users.FindAsync(Account.Id);
        if (user.Role != UserType.Student)
        {
            throw new AppException("Permission denied!");
        }

        var thesis = await _db.Theses.FindAsync(id);

        if (thesis.IsChosen)
        {
            throw new AppException("Thesis is already chosen!");
        }

        user.ChosenThesisId = thesis.Id;
        var notification = new Notification()
        {
            CreatedAt = DateTime.Now,
            IsRead = false,
            TitleTr = $"{user.Lastname} {user.Lastname} girdiğiniz konuyu seçti!",
            TitleKg = $"{user.Lastname} {user.Lastname} сиз киргизген теманы тандады!",
            SentToId = thesis.CuratorId
        };
        await _db.AddAsync(notification);
        await _db.SaveChangesAsync();
        var result = _mapper.Map<UserRequest>(user);
        if (user.Role == UserType.Student || user.Role == UserType.Teacher)
        {
            var deadline = await _db.Deadlines.OrderByDescending(d => d.Id).FirstOrDefaultAsync(d => d.DepartmentId == user.DepartmentId);
            result.Deadline = deadline.EndDate;
        }
        return Ok(result);
    }
    
    [HttpPut]
    public async Task<IActionResult> ToggleIsChosenStatus(long id)
    {
        var user = await _db.Users.FindAsync(Account.Id);
        if (user.Role != UserType.Teacher)
        {
            throw new AppException("Permission denied!");
        }

        var thesis = await _db.Theses.FindAsync(id);
        if (thesis.CuratorId != user.Id)
        {
            throw new AppException("The thesis is not created by you!");
        }

        thesis.IsChosen = !thesis.IsChosen;
        await _db.SaveChangesAsync();
        return Ok(thesis);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(long id)
    {
        var user = Account;
        if (user.Role != UserType.Teacher)
        {
            throw new AppException("Permission denied!");
        }

        var thesis = await _db.Theses.FindAsync(id);
        
        if (thesis.CuratorId != user.Id)
        {
            throw new AppException("The thesis is not created by you!");
        }

        _db.Remove(thesis);
        await _db.SaveChangesAsync();
        
        return Ok(thesis);
    }
}