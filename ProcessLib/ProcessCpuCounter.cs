﻿using System.Linq;
using System.Diagnostics;
using System.IO;
namespace ProcessLib
{
    internal static class ProcessCpuCounter
    {
        internal static PerformanceCounter GetPerfCounterForProcessId(Process process, string processCounterName = "% Processor Time")
        {
            string instance = GetInstanceNameForProcessId(process);
            if (string.IsNullOrEmpty(instance))
                return null;

            return new PerformanceCounter("Process", processCounterName, instance);
        }

        internal static string GetInstanceNameForProcessId(Process process)
        {
            string processName = process.ProcessName.Replace(".exe", "");

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
                    if (val == process.Id)
                    {
                        return instance;
                    }
                }
            }
            return null;
        }
    }
}