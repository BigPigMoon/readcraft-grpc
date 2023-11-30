using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services.Auth
{
    internal interface IAuthService
    {
        Task<TokensResponse> Login(string email, string password);
        Task<TokensResponse> Register(string email, string username, string password);
        Task<TokensResponse> Refresh(string refreshToken);
        Task Logout(int userId);
    }
}
