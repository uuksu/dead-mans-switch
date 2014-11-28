using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeathMansSwitch
{
    class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private static DateTime lastMovementTime;
        private static int lastX;
        private static int lastY;

        static void Main(string[] args)
        {
            POINT p;
            while (true)
            {
                GetCursorPos(out p);

                if (lastX != p.X || lastY != p.Y)
                {
                    lastMovementTime = DateTime.Now;
                    lastX = p.X;
                    lastY = p.Y;
                }

                if ((DateTime.Now - lastMovementTime).Seconds >= 10)
                {
                    Process process = Process.GetProcesses().FirstOrDefault(i => i.ProcessName.Contains("HP.Sprout.ThreeDSnapshot"));

                    if (process != null)
                    {
                        process.Kill();
                        Console.WriteLine("{0} killed.", process.ProcessName);
                        lastMovementTime = DateTime.Now;
                    }
                }
            }


        }
    }
}
