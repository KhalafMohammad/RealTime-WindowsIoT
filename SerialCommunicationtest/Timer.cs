using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    internal class Timer
    {
        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);


        private double targetPeriodnt;
        private const double targetPeriodMs = 1.0;
        private long freq;



        public Timer()
        {
            // set current threads on realtime and a timecritical priority level
            Process process = Process.GetCurrentProcess();
            process.ProcessorAffinity = 0xF00; // use only the first processor
            process.PriorityClass = ProcessPriorityClass.RealTime;

            Console.WriteLine("Processor affinity: " + process.ProcessorAffinity.ToString("X"));
            for (int i = 0; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
            {

                process.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
                process.Threads[i].ProcessorAffinity = 0xF00;
            }
            Console.WriteLine("Processor affinity: " + process.ProcessorAffinity.ToString("X"));

            Console.WriteLine($"Process base priority: {process.BasePriority}");

            if (!QueryPerformanceFrequency(out long frequency))
            {
                Console.WriteLine("Error, Timer.cs: QueryPerformanceFrequency failed.");

            }
            this.freq = frequency;
            targetPeriodnt = 1000.0 / freq;

        }

        public double BeginCount()
        {
            QueryPerformanceCounter(out long start);
            return start;
        }

        public void EndCountDelay(long time_started)
        {

            QueryPerformanceCounter(out long stop);
            double elapsed = (stop - time_started) * targetPeriodnt;
            double remaining = targetPeriodMs - elapsed;
            double oldlap = elapsed;
            if (remaining < targetPeriodMs)
            {
                while ((stop - time_started) * targetPeriodnt < remaining)
                {
                    QueryPerformanceCounter(out stop);
                }
                double elapsed2 = (stop - time_started) * targetPeriodnt;
                Console.WriteLine($"target time {targetPeriodMs}, full time: {elapsed2:f4}ms, remaining time: {remaining:f4}ms, old lap: {oldlap:f4}ms");
            }
            
        }
    }
}
