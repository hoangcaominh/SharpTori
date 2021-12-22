using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SharpTori;

namespace Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IntPtr handle = MemoryReader.GetHandle("th07");
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Error opening process. Last Win32 error: {0}", Marshal.GetLastWin32Error());
                return;
            }

            TH07 game = new TH07(handle);
            while (MemoryReader.IsProcessAlive(handle))
            {
                Console.Clear();
                Console.WriteLine("{0} {1}", game.GetMainShot(), game.GetSubShot());
                Console.WriteLine("{0:N0}", game.GetScore());
                Console.WriteLine(game.GetMissCount());
                Console.WriteLine(game.GetBombCount());
                Console.WriteLine(game.GetBorderBreakCount());
                Thread.Sleep(100);
            }
        }
    }
}
