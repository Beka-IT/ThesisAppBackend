using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Municipality.Data;
using ThesisApp.Entities;
using ThesisApp.Helpers;
using ThesisApp.Models;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public UsersController(AppDbContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        if(request == null)
        {
            return BadRequest("Баардык талааларды толтурунуз!");
        }

        if (_db.Users.Any(x => x.Email == request.Email))
        {
            return BadRequest("Бул email бош эмес!");
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