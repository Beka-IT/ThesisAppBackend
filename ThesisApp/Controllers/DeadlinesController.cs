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
public class DeadlinesController : BaseController
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public DeadlinesController(AppDbContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var user = Account;
        var deadline = await _db.Deadlines.OrderByDescending(d => d.Id).FirstOrDefaultAsync(d => d.DepartmentId == user.DepartmentId);
        return Ok(deadline);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateDeadlineRequest req)
    {
        var user = Account;
        if (user.Role != UserType.DepartmentAdmin)
        {
            throw new AppException("Permission denied!");
        }
        var newDeadline = _mapper.Map<Deadline>(req);
        newDeadline.DepartmentId = user.DepartmentId;
        await _db.AddAsync(newDeadline);
        await _db.SaveChangesAsync();
        return Ok(newDeadline);
    }
}