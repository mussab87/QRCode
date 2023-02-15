using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility.Toolkit.Unmanaged
{
    public class UnmanagedMemory : IDisposable
    {
        private IntPtr _ptrToUnmanagedMemory = IntPtr.Zero;

        public UnmanagedMemory(int amountToAllocate)
        {
            _ptrToUnmanagedMemory = Marshal.AllocHGlobal(amountToAllocate);
        }

        public IntPtr PtrToUnmanagedMemory
        {
            get { return _ptrToUnmanagedMemory; }
        }

        public void Dispose()
        {
            if (_ptrToUnmanagedMemory != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_ptrToUnmanagedMemory);
                _ptrToUnmanagedMemory = IntPtr.Zero;
            }
        }
    }
}