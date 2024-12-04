using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace WinSerialCommunication
{
    internal class NewScurve
    {




        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);


        public static float max_freq = 10000;
        public static int curr_freq = 0;
        public static float j_max = 5.000F;
        public static float t_j = 0.455F;
        public static float dt = 0.001F;
        public static int target = 3200; // target position
        public static int positie; // incoming position from the serial port
        public static int first_portion; // 1/3rd of the target
        public static int second_portion; // 2/3rds of the target
        public static Stopwatch watch = new Stopwatch(); // start stopwatch
        public static bool flag = true;


        public static void Sigmoid_curve(ref SerialPort sp)
        {
            //max_freq = Value;
            //max_freq = (int)Math.Round((double)Value / 3);
            Process process = Process.GetCurrentProcess();
            //for (int i = 0; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
            //{
            //    process.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
            //    Console.WriteLine("Thread ID: " + process.Threads[i].Id + " Priority: " + process.Threads[i].PriorityLevel);
            //}

            if (!QueryPerformanceFrequency(out long frequency))
            {
                throw new InvalidOperationException("Failed to query performance frequency");
            }

            double targetPeriodMs = 1.000;


            for (float i = t_j; i > 0; i -= dt)
            {
                watch.Restart();

                curr_freq = (int)Math.Round((max_freq / (1 + Math.Pow(1000000, (i + 1 / 2)))));
                sp.Write("motor1 " + curr_freq + " L\n");

                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

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


            }

            for (float i = j_max; i > t_j; i -= dt)
            {
                watch.Restart();

                sp.Write("motor1 " + curr_freq + " L\n");
                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

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

            }




            for (float i = 0; i < t_j; i += dt)
            {
                watch.Restart();

                curr_freq = (int)Math.Round((max_freq / (1 + Math.Pow(1000000, (i + 1 / 2)))));
                sp.Write("motor1 " + curr_freq + " L\n");
                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

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
            }
            sp.Write("motor1 0 L\n");

        }
    }
}
