using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
    public class Context
    {
        public Sqlite db { get; }
        private Context()
        {
            db = new Sqlite();
        }

        private static Context? _instance;

        public static Context GetInstance()
        {
            return _instance ??= new Context();
        }
    }
}
