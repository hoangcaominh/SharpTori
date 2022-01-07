using System;
using System.Runtime.InteropServices;

namespace SharpTori
{
    public static class Extensions
    {
        /// <summary>
        /// Convert byte array to a generic type.
        /// </summary>
        /// <typeparam name="T">The generic type to convert to.</typeparam>
        /// <param name="value">The byte array to be converted.</param>
        /// <returns>The result of the conversion of type T</returns>
        public static T ToStructure<T>(this byte[] value)
        {
            T ret = default;
            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            try
            {
                ret = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            catch (Exception e)
            {
                Console.WriteLine("Byte array conversion error: {0}", e.Message);
            }
            finally
            {
                handle.Free();
            }
            return ret;
        }
    }
}
