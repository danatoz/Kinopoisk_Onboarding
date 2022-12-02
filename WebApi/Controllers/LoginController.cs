using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Models;
using WebApi.Authentication;

namespace WebApi.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]UserModel model)
    {
        return await Task.Run(() =>
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(model);

            if (user != null) 
                response = Ok(new { token = CreateJWT(user) });

            return response;
            
        });
    }
    private UserModel Authenticate(UserModel model)
    {
        if (model.Login == "test" && model.Password == "123")
            return new UserModel { Login = model.Login, Email = "test@gmail.com" };

        return null;
    }

    private string CreateJWT(UserModel model)
    {
        var secretKey = AuthOptions.GetSymmetricSecurityKey();
        var credential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, model.Login),
            new Claim(JwtRegisteredClaimNames.Sub, model.Login),
            new Claim(JwtRegisteredClaimNames.Email, model.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        var token = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER, 
            audience: AuthOptions.AUDIENCE, 
            claims: claims, 
            expires: DateTime.Now.AddDays(1), 
            signingCredentials: credential);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}