using Microsoft.AspNetCore.Mvc;
using BL.Abstract;
using Core.Entities.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IAuthService _authService;

    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public ActionResult Login(UserModel userModel)
    {
        var result = _authService.Login(userModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}