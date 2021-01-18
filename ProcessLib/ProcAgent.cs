using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

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
    public class ProcAgent: IDisposable
    {
        private PerformanceCounter cpuCounter;
        private Process process;
        private volatile bool stop;
        public bool Exited { get => process.HasExited; }
        public long RamUsage { get; private set; }
        public string Name { get; private set; }
        public double CpuUsage { get; private set; }
        private TimeSpan lastCpuTime;
        private Stopwatch stopwatch;
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
                return true;
            } catch(ArgumentException)
            { 
                return false;
            }
        }
        private bool UpdateProcInfo()
        {
            try
            {
                if (process != null && !process.HasExited )
                {
                    TimeSpan cpuTime = process.TotalProcessorTime;
                    stopwatch.Stop();

                    process.Refresh();
                    RamUsage = process.WorkingSet64 / (1024 * 1024);
                    CpuUsage = (cpuTime - lastCpuTime).TotalMilliseconds / (Environment.ProcessorCount * stopwatch.ElapsedMilliseconds);
                    lastCpuTime = cpuTime;
                    stopwatch.Restart();
                    return true;
                }
                else
                {
                    Dispose();
                    return false;
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return false;
            }
        }
        private void Loop()
        {
            stopwatch = Stopwatch.StartNew();
            while (!stop && UpdateProcInfo()) 
            {
                Thread.Sleep(3000);
            }
        }
        
        public void Listen()
        {
            Thread thread = new Thread(Loop)
            {
                IsBackground = true,
            };
            thread.Start();
        }
        public void StopListen()
        {
            stop = true;
        }
        public override string ToString()
        {
            const int maxLength = 25;
            string name = (Name.Length > maxLength) ? Name.Substring(0, maxLength) : Name.PadRight(maxLength);
            return $"{GetPID()}  \t{name}\t{CpuUsage * 100:0.#}%\t{RamUsage}MB";
        }
        public bool Kill()
        {
            try
            {
                process.Kill();
                Dispose();
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            stop = true;
            if (process != null)
            {
                process.Dispose();
                process = null;
            }
            if (cpuCounter != null)
            {
                cpuCounter.Dispose();
                cpuCounter = null;
            }

        }

        ~ProcAgent()
        {
            Dispose();
        }
    }
}
