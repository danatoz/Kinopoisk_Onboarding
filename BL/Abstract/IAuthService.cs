using Core.Entities.Models;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;

namespace BL.Abstract;

public interface IAuthService
{
    IDataResult<AccessToken> Login(UserModel userModel);

    IDataResult<AccessToken> CreateAccessToken(UserModel userModel);
}