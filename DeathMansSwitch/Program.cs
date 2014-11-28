using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        private static List<KillTask> killTasks;

        /// <summary>
        /// Reads the specified configuration file and parses kill tasks.
        /// </summary>
        /// <param name="filename">The filename.</param>
        static void ReadConfigurationFile(string filename)
        {
            foreach (string line in File.ReadLines(filename))
            {
                // Lines are in format [process name];[hours]:[minutes]:[seconds]
                string[] splittedLine = line.Split(';');
                string processName = splittedLine[0];

                string[] timeUnits = splittedLine[1].Split(':');

                TimeSpan timeout = new TimeSpan(
                    Int32.Parse(timeUnits[0]), 
                    Int32.Parse(timeUnits[1]), 
                    Int32.Parse(timeUnits[2]));

                killTasks.Add(new KillTask(processName, timeout));
            }
        }

        static void Main(string[] args)
        {
            killTasks = new List<KillTask>();

            ReadConfigurationFile("tasks.config");

            POINT p;

            SpinWait spin = new SpinWait();
            while (true)
            {
                GetCursorPos(out p);

                if (lastX != p.X || lastY != p.Y)
                {
                    lastMovementTime = DateTime.Now;
                    lastX = p.X;
                    lastY = p.Y;

                    foreach (KillTask killTask in killTasks)
                    {
                        killTask.Killed = false;
                    }
                }

                foreach (KillTask killTask in killTasks)
                {
                    // Check if the kill task should be executed
                    killTask.CheckTimeout(lastMovementTime);
                }
                
                spin.SpinOnce();
            }


        }
    }
}
