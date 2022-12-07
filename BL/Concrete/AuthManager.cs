using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BL.Abstract;
using BL.Constants;
using Core.Entities.Models;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;

namespace BL.Concrete;

public class AuthManager : IAuthService
{
    private IConfiguration _configuration;

    private readonly TokenOptions _tokenOptions;

    public AuthManager(IConfiguration configuration)
    {
        _configuration = configuration;
        var tokenOptionsSection = configuration.GetSection("TokenOptions");
        _tokenOptions = new TokenOptions
        {
            Issuer = tokenOptionsSection["Issuer"],
            Audience = tokenOptionsSection["Audience"],
            AccessTokenExpiration = int.Parse(tokenOptionsSection["AccessTokenExpiration"]),
            SecurityKey = tokenOptionsSection["SecurityKey"]
        };
    }

    public IDataResult<AccessToken> Login(UserModel userModel)
    {
        //Check user in db

        //Validate user password
        var tokenModel = CreateAccessToken(userModel);

        return tokenModel;
    }

    public IDataResult<AccessToken> CreateAccessToken(UserModel model)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey));
        var credential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, model.Login),
            new Claim(JwtRegisteredClaimNames.Sub, model.Login),
            new Claim(JwtRegisteredClaimNames.Email, model.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        var token = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration),
            signingCredentials: credential);

        return new SuccessDataResult<AccessToken>(new AccessToken{ Token = new JwtSecurityTokenHandler().WriteToken(token) }, Message.AccessTokenCreated);
    }
}