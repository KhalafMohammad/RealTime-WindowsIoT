using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading;

namespace WinSerialCommunication
{


    internal class Newclass
    {
        public static void Exicute()
        {
            // How many iterations you want to run
            const int iterations = 1000;

            // Target time period for 1kHz (1ms)
            const double targetPeriodMs = 1.0;

            var stopwatch = new Stopwatch();

            for (int i = 0; i < iterations; i++)
            {
                stopwatch.Restart();

                // Your loop contents here
                // For example:
                double result = Math.Sin(i) * Math.Cos(i);

                // Calculate how long the operation took
                double executionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

                // Calculate remaining time to reach 1ms period
                double remainingTimeMs = targetPeriodMs - executionTimeMs;

                if (remainingTimeMs > 0)
                {
                    // Precise waiting for the remaining time
                    var waitTimer = new Stopwatch();
                    waitTimer.Start();
                    while (waitTimer.Elapsed.TotalMilliseconds < remainingTimeMs)
                    {
                        Thread.SpinWait(1);
                    }
                }

                // Optional: Print timing information every 100 iterations
                if (i % 100 == 0)
                {
                    Console.WriteLine($"Iteration {i}: Operation took {executionTimeMs:F3}ms, " +
                                    $"Waited {Math.Max(0, remainingTimeMs):F3}ms");
                }
            }
        }
    }

}
