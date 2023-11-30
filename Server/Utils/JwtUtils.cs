using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using Server.Services.Auth;
using Storage.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    internal class JwtUtils
    {
        public static JwtSecurityToken DecodeJwt(string token)
        {
            JwtSecurityTokenHandler jwtHandler = new();

            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token.Split(' ')[1]);

            if (jwtToken == null)
                throw new RpcException(new Status(StatusCode.Internal, "Can not read token"));

            return jwtToken;
        }

        public static int GetUid(JwtSecurityToken jwt)
        {
            var userID = jwt.Claims.FirstOrDefault(claims => claims.Type == JwtClaimTypes.UserId)?.Value;

            if (userID == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Uid not found"));

            return int.Parse(userID);
        }


        private static ClaimsIdentity GetIdentity(User user, string scope)
        {
            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Email, user.Email),
                new(JwtClaimTypes.UserId, user.Id.ToString()),
                new(JwtClaimTypes.Scope, scope),
                new(JwtClaimTypes.Username, user.Username),
            };

            ClaimsIdentity claimsIdentity = new(claims, "Token");

            return claimsIdentity;
        }

        public static string GetToken(User user, TimeSpan lifetime, string scope)
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
    }
}
