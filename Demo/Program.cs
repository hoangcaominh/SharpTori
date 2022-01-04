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
            IntPtr handle = MemoryReader.GetHandle("th17");
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Error opening process. Last Win32 error: {0}", Marshal.GetLastWin32Error());
                return;
            }

            TH17 game = new TH17(handle);
            TH17.HyperCount hyperCount;
            while (MemoryReader.IsProcessAlive(handle))
            {
                Console.Clear();

                game.AutoReset();
                hyperCount = game.GetHyperCount();

                Console.WriteLine(game.GetMissCount());
                Console.WriteLine(game.GetBombCount());
                Console.WriteLine("{0} {1} {2} {3}", hyperCount.Wolf, hyperCount.Otter, hyperCount.Eagle, hyperCount.Break);

                Thread.Sleep(1);
            }
        }
    }
}
