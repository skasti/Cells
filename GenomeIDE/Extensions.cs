using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenomeIDE
{
    public static class Extensions
    {
        public static string Inject(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
