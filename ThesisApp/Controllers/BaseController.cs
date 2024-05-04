using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ThesisApp.Entities;

namespace ThesisApp.Controllers;

public class BaseController : ControllerBase
{
    public User? Account
    {
        get
        {
            byte[]? userData = HttpContext?.Session?.Get("Account");
            if (userData != null)
            {
                string jsonString = Encoding.UTF8.GetString(userData);
                return JsonSerializer.Deserialize<User>(jsonString);
            }
            return null;
        }
    }
}