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


        public static float max_freq = 3200;
        public static int curr_freq = 0;
        public static float acc_max = 10000;
        public static float j_max = 1.5F;
        public static float t_j = 0.5F;
        public static float dt = 0.005F;
        public static int target = 3200; // target position
        public static int positie; // incoming position from the serial port
        public static int first_portion; // 1/3rd of the target
        public static int second_portion; // 2/3rds of the target
        public static Stopwatch watch = new Stopwatch(); // start stopwatch
        public static bool flag = true;




        public static void Sigmoid_curve(ref SerialPort sp)
        {

            Process process = Process.GetCurrentProcess();
            for (int i = 0; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
            {
                process.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
                Console.WriteLine("Thread ID: " + process.Threads[i].Id + " Priority: " + process.Threads[i].PriorityLevel);
            }

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
                QueryPerformanceCounter(out long start);
                //watch.Restart();
                curr_freq = (int)Math.Round((max_freq / (1 + Math.Pow(1000000, (i + 1 / 2)))));
                Write.data(ref sp, curr_freq);
                //watch.Stop();
                //int elapsed = (int)watch.ElapsedMilliseconds;
                //if (elapsed < 1)
                //{
                //    Thread.Sleep(1 - elapsed);
                //}

                QueryPerformanceCounter(out long end);
                double elapsed = (end - start) * 1000.0 / frequency;
                double remainingTimeMs = targetPeriodMs - elapsed;
                if (remainingTimeMs > 0)
                {
                    while ((end - start) * 1000.0 / frequency < remainingTimeMs)
                    {
                        QueryPerformanceCounter(out end);
                        //elapsed = (end - start) * 1000.0 / frequency;
                    }
                }
            }

            for (float i = t_j; i > 0; i -= dt)
            {
                QueryPerformanceCounter(out long start);

                Write.data(ref sp, curr_freq);
                if (!flag)
                {
                    break;
                }

                QueryPerformanceCounter(out long end);

                double elapsed = (end - start) * 1000.0 / frequency;
                double remainingTimeMs = targetPeriodMs - elapsed;
                if (remainingTimeMs > 0)
                {
                    while ((end - start) * 1000.0 / frequency < remainingTimeMs)
                    {
                        QueryPerformanceCounter(out end);
                        //elapsed = (end - start) * 1000.0 / frequency;
                    }
                }
            }




            for (float i = 0; i < t_j; i += dt)
            {
                QueryPerformanceCounter(out long start);
                if (!flag)
                {
                    break;
                }
                //watch.Restart();
                curr_freq = (int)Math.Round((max_freq / (1 + Math.Pow(1000000, (i + 1 / 2)))));
                Write.data(ref sp, curr_freq);
                QueryPerformanceCounter(out long end);
                double elapsed = (end - start) * 1000.0 / frequency;
                double remainingTimeMs = targetPeriodMs - elapsed;
                if (remainingTimeMs > 0)
                {
                    while ((end - start) * 1000.0 / frequency < remainingTimeMs)
                    {
                        QueryPerformanceCounter(out end);
                        //elapsed = (end - start) * 1000.0 / frequency;
                    }
                }
            }
        }
    }
}
