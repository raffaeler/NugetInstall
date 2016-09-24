using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// command lines to test:
// -ns -s:h:\_nuget -t:h:\_nuget -d -w
// -ns -s:h:\_nuget -t:h:\_nuget -w
// -ns -s:h:\_nuget -t:h:\_nuget -h -w

namespace NugetInstall
{
    class Program
    {
        private string _nugetExe = Defaults.NugetExe;
        private string _nugetExt = Defaults.NugetExt;
        private string _symbolsMarker = Defaults.SymbolsMarker;
        private string _nugetParams = Defaults.NugetParams;

        private string _packageFolder;
        private string _nugetFolder;
        private string _packageToSearch;
        private bool _areSymbolsExcluded;
        private bool _isHelpRequested;
        private bool _isDeleteRequested;
        private bool _isLogRequested;
        private bool _isWaitRequested;
        private bool _useExePath;

        public string CurrentFolder { get; set; }

        static void Main(string[] args)
        {
            new Program().Start(args);
        }

        private string GetExePath()
        {
            var filename = System.Reflection.Assembly.GetEntryAssembly().Location;
            var path = filename.Substring(0, filename.LastIndexOf('\\'));
            return path;
        }

        private void Start(string[] args)
        {
            CurrentFolder = Directory.GetCurrentDirectory();
            ReadConfiguration();
            ReadCommandLine(args);

            if (_isHelpRequested)
            {
                Help();
            }
            else
            {
                Install();
            }

            if (_isWaitRequested)
            {
                Console.ReadKey();
            }
        }

        private void ReadConfiguration()
        {
            string temp;
            temp = ConfigurationManager.AppSettings["NugetExt"];
            if (!string.IsNullOrEmpty(temp))
                _nugetExt = temp;

            temp = ConfigurationManager.AppSettings["SymbolsMarker"];
            if (!string.IsNullOrEmpty(temp))
                _symbolsMarker = temp.ToLower();

            temp = ConfigurationManager.AppSettings["NugetParams"];
            if (!string.IsNullOrEmpty(temp))
                _nugetParams = temp;
        }

        private void ReadCommandLine(string[] args)
        {
            if (args.Match("-h"))
            {
                _isHelpRequested = true;
            }

            if (args.Match("-l"))
            {
                _isLogRequested = true;
            }

            if (args.Match("-x"))
            {
                _useExePath = true;
            }

            if (args.Match("-ns"))
            {
                _areSymbolsExcluded = true;
            }

            if (args.Match("-d"))
            {
                _isDeleteRequested = true;
            }

            if (args.Match("-w"))
            {
                _isWaitRequested = true;
            }

            _packageFolder = args.GetParam("-s:");
            if (_packageFolder == null)
            {
                if (_useExePath)
                {
                    _packageFolder = GetExePath();
                }
                else
                {
                    _packageFolder = CurrentFolder;
                }
            }

            _nugetFolder = args.GetParam("-t:");
            if (_nugetFolder == null)
            {
                if (_useExePath)
                {
                    _nugetFolder = GetExePath();
                }
                else
                {
                    _nugetFolder = CurrentFolder;
                }
            }

            _packageToSearch = args.GetParam("-p:");
            if (_packageToSearch == null)
            {
                _packageToSearch = "*." + _nugetExt;
            }
        }

        private string[] GetPackages()
        {
            return Directory.GetFiles(_packageFolder, _packageToSearch);
        }

        private string[] GetPackagesButSymbols()
        {
            var all = GetPackages();
            var symbols = all
                .Where(s => s.ToLower().Contains(_symbolsMarker.ToLower()));

            return all.Except(symbols).ToArray();
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

        private void Install()
        {
            if (_useExePath)
            {
                Directory.SetCurrentDirectory(GetExePath());
            }
            else
            {
                Directory.SetCurrentDirectory(_nugetFolder);
            }

            Log("Start");

            string[] packages;
            if (!_areSymbolsExcluded)
            {
                packages = GetPackages();
            }
            else
            {
                packages = GetPackagesButSymbols();
            }

            foreach (var packageFile in packages)
            {
                var cmdline = string.Format(_nugetParams, packageFile, _nugetFolder);
                var ret = Execute(_nugetExe, cmdline);
                Log(_nugetExe + " " + cmdline + "   ==> " + ret.ToString());
                if (_isDeleteRequested)
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
            if (_isLogRequested)
            {
                string logMessage = string.Format("{0}-{1}\t{2}\r\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), message);
                try
                {
                    File.AppendAllText("NugetInstall.log", logMessage);
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
