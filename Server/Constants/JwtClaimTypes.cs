using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static public class JwtClaimTypes
    {
        public static string Email { get; } = "email";
        public static string UserId { get; } = "uid";
        public static string Scope { get; } = "scope";
        public static string Username { get; } = "username";
    }
}
