// Purpose: Contains the Scurve class which is responsible for the S-curve motion profile.
// The S-curve motion profile is a motion profile that is used to move a motor from a standstill to a desired speed and back to a standstill.
// used to prevent the motor from jerking and to ensure a smooth motion profile.
// divided into three phases: acceleration, constant speed, and deceleration.
// implemented using the following formula:
// curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));

//#define _kernel_timer
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;



namespace WinSerialCommunication
{

    internal class Scurvev1
    {

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);


        public int accelertion = 2000;
        public double curr_freq = 0;
        public double j_max;
        public double t_j;
        public double dt = 0.001F;
        public int target = 1000; // target position
        public int positie; // incoming position from the serial port
        private int dir;
        const double targetPeriodMs = 1.000;
        const double error = 0.005f;
        public string motor;
        public long frequency;
        private Stopwatch watch = new Stopwatch(); // start stopwatch

        public Scurvev1(double j_max, double j, int target, int positie, string motor)
        {

            this.j_max = j_max;
            t_j = j;
            this.target = target;
            this.positie = positie;
            this.motor = motor;
#if _kernel_timer

            QueryPerformanceFrequency(out long frequency);
#endif

        }

        public void Phase_one(ref SerialPort sp, int steps)
        {


            if (steps < 0) // ditermin the direction of the motor
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }
#if _kernel_timer

            QueryPerformanceCounter(out long start1);
#endif
            for (double t = 0.000f; t <= t_j; t += dt)
            {

                watch.Restart();

                curr_freq = Math.Round(accelertion * (1 - (float)Math.Pow((1 - t / t_j), 2))); // S-curve formula 

                if (dir == -1)
                {
                    sp.Write(motor + curr_freq + " L\n");
                }
                else
                {
                    sp.Write(motor + curr_freq + " R\n");
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
#if _kernel_timer

            QueryPerformanceCounter(out long stop1);
            double elapsed1 = (stop1 - start1) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");
#endif
        }



        public void Phase_two(ref SerialPort sp, int steps)
        {

            if (steps < 0)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }
#if _kernel_timer
            QueryPerformanceCounter(out long start1);
#endif

            for (double t = 0; t <= j_max + error; t += dt)
            {

                watch.Restart();
                curr_freq = accelertion;
                if (dir == -1)
                {
                    sp.Write(motor + curr_freq + " L\n");
                }
                else
                {
                    sp.Write(motor + curr_freq + " R\n");
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
#if _kernel_timer

            QueryPerformanceCounter(out long stop1);
            double elapsed1 = (stop1 - start1) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");
#endif
        }



        public void Phase_three(ref SerialPort sp, int steps)
        {

            if (steps < 0)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }
#if _kernel_timer

            QueryPerformanceCounter(out long start1);
#endif

            for (double t = t_j; t >= 0; t -= dt)
            {

                watch.Restart();

                curr_freq = Math.Round(accelertion * (1 - (float)Math.Pow((1 - t / t_j), 2))); //0.50F
                //Console.WriteLine($"Current frequency: {curr_freq}");

                if (dir == -1)
                {
                    
                    sp.Write(motor + curr_freq + " L\n");
                }
                else
                {
                    sp.Write(motor + curr_freq + " R\n");
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
#if _kernel_timer
            QueryPerformanceCounter(out long stop1);
            double elapsed1 = (stop1 - start1) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");
            
#endif
            sp.Write($"{motor} 0 R\n"); // end the motion profile by writing 0 to the motor
            sp.Write($"{motor} 0 L\n"); // 0 also means that the controller must send the motor position back to the PC

        }
    }
}
