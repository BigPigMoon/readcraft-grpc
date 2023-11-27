using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class Jwt
    {
        public string GenerateToken(int uid, string email)
        {
            var token = JwtBuilder
                .Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(20).ToUnixTimeSeconds())
                .AddClaim("uid", uid)
                .AddClaim("email", email)
                .Encode();

            return token;
        }
    }
}
