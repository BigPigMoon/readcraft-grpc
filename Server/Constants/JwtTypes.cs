using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static public class JwtTypes
    {
        public static string Access { get; } = "access";
        public static string Refresh { get; } = "refresh";
    }
}
