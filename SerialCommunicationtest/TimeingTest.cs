using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    class TimeingTest
    {
        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        const double targetPeriodMs = 1.0000f;
        private static long frequency;
        private static long start;
        private static long stop;
        private static double elapsedTime;
        private static double sleepTime;
        private static double timeToSleep;



        public TimeingTest()
        {
            QueryPerformanceFrequency(out frequency);
            Process process = Process.GetCurrentProcess();
            process.ProcessorAffinity = 0xF00; // use only the last two processor
            process.PriorityClass = ProcessPriorityClass.RealTime;
            Console.WriteLine("Processor affinity: " + process.ProcessorAffinity);

            for (int i = 0; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
            {

                process.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
                process.Threads[i].ProcessorAffinity = 0xF00;

            }
        }

        public void Send_Pulse(ref SerialPort sp, int steps)
        {
            var  watch = new Stopwatch(); // start stopwatch
            //sp.Write("motor1 1000 L\n");
            //sp.Write("motor1 1000 L\n");

            for (int i = 0; i <= 416 ; i++)
            {

                watch.Restart();
                string steps_string = i.ToString();
                sp.Write("motor1 " + steps_string + " R\n");
                Console.WriteLine(i);
                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

                double remainingTimeMs = targetPeriodMs - executionTimeMs;

                if (remainingTimeMs > 0)
                {
                    var waitTimer = new Stopwatch();
                    waitTimer.Start();
                    while (waitTimer.Elapsed.TotalMilliseconds < remainingTimeMs)
                    {
                        Thread.SpinWait(1);
                    }
                }
                
            }
            sp.Write("motor1 0 R\n");
            sp.Write("motor1 0 R\n");
            Console.WriteLine("Done");
            

        }

    }
}
