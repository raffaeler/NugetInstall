using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetInstall
{
    public static class LinqExtensions
    {
        public static string GetParam(this IEnumerable<string> container, string par)
        {
            return container
                .Where(s => s.StartsWith(par))
                .Select(s => s.Substring(par.Length))
                .FirstOrDefault();
        }

        public static bool Match(this IEnumerable<string> container, string par)
        {
            return container
                .Any(s => s.StartsWith(par));
        }
    }
}
