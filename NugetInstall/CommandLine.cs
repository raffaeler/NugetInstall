using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetInstall
{
    internal class CommandLine
    {
        public string PackageFolder { get; set; }
        public string NugetFolder { get; set; }
        public string PackageToSearch { get; set; }
        public bool AreSymbolsExcluded { get; set; }
        public bool IsHelpRequested { get; set; }
        public bool IsDeleteRequested { get; set; }
        public bool IsLogRequested { get; set; }
        public bool IsWaitRequested { get; set; }
        public bool UseExePath { get; set; }
    }
}
