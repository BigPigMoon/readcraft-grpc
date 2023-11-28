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

        public async Task<TokensResponse> Login(string email, string password)
        {

            Context ctx = Context.GetInstance();

            var user = await ctx.db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            if (!VerifyString(password, user.PasswordHash))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid login or password"));

            string accessToken = GetToken(user, AuthOptions.accessLifetime, "access");
            string refreshToken = GetToken(user, AuthOptions.refreshLifetime, "refresh");

            user.TokenHash = HashString(refreshToken);
            await ctx.db.SaveChangesAsync();

            return new TokensResponse
            {
                Access = accessToken,
                Refresh = refreshToken,
            };
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

            User newUser = new() { Email = email, Username = username, PasswordHash = HashString(password) };

            await ctx.db.AddAsync(newUser);
            await ctx.db.SaveChangesAsync();

            string accessToken = GetToken(newUser, AuthOptions.accessLifetime, "access");
            string refreshToken = GetToken(newUser, AuthOptions.refreshLifetime, "refresh");

            newUser.TokenHash = HashString(refreshToken);
            await ctx.db.SaveChangesAsync();

            return new TokensResponse
            {
                Access = accessToken,
                Refresh = refreshToken,
            };
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

        private string GetToken(User user, TimeSpan lifetime, string scope)
        {
            var now = DateTime.Now;

            ClaimsIdentity identity = GetIdentity(user, scope);

            JwtSecurityToken token = new(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(lifetime),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashString(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        private bool VerifyString(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
