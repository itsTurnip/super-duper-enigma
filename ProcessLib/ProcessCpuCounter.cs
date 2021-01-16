using System.Linq;
using System.Diagnostics;
using System.IO;

internal static class ProcessCpuCounter
{
    internal static PerformanceCounter GetPerfCounterForProcessId(int processId, string processCounterName = "% Processor Time")
    {
        string instance = GetInstanceNameForProcessId(processId);
        if (string.IsNullOrEmpty(instance))
            return null;

        return new PerformanceCounter("Process", processCounterName, instance);
    }

    internal static string GetInstanceNameForProcessId(int processId)
    {
        var process = Process.GetProcessById(processId);
        string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

        PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
        string[] instances = cat.GetInstanceNames()
            .Where(inst => inst.StartsWith(processName))
            .ToArray();

        foreach (string instance in instances)
        {
            using (PerformanceCounter cnt = new PerformanceCounter("Process",
                "ID Process", instance, true))
            {
                int val = (int)cnt.RawValue;
                if (val == processId)
                {
                    return instance;
                }
            }
        }
        return null;
    }
}