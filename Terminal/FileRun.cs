using System;

namespace Terminal
{
    internal class FileRun
    {
        public static void ExecuteCommand()
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + " regsvr32" + @" WSCRun.wsc")
            {
                CreateNoWindow = false,
                Verb = "runas",
                UseShellExecute = true
            };
            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                process.WaitForExit();
            }

            Type procT = Type.GetTypeFromProgID("WSCRun.WSC");
            dynamic proc = Activator.CreateInstance(procT);
            proc.Run();
        }
    }
}