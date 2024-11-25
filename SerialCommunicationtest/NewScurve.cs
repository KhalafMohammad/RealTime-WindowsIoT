using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinSerialCommunication
{

    internal class NewScurve
    {


        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);


        public static float max_freq = 2000;
        public static int curr_freq = 0;
        public static float acc_max = 10000;
        public static float j_max = 3.2F;
        public static float t_j = 0.5F;
        public static float dt = 0.001F;
        public static int target = 3200; // target position
        public static int positie; // incoming position from the serial port
        public static int first_portion; // 1/3rd of the target
        public static int second_portion; // 2/3rds of the target
        public static bool flag = true;
        public static Stopwatch watch = new Stopwatch(); // start stopwatch




        public static void Sigmoid_curve(ref SerialPort sp)
        {
            RealTime.Process_managment(Process.GetCurrentProcess(), (IntPtr)0xF0, ProcessPriorityClass.RealTime);
            RealTime.Threads_managment(Process.GetCurrentProcess(), ThreadPriorityLevel.Highest);


            if (!QueryPerformanceFrequency(out long frequency))
            {
                throw new InvalidOperationException("Failed to query performance frequency");
            }
            double targetPeriodMs = 1.0;

            first_portion = (int)Math.Round((double)target / 3);
            second_portion = first_portion * 2;
            flag = true;

            for (float i = t_j; i > 0; i -= dt)
            {
                watch.Restart();

                //watch.Restart();
                curr_freq = (int)Math.Round((max_freq / (1 + Math.Pow(1000000, (i + 1 / 2)))));
                Write.data(ref sp, curr_freq);
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

            for (float i = 3.2f; i > 0; i -= dt)
            {
                watch.Restart();

                Write.data(ref sp, curr_freq);
                if (!flag)
                {
                    break;
                }
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
                if (!flag)
                {
                    break;
                }
                
                curr_freq = (int)Math.Round((max_freq / (1 + Math.Pow(1000000, (i + 1 / 2)))));
                Write.data(ref sp, curr_freq);
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
        }
    }
}
