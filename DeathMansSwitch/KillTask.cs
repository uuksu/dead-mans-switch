using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathMansSwitch
{
    class KillTask
    {
        private readonly TimeSpan timeout;
        private readonly string processName;

        public bool Killed { get; set; }

        public KillTask(string processName, TimeSpan timeout)
        {
            this.processName = processName;
            this.timeout = timeout;
        }

        public void CheckTimeout(DateTime lastMovementTime)
        {
            if ((DateTime.Now - lastMovementTime) >= timeout && !Killed)
            {
                Process process = Process.GetProcesses().FirstOrDefault(i => i.ProcessName.Contains(processName));

                if (process != null)
                {
                    process.Kill();
                    Console.WriteLine("{0} killed.", process.ProcessName);
                    Killed = true;
                }
            }
        }
    }
}
