using Storage;
using Storage.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Server.Utils;

namespace Server.Services.Auth
{
    internal class AuthService : IAuthService
    {
        public async Task<TokensResponse> Login(string email, string password)
        {

            Context ctx = Context.GetInstance();

            var user = await ctx.db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            if (!HashUtil.VerifyPassword(password, user.PasswordHash))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid login or password"));

            string accessToken = JwtUtils.GetToken(user, AuthOptions.accessLifetime, JwtTypes.Access);
            string refreshToken = JwtUtils.GetToken(user, AuthOptions.refreshLifetime, JwtTypes.Refresh);

            user.TokenHash = HashUtil.HashToken(refreshToken);
            await ctx.db.SaveChangesAsync();

            return new TokensResponse
            {
                Access = accessToken,
                Refresh = refreshToken,
            };
        }

        public async Task Logout(long userId)
        {
            var ctx = Context.GetInstance();

            var user = await ctx.db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            user.TokenHash = null;

            await ctx.db.SaveChangesAsync();
        }

        public async Task<TokensResponse> Refresh(string refreshToken)
        {
            JwtSecurityTokenHandler jwtHandler = new();

            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(refreshToken);

            if (jwtToken == null)
                throw new RpcException(new Status(StatusCode.Internal, "Can not read token"));

            var scope = jwtToken.Claims.FirstOrDefault(claims => claims.Type == JwtClaimTypes.Scope)?.Value;

            if (scope == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Scope not found"));

            if (scope != JwtTypes.Refresh)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Use the refresh token"));

            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.UserId)?.Value;

            if (userId == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Uid not found"));

            var ctx = Context.GetInstance();

            var user = await ctx.db.Users.Where(u => u.Id == int.Parse(userId)).FirstOrDefaultAsync();

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            if (user.TokenHash == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User does not authorize"));

            if (!HashUtil.VerifyToken(refreshToken, user.TokenHash))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Token is not valid"));

            string newAccessToken = JwtUtils.GetToken(user, AuthOptions.accessLifetime, JwtTypes.Access);
            string newRefreshToken = JwtUtils.GetToken(user, AuthOptions.refreshLifetime, JwtTypes.Refresh);

            user.TokenHash = HashUtil.HashToken(newRefreshToken);
            await ctx.db.SaveChangesAsync();

            return new TokensResponse
            {
                Access = newAccessToken,
                Refresh = newRefreshToken,
            };
        }

        public async Task<TokensResponse> Register(string email, string username, string password)
        {
            Context ctx = Context.GetInstance();

            var user = await ctx.db.Users.Where(u => u.Email == email && u.Username == username).FirstOrDefaultAsync();

            if (user != null)
                throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists"));

            User newUser = new() { Email = email, Username = username, PasswordHash = HashUtil.HashPassowd(password) };

            await ctx.db.AddAsync(newUser);
            await ctx.db.SaveChangesAsync();

            string accessToken = JwtUtils.GetToken(newUser, AuthOptions.accessLifetime, JwtTypes.Access);
            string refreshToken = JwtUtils.GetToken(newUser, AuthOptions.refreshLifetime, JwtTypes.Refresh);

            newUser.TokenHash = HashUtil.HashToken(refreshToken);
            await ctx.db.SaveChangesAsync();

            return new TokensResponse
            {
                Access = accessToken,
                Refresh = refreshToken,
            };
        }

    }
}
