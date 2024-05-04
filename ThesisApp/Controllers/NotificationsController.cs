using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municipality.Data;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class NotificationsController : BaseController
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public NotificationsController(AppDbContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetALl()
    {
        var user = Account;
        var notifications = await _db.Notifications.Where(n => n.SentToId == user.Id)
            .ToListAsync();
        return Ok(notifications);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(long id)
    {
        var user = Account;
        var notification = await _db.Notifications.FindAsync(id);
        notification.IsRead = true;
        await _db.SaveChangesAsync();
        return Ok(notification);
    }
}