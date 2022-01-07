using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpTori
{
    public abstract class THBase
    {
        private readonly IntPtr _handle;

        internal IntPtr Handle { get => _handle; }

        public THBase(IntPtr handle)
        {
            _handle = handle;
        }

        public abstract void Reset();

        public abstract bool IsNewGame();
    }
}
