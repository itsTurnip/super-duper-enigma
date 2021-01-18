using System.Runtime.InteropServices;

namespace DispatcherLib
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESSENTRY32
    {
        public int dwSize;
        public int cntUsage;
        public int th32ProcessID;
        public long th32DefaultHeapID;
        public int th32ModuleID;
        public int cntThreads;
        public int th32ParentProcessID;
        public long pcPriClassBase;
        public int dwFlags;
        public char szExeFile;
    }
}
