using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace DispatcherLib
{
    [ComVisible(true)]
    [Guid("8251bda3-9a44-4628-bfeb-d5d6427b37af")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Dispatcher
    {
        private const int TH32CS_SNAPPROCESS = 0x2;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(int dwFlags, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 peProcessEntry);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 peProcessEntry);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
        private volatile bool stop = false;

        private ThreadSafeList<object> procs = new ThreadSafeList<object>();
        private const int procInfoSize = 296;
        private Thread listener;
        private ManagementEventWatcher watcher = new ManagementEventWatcher("SELECT * " +
            "FROM __InstanceCreationEvent WITHIN 1" +
            "WHERE TargetInstance ISA 'Win32_Process'");
        private IEnumerable<PROCESSENTRY32> ProcEntries()
        {
            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            PROCESSENTRY32 procInfo = new PROCESSENTRY32
            {
                dwSize = procInfoSize
            };
            try
            {
                if (hSnapshot != null && Process32First(hSnapshot, ref procInfo))
                {
                    do
                    {
                        yield return procInfo;
                    } while (Process32Next(hSnapshot, ref procInfo));
                }
            } finally
            {
                if (hSnapshot != null)
                    CloseHandle(hSnapshot);
            }
        }
        public Dispatcher()
        {
            foreach (var procInfo in ProcEntries())
            {
                object proc = CreateProc(procInfo.th32ProcessID);
                if (proc != null)
                    procs.Add(proc);
            }
            watcher.EventArrived += Watcher_EventArrived;
            watcher.Start();
        }

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject baseObject = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            object p = baseObject["ProcessId"];
            int pid;
            if (p != null)
                pid = Convert.ToInt32(p);
            else
                return;
            dynamic proc = CreateProc(pid);
            if (proc != null)
            {
                procs.Add(proc);
                proc.Listen();
            }
        }

        private object CreateProc(int pid)
        {
            Type procT = Type.GetTypeFromProgID("ProcessLib.ProcAgent");
            dynamic proc = Activator.CreateInstance(procT);
            if (!proc.SetPID(pid))
                return null;
            proc.Listen();
            return proc;
        }
        public IList<object> GetList()
        {
            return procs;
        }
        public bool Kill(int num)
        {
            
            dynamic proc = procs[num];
            if (proc.Kill())
            {
                procs.RemoveAt(num);
                return true;
            }
            return false;
        }
        public void Stop()
        {
            stop = true;
        }
        const int UpdateDelay = 2000;
        private void Update()
        {
            while (!stop)
            {
                List<object> dead = new List<object>();
                foreach(dynamic proc in procs)
                {
                    if (proc.Exited)
                        dead.Add(proc);
                }
                foreach (object proc in dead)
                    procs.Remove(proc);
                Thread.Sleep(UpdateDelay);
            }

        }
        public void Listen()
        {
            listener = new Thread(Update) { IsBackground = true };
            listener.Start();
        }
    }
}
