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
        return Ok(result);
    }
}