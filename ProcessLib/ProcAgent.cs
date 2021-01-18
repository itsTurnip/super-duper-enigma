using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace DispatcherLib
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
        private Thread thread;
        private PerformanceCounter cpuCounter;
        private Process process;
        private volatile bool stop;
        public long RamUsage { get; private set; }
        public string Name { get; private set; }
        public float CpuUsage { get; private set; }
        public ProcAgent()
        {

        }
        public int GetPID()
        {
            return (process != null) ? process.Id : -1;
        }
        public bool SetPID(int pid)
        {
            try
            {
                process = Process.GetProcessById(pid);
                Name = process.ProcessName;
                cpuCounter = ProcessCpuCounter.GetPerfCounterForProcessId(process.Id);
                return true;
            } catch(ArgumentException)
            { 
                return false;
            }
        }
        private bool UpdateProcInfo()
        {
            if (process != null && !process.HasExited && cpuCounter != null)
            {
                try
                {
                    process.Refresh();
                    RamUsage = process.WorkingSet64 / (1024 * 1024);
                    CpuUsage = cpuCounter.NextValue();
                    return true;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    return false;
                }
            }
            else if (process != null)
            {
                process.Dispose();
                process = null;
            }
            
            return false;
        }
        private void Loop()
        {
            while (!stop && UpdateProcInfo()) ;
        }
        
        public void Listen()
        {
            thread = new Thread(Loop)
            {
                IsBackground = true,
            };
        }
        public void Stop()
        {
            stop = true;
        }
        public override string ToString()
        {
            return $"{GetPID()}   {Name}    {CpuUsage / 4}%\t{RamUsage}MB";
        }
        public bool Kill()
        {
            try
            {
                stop = true;
                process.Kill();
                process.Dispose();
                cpuCounter.Dispose();
                process = null;
                return true;
            } catch (Exception)
            {
                return false;
            }
        }
        ~ProcAgent()
        {
            if(process != null)
                process.Dispose();
            if (cpuCounter != null)
                cpuCounter.Dispose();
        }
    }
}
