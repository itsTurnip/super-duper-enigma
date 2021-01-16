using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace ProcessLib
{
    //[ComVisible(true)]
    //[Guid("8b79ee71-8c1d-4b20-90cd-cf8b5b3ecb47")]
    //[InterfaceType(ComInterfaceType.InterfaceIsDual)]
    //public interface IProc
    //{
    //    int GetPID();
    //    bool SetPID(int PID);
    //}
    [ComVisible(true)]
    [Guid("7b1b3989-054f-406a-91c9-96ed7104f7d9")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ProcAgent
    {
        private PerformanceCounter cpuCounter;
        private Process process;
        public long RamUsage { get; private set; }
        public string Name { get; private set; }
        public float CpuUsage { get; private set; }
        public ProcAgent()
        {

        }
        public int GetPID()
        {
            return process.Id;
        }
        public bool SetPID(int pid)
        {
            Process process;
            try
            {
                process = Process.GetProcessById(pid);
                this.process = process;
                Name = process.ProcessName;
                cpuCounter = ProcessCpuCounter.GetPerfCounterForProcessId(process.Id);
                return true;

            } catch(ArgumentException)
            {
                return false;
            }
        }
        public bool UpdateProcInfo()
        {
            if (!process.HasExited)
            {
                try
                {
                    process.Refresh();
                    RamUsage = process.WorkingSet64 / (1024 * 1024);
                    CpuUsage = cpuCounter.NextValue();
                    return true;
                } catch (System.ComponentModel.Win32Exception)
                {
                    return false;
                }
            }
            return false;
        }
        public override string ToString()
        {
            return $"{GetPID()}   {Name}    {CpuUsage / 4}%\t{RamUsage}MB";
        }
        public bool Kill()
        {
            try
            {
                process.Kill();
                return true;
            } catch (Exception)
            {
                return false;
            }
        }
    }
}
