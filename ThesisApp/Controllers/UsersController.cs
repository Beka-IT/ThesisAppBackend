using Microsoft.AspNetCore.Mvc;
using ThesisApp.Helpers;

namespace ThesisApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
    public UsersController() {}

    [HttpGet]
    public string GetText()
    {
        return "I am the own this app";
    }
}