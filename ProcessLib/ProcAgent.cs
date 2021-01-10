using System;
using System.Runtime.InteropServices;

namespace ProcessLib
{
    [ComVisible(true)]
    [Guid("fa578469-f3fa-4f44-bd94-81c1e759e56c")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ProcAgent
    {
        public int ProcId { get; }
        public ProcAgent(int procId)
        {
            ProcId = procId;
        }
    }
}
