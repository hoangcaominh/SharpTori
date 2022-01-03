using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpTori
{
    /// <summary>
    /// A class for reading memory from a 32-bit application.
    /// </summary>
    public static class MemoryReader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetExitCodeProcess(IntPtr hProcess, out uint ExitCode);

        /// <summary>
        /// Get the handle of the process with the specified name.
        /// </summary>
        /// <param name="processName">The name of the process to get the handle from.</param>
        /// <returns>The handle of the specified process if the process exists, and IntPtr.Zero if otherwise.</returns>
        public static IntPtr GetHandle(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            // Return IntPtr.Zero if there are no processes
            if (processes.Length == 0)
                return IntPtr.Zero;

            // Open process and get handle
            return OpenProcess(0x1010, false, processes.FirstOrDefault().Id); // 0x1010 = PROCESS_VM_READ | PROCESS_QUERY_LIMITED_INFORMATION
        }

        /// <summary>
        /// Check whether the process is running.
        /// </summary>
        /// <param name="handle">The handle of the process</param>
        /// <returns>The result indicating whether the process is running.</returns>
        public static bool IsProcessAlive(IntPtr handle)
        {
            if (!GetExitCodeProcess(handle, out uint status))
            {
                Console.WriteLine("Error getting exit code. Last Win32 error: {0}", Marshal.GetLastWin32Error());
                return false;
            }
            return status == 259;
        }

        /// <summary>
        /// Reads the memory from a specified list of address offsets and write the value read to target.
        /// </summary>
        /// <typeparam name="T">The generic type representing the target type.</typeparam>
        /// <param name="handle">The handle to read the memory from.</param>
        /// <param name="offsets">The list of offsets to be applied to the reading address.</param>
        /// <param name="target">The object to be written with the value read from the memory.</param>
        /// <param name="byteCount"></param>
        /// <returns>The result of the memory reading.</returns>
        public static bool ReadMemory<T>(IntPtr handle, uint[] offsets, ref T target, int byteCount)
        {
            bool success = true;
            try
            {
                uint address = 0;
                for (int i = 0; i < offsets.Length; i++)
                {
                    // Get the address we want to read.
                    address += offsets[i];

                    // Create buffer with size based on the current offset.
                    byte[] buffer = new byte[(i == offsets.Length - 1) ? byteCount : sizeof(uint)];

                    // Read memory should succeed. It can also throw exception in some circumstances.
                    if (!ReadProcessMemory(handle, (IntPtr)address, buffer, buffer.Length, out _))
                        throw new Exception(string.Format("Failed to read memory at address {0:X8}. Last Win32 error code: {1}", address, Marshal.GetLastWin32Error()));

                    // If i is at the end of the array, deserialize the byte array. Otherwise parse buffer as the base address.
                    if (i == offsets.Length - 1)
                        target = buffer.ToStructure<T>();
                    else
                        address = BitConverter.ToUInt32(buffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("MemoryReader Exception: {0}", e.Message);
                success = false;
            }
            return success;
        }
    }
}
