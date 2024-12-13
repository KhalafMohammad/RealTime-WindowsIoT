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
            
        }

        public void Send_Pulse()
        {
            RealTime.GetThreadID(out int threadid);
            Console.WriteLine("ss ID: " + threadid);



            var watch = new Stopwatch(); // start stopwatch
            int j = 0;
            int k = 0;
            double minTime = double.MaxValue;
            double maxTime = double.MinValue;
            double totalTime = 0;
            int count = 0;

            if (Stopwatch.IsHighResolution)
            {
                Console.WriteLine("Operations timed using the system's high-resolution performance counter.");
            }
            else
            {
                Console.WriteLine("Operations timed using the DateTime class.");
            }

            while (true)
            {
                //QueryPerformanceCounter(out start);
                //for (int i = 0; i < 10; i++)
                //{
                //    j++;
                //}
                //k++;
                //if (k > 10000000){ k = 0; }
                //QueryPerformanceCounter(out stop);

                //double elapsedticks = stop - start;
                //elapsedTime = (elapsedticks / frequency) * 1_000_000;

                //// Update min, max, and total time
                //if (elapsedTime < minTime) minTime = elapsedTime;
                //if (elapsedTime > maxTime) maxTime = elapsedTime;
                //totalTime += elapsedTime;
                //count++;

                //double avgTime = totalTime / count;

                //Console.WriteLine($"Execution time: {elapsedTime} us, Min time: {minTime:f3} us, Max time: {maxTime:f3} us, Avg time: {avgTime:f3} us");

               
            }

            //watch.Start();
            //sp.Write("motor1 1000 L\n");
            //sp.Write("motor1 1000 L\n");
            //watch.Stop();
            //elapsedTime = watch.Elapsed.TotalMilliseconds;
            //sleepTime = targetPeriodMs - elapsedTime;
            //double executionTimeMs = watch.Elapsed.TotalMilliseconds;

            //double remainingTimeMs = targetPeriodMs - executionTimeMs;
            //if (sleepTime > 0)
            //{
            //    timeToSleep = sleepTime;
            //    watch.Reset();
            //    watch.Start();
            //    while (watch.Elapsed.TotalMilliseconds < timeToSleep)
            //    {
            //        // do nothing
            //    }
            //    watch.Stop();
            //}
        }
    }
}
