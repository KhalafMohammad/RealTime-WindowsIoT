// Purpose: Contains the Scurve class which is responsible for the S-curve motion profile.
// The S-curve motion profile is a motion profile that is used to move a motor from a standstill to a desired speed and back to a standstill.
// used to prevent the motor from jerking and to ensure a smooth motion profile.
// divided into three phases: acceleration, constant speed, and deceleration.
// implemented using the following formula:
// curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));
// the speed is the same as the the target steps e.g 3200 maximum speed is 3200

//#define kernel32
#define stopwatch
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;



namespace WinSerialCommunication
{

    internal class Scurve2
    {

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public static double curr_freq = 0;
        public static float j_max = 1.5F;
        public static float t_j = 0.500F;
        public static float dt = 0.001F;
        public static int target = 1000; // target position
        public static int positie; // incoming position from the serial port
        public static bool flag = true;
        public static int dir;
        const double targetPeriodMs = 1.0;
#if stopwatch
        public static Stopwatch watch = new Stopwatch(); // start stopwatch
#endif


        public static void Phase_one(ref SerialPort sp, int accelertion)
        {

            RealTime.manage_thread(Process.GetCurrentProcess(), Thread.CurrentThread.ManagedThreadId, ThreadPriorityLevel.TimeCritical); // temp for testing purposes thread priority
            QueryPerformanceFrequency(out long frequency);


            int acc_b = Math.Abs(accelertion); // absolute value of the acceleration

            if (accelertion < 0) // ditermin the direction of the motor
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }



            //var watch2 = new Stopwatch(); // start stopwatch
            //watch2.Start();
            QueryPerformanceCounter(out long start1);


            for (float t = 0.001f; t < t_j; t += dt)
            {
#if kernel32
                QueryPerformanceCounter(out long start);
#elif stopwatch
                watch.Restart();

#endif

                curr_freq = Math.Round(acc_b * (1 - (float)Math.Pow((1 - t / t_j), 2))); // S-curve formula
                if (dir == -1)
                {
                    curr_freq = -curr_freq;
                    Write.data(ref sp, (int)curr_freq);
                }
                else
                {
                    Write.data(ref sp, (int)curr_freq);
                }
#if kernel32
                QueryPerformanceCounter(out long stop);
                double elapsed = (stop - start) * 1000.0 / frequency;
                double remaining = targetPeriodMs - elapsed;
                double oldlap = elapsed;
                if (remaining > 0)
                {
                    while((stop - start) * 1000.0 / frequency < targetPeriodMs)
                    {
                        QueryPerformanceCounter(out stop);
                        Thread.SpinWait(1);
                    }
                    QueryPerformanceCounter(out long stop2);
                    double elapsed2 = (stop2 - start) * 1000.0 / frequency;
                    Console.WriteLine($"Iteration {curr_freq:f2}, target time {targetPeriodMs}, full time: {elapsed2:f3}ms, remaining time: {remaining:f4}ms, old lap: {oldlap:f4}ms");

                }
#elif stopwatch
                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

                Time_delay(executionTimeMs);
#endif
            }
            //watch2.Stop();
            //Console.WriteLine($"Total time: {watch2.ElapsedMilliseconds:f5}ms");
            QueryPerformanceCounter(out long stop1);
            double elapsed1 = (stop1 - start1) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");

            flag = true;
            Phase_two(ref sp, accelertion); // call the next phase (Phase_two)
        }
        public static void Phase_two(ref SerialPort sp, int acceleration)
        {

            QueryPerformanceFrequency(out long frequency);
            int acc_b = Math.Abs(acceleration); // absolute value of the acceleration

            if (acceleration < 0)
            {
                dir = -1;
            }
            else
            {
                dir = 1;

            }
            QueryPerformanceCounter(out long start1);

            // cheange this to 0.45 or 0.5
            for (float t = t_j; t < 1.000; t += dt)
            {
#if kernel32
                QueryPerformanceCounter(out long start);
#elif stopwatch
                watch.Restart();

#endif  
                if (flag == false)
                {
                    break;
                }
                curr_freq = acc_b;

                if (dir == -1)
                {
                    curr_freq = -curr_freq;
                    Write.data(ref sp, (int)curr_freq);
                }
                else
                {
                    Write.data(ref sp, (int)curr_freq);
                }
#if kernel32
                QueryPerformanceCounter(out long stop);
                double elapsed = (stop - start) * 1000.0 / frequency;
                double remaining = targetPeriodMs - elapsed;
                if(remaining > 0)
                {
                    while((stop - start) * 1000.0 / frequency < targetPeriodMs)
                    {
                        QueryPerformanceCounter(out stop);
                        Thread.SpinWait(1);

                    }
                }
#elif stopwatch
                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

                Time_delay(executionTimeMs);
#endif
            }
            QueryPerformanceCounter(out long stop1);
            double elapsed1 = (stop1 - start1) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");
            if (flag == true)
            {
                Phase_three(ref sp, acceleration);
            }

        }

        public static void Phase_three(ref SerialPort sp, int accelertion)
        {
            QueryPerformanceFrequency(out long frequency);
            int acc_b = Math.Abs(accelertion); // absolute value of the acceleration

            if (accelertion < 0)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }


            QueryPerformanceCounter(out long start1);

            for (float t = t_j; t > 0; t -= dt)
            {
#if kernel32
                QueryPerformanceCounter(out long start);
#elif stopwatch
                watch.Restart();

#endif
                if (flag == false)
                {
                    Write.data(ref sp, 0);
                    break;
                }


                curr_freq = Math.Round(acc_b * (1 - (float)Math.Pow((1 - t / t_j), 2)));
                if (dir == -1)
                {
                    curr_freq = -curr_freq;
                    Write.data(ref sp, (int)curr_freq);
                }
                else
                {
                    Write.data(ref sp, (int)curr_freq);
                }
#if kernel32
                QueryPerformanceCounter(out long stop);
                double elapsed = (stop - start) * 1000.0 / frequency;
                double remaining = targetPeriodMs - elapsed;
                if(remaining > 0)
                {
                    while((stop - start) * 1000.0 / frequency < targetPeriodMs)
                    {
                        QueryPerformanceCounter(out stop);
                        Thread.SpinWait(1);

                    }
                }
#elif stopwatch
                double executionTimeMs = watch.Elapsed.TotalMilliseconds;

                Time_delay(executionTimeMs);
#endif
            }
            QueryPerformanceCounter(out long stop1);
            double elapsed1 = (stop1 - start1) * 1000.0 / frequency;
            Console.WriteLine($"Total time: {elapsed1:f5}ms");
            if (flag == false)
            {
                Write.data(ref sp, 0);
            }

            //int final_freq = 0;
            //Write.data(ref sp, final_freq);
            flag = true;
        }


        private static void Time_delay(double executionTimeMs)
        {
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


