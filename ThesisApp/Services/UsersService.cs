using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Municipality.Data;
using ThesisApp.Entities;

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