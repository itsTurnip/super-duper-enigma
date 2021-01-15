using System;
using System.Runtime.InteropServices;

namespace ProcessLib
{
    [ComVisible(true)]
    [Guid("7b1b3989-054f-406a-91c9-96ed7104f7d9")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ProcAgent
    {
        public int ProcId { get; }
        public ProcAgent()
        {

        }
        public double RetDoub()
        {
            return 5.0;
        }
    }
}
