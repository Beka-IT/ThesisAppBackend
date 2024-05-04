using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municipality.Data;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class FacultiesController : BaseController
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public FacultiesController(AppDbContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetFaculties()
    {
        var faculties = await _db.Faculties.ToListAsync();
        return Ok(faculties);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDepartments(int facultyId)
    {
        var departments = await _db.Departments.Where(d => d.FacultyId == facultyId).ToListAsync();
        return Ok(departments);
    }
}