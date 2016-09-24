using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetInstall
{
    internal class Configuration
    {
        public Configuration()
        {
            NugetExe = Defaults.NugetExe;
            NugetExt = Defaults.NugetExt;
            SymbolsMarker = Defaults.SymbolsMarker;
            NugetParams = Defaults.NugetParams;
        }

        public string NugetExe { get; private set; }
        public string NugetExt { get; private set; }
        public string SymbolsMarker { get; private set; }
        public string NugetParams { get; private set; }

        public void ReadConfiguration()
        {
            string temp;
            temp = ConfigurationManager.AppSettings["NugetExt"];
            if (!string.IsNullOrEmpty(temp))
                NugetExt = temp;

            temp = ConfigurationManager.AppSettings["SymbolsMarker"];
            if (!string.IsNullOrEmpty(temp))
                SymbolsMarker = temp.ToLower();

            temp = ConfigurationManager.AppSettings["NugetParams"];
            if (!string.IsNullOrEmpty(temp))
                NugetParams = temp;
        }
    }
}
