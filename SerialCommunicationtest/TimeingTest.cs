using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        }

        public void Send_Pulse(ref SerialPort sp)
        {
            QueryPerformanceFrequency(out frequency);

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
            ushort motor1 = 1000;
            char motor1_direction = 'L';
            ushort motor2 = 1000;
            char motor2_direction = 'L';
            //while (true)
            //{
            byte[] value_bytes = new byte[6];
            value_bytes[0] = (byte)((int)motor1 >> 8); // shift 8 bits to the right
            value_bytes[1] = (byte)((int)motor1 & 0xFF); // bitwise AND with 0xFF
            value_bytes[2] = (byte)((motor1_direction)); // write 1
            value_bytes[3] = (byte)((int)motor2 >> 8); // shift 8 bits to the right
            value_bytes[4] = (byte)((int)motor2 & 0xFF); // bitwise AND with 0xFF
            value_bytes[5] = (byte)((motor2_direction)); // write 1

            QueryPerformanceCounter(out start);
            sp.Write("m1 1000 R\n"); // write 1
            //string valueString = BitConverter.ToString(value_bytes).Replace("-", " ");
            //Console.WriteLine(Write.GetTimestamp() + " Wrote " + valueString + " over " + sp.PortName + ".");
            QueryPerformanceCounter(out stop);


            double elapsed1 = (stop - start) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");

            //double elapsedticks = stop - start;
            //elapsedTime = (elapsedticks / frequency) * 1000;

            //// Update min, max, and total time
            //if (elapsedTime < minTime) minTime = elapsedTime;
            //if (elapsedTime > maxTime) maxTime = elapsedTime;
            //totalTime += elapsedTime;
            //count++;

            //double avgTime = totalTime / count;

            //Console.WriteLine($"Execution time: {elapsedTime} us, Min time: {minTime:f3} us, Max time: {maxTime:f3} us, Avg time: {avgTime:f3} us");


            //}

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
