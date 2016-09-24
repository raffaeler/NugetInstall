using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetInstall
{
    internal class CommandLine
    {
        private Configuration _configuration;

        public CommandLine(Configuration configuration)
        {
            _configuration = configuration;
        }

        public string PackageFolder { get; set; }
        public string NugetFolder { get; set; }
        public string PackageToSearch { get; set; }
        public bool AreSymbolsExcluded { get; set; }
        public bool IsHelpRequested { get; set; }
        public bool IsDeleteRequested { get; set; }
        public bool IsLogRequested { get; set; }
        public bool IsWaitRequested { get; set; }
        public bool UseExePath { get; set; }

        public void ReadCommandLine(string[] args)
        {
            if (args.Match("-h"))
            {
                IsHelpRequested = true;
            }

            if (args.Match("-l"))
            {
                IsLogRequested = true;
            }

            if (args.Match("-x"))
            {
                UseExePath = true;
            }

            if (args.Match("-ns"))
            {
                AreSymbolsExcluded = true;
            }

            if (args.Match("-d"))
            {
                IsDeleteRequested = true;
            }

            if (args.Match("-w"))
            {
                IsWaitRequested = true;
            }

            PackageFolder = args.GetParam("-s:");
            if (PackageFolder == null)
            {
                if (UseExePath)
                {
                    PackageFolder = GetExePath();
                }
                else
                {
                    PackageFolder = Directory.GetCurrentDirectory();
                }
            }

            NugetFolder = args.GetParam("-t:");
            if (NugetFolder == null)
            {
                if (UseExePath)
                {
                    NugetFolder = GetExePath();
                }
                else
                {
                    NugetFolder = Directory.GetCurrentDirectory();
                }
            }

            PackageToSearch = args.GetParam("-p:");
            if (PackageToSearch == null)
            {
                PackageToSearch = "*." + _configuration.NugetExt;
            }
        }

        public void SetCurrentDirectory()
        {
            if (UseExePath)
            {
                Directory.SetCurrentDirectory(GetExePath());
            }
            else
            {
                Directory.SetCurrentDirectory(NugetFolder);
            }
        }

        private string GetExePath()
        {
            var filename = System.Reflection.Assembly.GetEntryAssembly().Location;
            var path = filename.Substring(0, filename.LastIndexOf('\\'));
            return path;
        }
    }
}
