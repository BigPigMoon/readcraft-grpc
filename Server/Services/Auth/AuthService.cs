using Storage;
using Storage.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Server.Services.Auth
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
            Context ctx = Context.GetInstance();

            var user = await ctx.db.Users.Where(u => u.Email == email && u.Username == username).FirstOrDefaultAsync();

            if (user != null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists"));
            }

            User newUser = new() { Email = email, Username = username, PasswordHash = password };

            await ctx.db.AddAsync(newUser);
            await ctx.db.SaveChangesAsync();

            var now = DateTime.Now;

            ClaimsIdentity identity = GetIdentity(newUser, "access");

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(AuthOptions.accessLifetime),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new TokensResponse() { Access = encodedJwt, Refresh = "" };
        }

        private ClaimsIdentity GetIdentity(User user, string scope)
        {
            var claims = new List<Claim>
            {
                new("email", user.Email),
                new("uid", user.Id.ToString()),
                new("scope", scope),
                new("username", user.Username),
            };

            ClaimsIdentity claimsIdentity = new(claims, "Token");

            return claimsIdentity;
        }
    }
}
