using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    internal class HashUtil
    {
        public static string HashString(string str)
        {
            return BCrypt.Net.BCrypt.HashPassword(str, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public static bool VerifyString(string str, string hashedStr)
        {
            return BCrypt.Net.BCrypt.Verify(str, hashedStr);
        }
    }
}
