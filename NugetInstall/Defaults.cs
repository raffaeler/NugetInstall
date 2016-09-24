using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetInstall
{
    internal static class Defaults
    {
        public static readonly string NugetExe = "nuget.exe";
        public static readonly string NugetExt = "nupkg";
        public static readonly string SymbolsMarker = ".symbols.";
        public static readonly string NugetParams = "add {0} -Source {1}";
        public static readonly string LogFile = "NugetInstall.log";
    }
}
