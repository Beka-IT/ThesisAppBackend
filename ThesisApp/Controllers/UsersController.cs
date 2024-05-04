using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Municipality.Data;
using ThesisApp.Entities;
using ThesisApp.Helpers;
using ThesisApp.Models;
using ThesisApp.Services;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    private readonly UsersService _usersService;
    public UsersController(AppDbContext context, IMapper mapper, UsersService usersService)
    {
        _db = context;
        _mapper = mapper;
        _usersService = usersService;
    }

    [HttpPost]
    [AllowAnonymous]
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

    [HttpGet]
    public string GetText()
    {
        return "I am the owner this app";
    }
}