using Server.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class AuthService : IAuthService
    {
        public AuthService() { }

        public Task<TokensResponse> Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task Logout(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TokensResponse> Refresh(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<TokensResponse> Register(string email, string username, string password)
        {
            return new TokensResponse() { Access = "", Refresh = "" };
        }
    }
}
