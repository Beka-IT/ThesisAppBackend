using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using Municipality.Data;
using ThesisApp.Entities;
using ThesisApp.Models;

namespace ThesisApp.Services;

public class UsersService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;

    public UsersService(IMapper mapper, AppDbContext db)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task<User> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        
        if (user is not null)
        {
            user = BCrypt.Net.BCrypt.Verify(req.Password, user.Password) ? user : null;
        }
        return user;
    }
    
    public async Task<int> GenerateAndSendConfirmCode(User user)
    {
        var random = new Random();
        var code = random.Next(100000, 999999);
        var userId = _db.Users.FirstOrDefault(u => u.Email == user.Email).Id;
        
        var confirmationCode = new ConfirmationCode()
        {
            UserId = userId,
            Code = code
        };
        await _db.AddAsync(confirmationCode);
        await _db.SaveChangesAsync();
        SendConfirmCodeToEmail(code, user.Email);
        return code;
    }
    public async Task<bool> Confirm(ConfirmRequest req)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == req.Email);
        var isVerified =  await _db.ConfirmationCodes
            .AnyAsync(c => c.UserId == user.Id && c.Code == req.Code);
        
        if (isVerified)
        {
            user.IsVerified = true;
            await _db.SaveChangesAsync();
        }
        return isVerified;
    }
    
    private void SendConfirmCodeToEmail(int code, string userEmail)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("1904.01028@manas.edu.kg"));
        email.To.Add(MailboxAddress.Parse(userEmail));
        email.Subject = "Title";
        email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>{code}</h1>" };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("1904.01028@manas.edu.kg", "Master2510!");
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}