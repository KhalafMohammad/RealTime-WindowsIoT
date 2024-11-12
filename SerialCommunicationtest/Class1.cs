// timers tester class for real-time performance

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace WinSerialCommunication
{


    internal class Newclass
    {


        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public void Exicute()
        {
            
            Process process = Process.GetCurrentProcess();
            for (int i = 1; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
            {
                process.Threads[i].ProcessorAffinity = (IntPtr)0xF0; // use only the first processor
                process.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
                Console.WriteLine("Thread ID: " + process.Threads[i].Id + " Priority: " + process.Threads[i].PriorityLevel);
            }
            

            // How many iterations you want to run
            const int iterations = 100;

            // Target time period for 1kHz (1ms)
            if (!QueryPerformanceFrequency(out long frequency))
            {
                throw new InvalidOperationException("Failed to query performance frequency");
            }

            var stopwatch = new Stopwatch();

            const double targetPeriodMs = 1.0;
            double targetPeriodnt = 1000.0 / frequency; // / frequency
            Console.WriteLine($"Target period: {targetPeriodnt}ms, frequency: {frequency}");
            stopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {

                //stopwatch.Restart();
                QueryPerformanceCounter(out long start);


                // Your loop contents here
                // For example:
                //double result = Math.Sin(i) * Math.Cos(i);
                double result = Math.Pow(i, 3) + Math.Sqrt(i) * Math.Log(i + 1) - Math.Sin(i) * Math.Cos(i);


                QueryPerformanceCounter(out long end);

                double elapsed = (end - start) * targetPeriodnt;
                double remainingTimeMs = targetPeriodMs - elapsed;
                double old_elapsed = elapsed;


                if (remainingTimeMs > 0)
                {
                    while ((end - start) * targetPeriodnt < remainingTimeMs)
                    {
                        
                        Thread.SpinWait(1);
                        QueryPerformanceCounter(out end);
                        //elapsed = (end - start) * targetPeriodnt;
                    }
                    QueryPerformanceCounter(out long Nend);
                    double newelapsed = (Nend - start) * targetPeriodnt;
                    Console.WriteLine($"Iteration {result:f2}:  Target delay {targetPeriodMs}ms, Old elapsed {old_elapsed:f5}ms, Waited {remainingTimeMs:f5}, After delay {elapsed:f5}ms, Operation time {newelapsed:f5}");
                }



                // Calculate how long the operation took
                //double executionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

                // Calculate remaining time to reach 1ms period
                //double remainingTimeMs = targetPeriodMs - executionTimeMs;

                //if (remainingTimeMs > 0)
                //{
                //    // Precise waiting for the remaining time
                //    var waitTimer = new Stopwatch();
                //    waitTimer.Start();
                //    while (waitTimer.Elapsed.TotalMilliseconds < remainingTimeMs)
                //    {
                //        Thread.SpinWait(1);
                //    }
                //}

                //  Optional: Print timing information every 100 iterations
                //if (i % 100 == 0)
                //{
                //    Console.WriteLine($"Iteration {i}: Operation took {executionTimeMs:F3}ms, " +
                //                    $"Waited {Math.Max(0, remainingTimeMs):F3}ms");
                //}
            }
            
            Console.WriteLine($"Total time: {stopwatch.Elapsed.TotalMilliseconds/1000:f2}s");
        }
    }

}
