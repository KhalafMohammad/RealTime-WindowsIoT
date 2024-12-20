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

    internal class TimerTester
    {

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public void Exicute()
        {

            // How many iterations you want to run
            const int iterations = 100;

            // Target time period for 1kHz (1ms)
            if (!QueryPerformanceFrequency(out long frequency))
            {
                throw new InvalidOperationException("Failed to query performance frequency");
            }

            var stopwatch = new Stopwatch();

            const double targetPeriodMs = 1.0000;

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

                double elapsed = (end - start) * 1000.0 / frequency;
                double remainingTimeMs = targetPeriodMs - elapsed;
                double old_elapsed = elapsed;


                if (remainingTimeMs > 0)
                {
                    while (elapsed < remainingTimeMs)
                    {
                        QueryPerformanceCounter(out end);
                        Thread.SpinWait(1);
                        elapsed = (end - start) * 1000.0 / frequency;
                    }
                    QueryPerformanceCounter(out long Nend);
                    double newelapsed = (Nend - start) * 1000.0 / frequency;
                    Console.WriteLine($"Iteration {result:f2}:  Target delay {targetPeriodMs}ms, Old elapsed {old_elapsed:f5}ms, Waited {remainingTimeMs:f5}, After delay {elapsed:f5}ms, Operation time {newelapsed:f5}");
                }


                // another way to wait for the remaining time
                //#####################################################################################

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

                //#####################################################################################


                
            }

            Console.WriteLine($"Total time: {stopwatch.Elapsed.TotalMilliseconds/1000:f2}s");
        }
    }

}
