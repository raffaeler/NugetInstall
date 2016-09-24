using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// commonly used command lines:
// h:\_nuget\NugetInstall -ns -s:h:\_nuget -t:h:\_nuget -d
// h:\_nuget\NugetInstall -ns -s:h:\_nuget -t:h:\_nuget -w
// h:\_nuget\NugetInstall -ns -s:h:\_nuget -t:h:\_nuget -h
// h:\_nuget\NugetInstall -ns -x -d

namespace NugetInstall
{
    class Program
    {
        private Configuration _configuration;
        private CommandLine _commandLine;

        static void Main(string[] args)
        {
            new Program().Start(args);
        }

        private void Start(string[] args)
        {
            _configuration = new Configuration();
            _configuration.ReadConfiguration();

            _commandLine = new CommandLine(_configuration);
            _commandLine.ReadCommandLine(args);

            if (_commandLine.IsHelpRequested)
            {
                Help();
            }
            else
            {
                Install();
            }

            if (_commandLine.IsWaitRequested)
            {
                Console.ReadKey();
            }
        }

        private void Help()
        {
            Console.WriteLine("NugetInstall v1.0 - Copyright(c) Raffaele Rialdi @raffaeler");
            Console.WriteLine("NugetInstall -h");
            Console.WriteLine("\tPrint help");
            Console.WriteLine("");
            Console.WriteLine("NugetInstall [-h] [-ns]");
            Console.WriteLine("  -h           Print Help");
            Console.WriteLine("  -ns          Does not install symbol packages");
            Console.WriteLine("  -s:path      The path where packages to install are located");
            Console.WriteLine("               If not specified the current folder is used");
            Console.WriteLine("  -p:package   Install only the specified package");
            Console.WriteLine("               If not specified all the packages are installed");
            Console.WriteLine("  -d           Delete the package files that are installed successfully");
            Console.WriteLine("  -t:path      Specifies the path of the nuget repository");
            Console.WriteLine("               If not specified the current folder is used");
            Console.WriteLine("  -l           Creates a log in the path of the nuget repository (-t)");
            Console.WriteLine("  -x           Defaults paths to executable folder instead of current folder");
            Console.WriteLine("  -w           Wait a key to be pressed before exiting");
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private string[] GetPackages()
        {
            return Directory.GetFiles(_commandLine.PackageFolder, _commandLine.PackageToSearch);
        }

        private string[] GetPackagesButSymbols()
        {
            var all = GetPackages();
            var symbols = all
                .Where(s => s.ToLower().Contains(_configuration.SymbolsMarker.ToLower()));

            return all.Except(symbols).ToArray();
        }

        private void Install()
        {
            _commandLine.SetCurrentDirectory();

            Log("Start");

            string[] packages;
            if (!_commandLine.AreSymbolsExcluded)
            {
                packages = GetPackages();
            }
            else
            {
                packages = GetPackagesButSymbols();
            }

            foreach (var packageFile in packages)
            {
                var cmdline = string.Format(_configuration.NugetParams, packageFile, _commandLine.NugetFolder);
                var ret = Execute(_configuration.NugetExe, cmdline);
                Log(_configuration.NugetExe + " " + cmdline + "   ==> " + ret.ToString());
                if (_commandLine.IsDeleteRequested)
                {
                    ret = Delete(packageFile);
                    Log(string.Format("Deleting: {0}  {1}", packageFile, ret == 0 ? "successfully" : "but failed"));
                }
            }
        }

        private int Execute(string exe, string args)
        {
            try
            {
                var psi = new ProcessStartInfo(exe, args);
                var process = Process.Start(psi);
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private int Delete(string filename)
        {
            try
            {
                File.Delete(filename);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private void Log(string message)
        {
            if (_commandLine.IsLogRequested)
            {
                var now = DateTime.Now;
                string logMessage = string.Format("{0}-{1}\t{2}\r\n",
                    now.ToShortDateString(),
                    now.ToShortTimeString(),
                    message);

                try
                {
                    File.AppendAllText(Defaults.LogFile, logMessage);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }

            Console.WriteLine(message);
        }
    }
}
