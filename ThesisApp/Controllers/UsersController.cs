using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Municipality.Data;
using ThesisApp.Entities;
using ThesisApp.Enums;
using ThesisApp.Helpers;
using ThesisApp.Models;
using ThesisApp.Services;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : BaseController
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public readonly IConfiguration _config;
    private readonly UsersService _usersService;
    public UsersController(AppDbContext context, IMapper mapper, UsersService usersService, IConfiguration config)
    {
        _db = context;
        _config = config;
        _mapper = mapper;
        _usersService = usersService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllStudents()
    {
        var user = Account;

        var students = await _db.Users
            .Where(s => s.DepartmentId == user.DepartmentId && s.Role == UserType.Student)
            .Select(s => new
            {
                s.Id,
                s.Email,
                s.Firstname,
                s.Lastname,
                s.Patronomyc,
                s.CuratorId
            })
            .ToListAsync();

        return Ok(students);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyStudents()
    {
        var user = Account;
        if (user.Role != UserType.Teacher)
        {
            throw new AppException("Permission denied!");
        }
        
        var students = await _db.Users.Where(s => s.CuratorId == user.Id)
            .Select(s => new
            {
                s.Id,
                s.Email,
                s.Firstname,
                s.Lastname,
                s.Patronomyc,
                s.CuratorId
            })
            .ToListAsync();

        return Ok(students);
    }
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        if(request == null)
        {
            return BadRequest();
        }

        if (_db.Users.Any(x => x.Email == request.Email))
        {
            return BadRequest("This email is busy!");
        }

        var user = _mapper.Map<User>(request);

        if (char.IsDigit(request.Email[0]))
        {
            user.Role = Enums.UserType.Student;
        }
        else
        {
            user.Role = Enums.UserType.Teacher;
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _db.AddAsync(user);
        try
        {
            await _db.SaveChangesAsync();
            await _usersService.GenerateAndSendConfirmCode(user);
        }
        catch (Exception ex)
        {
            return Ok(ex.InnerException.Message);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<bool> Confirmation(ConfirmRequest req)
    {
        return await _usersService.Confirm(req);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyTeachers()
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Account.Id);

        var teachers = await _db.Users
            .Where(u => u.Role == UserType.Teacher && u.DepartmentId == user.DepartmentId)
            .Select(u => new {u.Id, u.Firstname, u.Lastname, u.Email})
            .ToListAsync();
        
        return Ok(teachers);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDepartmentTeachers(int departmentId)
    {
        var teachers = await _db.Users
            .Where(u => u.Role == UserType.Teacher && u.DepartmentId == departmentId)
            .Select(u => new {u.Id, u.Firstname, u.Lastname, u.Email})
            .ToListAsync();
        
        return Ok(teachers);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> SetDepartmentAdminRole(long teacherId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Account.Id);
        if (user.Role != UserType.SuperAdmin)
        {
            throw new AppException("Permission denied!");
        }

        var teacher = await _db.Users.FindAsync(teacherId);
        
        if (teacher is not null)
        {
            teacher.Role = UserType.DepartmentAdmin;
            await _db.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> SetStudentsCountLimit(int limit)
    {
        var teacher = await _db.Users.FirstOrDefaultAsync(t => t.Id == Account.Id);

        teacher.StudentsCountLimit = limit;

        await _db.SaveChangesAsync();

        return Ok();
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ChooseTeacher(long teacherId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Account.Id);
        var teacher = await _db.Users.FirstOrDefaultAsync(t => t.Id == teacherId);
        var studentCounts = _db.Users.Where(s => s.CuratorId == teacherId).Count();
        if (teacher.StudentsCountLimit >= (studentCounts + 1))
        {
            user.CuratorId = teacherId;
            await _db.SaveChangesAsync();
            return Ok();
        }
        else
        {
            throw new AppException("Too many students for this teacher!");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Account.Id);
        if (user is not null)
        {
            var isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(req.OldPassword, user.Password);
            if (isOldPasswordCorrect)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new AppException("Old password is incorrect!");
            }
        }
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _usersService.LoginAsync(req);
        
        if (user is null)
        {
            throw new AppException("Login or password is incorrect");
        }
        byte[] userData = JsonSerializer.SerializeToUtf8Bytes(user);
        HttpContext.Session.Set("Account", userData);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            null,
            expires: DateTime.Now.AddHours(10),
            signingCredentials: credentials);

        var result = _mapper.Map<UserRequest>(user);
        result.Token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
        if (user.Role == UserType.Student || user.Role == UserType.Teacher)
        {
            var deadline = await _db.Deadlines.OrderByDescending(d => d.Id).FirstOrDefaultAsync(d => d.DepartmentId == user.DepartmentId);
            result.Deadline = deadline.EndDate;
        }
        return Ok(result);
    }
}