// Purpose: Contains the Scurve class which is responsible for the S-curve motion profile.
// The S-curve motion profile is a motion profile that is used to move a motor from a standstill to a desired speed and back to a standstill.
// used to prevent the motor from jerking and to ensure a smooth motion profile.
// divided into three phases: acceleration, constant speed, and deceleration.
// implemented using the following formula:
// curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));


using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    internal class Scurve
    {
        public static float max_freq;
        public static double curr_freq = 0;
        public static float acc_max = 2000;
        public static float j_max = 1.5F;
        public static float t_j = 0.5F;
        public static float dt = 0.01F;
        public static int target = 1000; // target position
        public static int positie; // incoming position from the serial port
        public static int first_portion; // 1/3rd of the target
        public static int second_portion = first_portion * 2; // 2/3rds of the target
        public static bool flag = true;


        public static void Phase_one(ref SerialPort sp, int accelertion)
        {
            var watch = new System.Diagnostics.Stopwatch();

            //max_freq = accelertion;
            max_freq = (int)Math.Round((double)accelertion / 3);
            //j_max = acc_max / accelertion; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //j_max = accelertion / acc_max; ; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //t_j = (float)((33.3 * j_max) / 100);
            //int target = (j_max/2) * accelertion;
            first_portion = (int)Math.Round((double)target / 3);
            Console.WriteLine("Max Frequency: " + max_freq);
            Console.WriteLine("target: " + target);
            Console.WriteLine("First Portion: " + first_portion);
            Console.WriteLine("Second Portion: " + second_portion);

            for (float t = 0.001F; t < t_j; t += dt)
            {
                watch.Restart();

                curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2))); // S-curve formula
                Write.data(ref sp, (int)curr_freq);
                watch.Stop();
                //if (flag == false)
                //{
                //    break;
                //}
                int elapsed = (int)watch. ElapsedMilliseconds;
                float elapsed_f = (float)watch.ElapsedMilliseconds;
                Console.WriteLine("Time elapsed: " + elapsed_f);
                if (elapsed < 1)
                {
                    Thread.Sleep(1 - elapsed);
                }
            }
            watch.Stop();
            Console.WriteLine("Time elapsed: " + watch.ElapsedMilliseconds);
            flag = true;
            Console.WriteLine("Max Frequency: " + curr_freq);
        }
        public static void Phase_two(ref SerialPort sp, int accelertion)
        {
            var watch = new System.Diagnostics.Stopwatch();

            max_freq = accelertion;

            //j_max = acc_max / accelertion; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //j_max = accelertion / acc_max; ; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //t_j = (float)((33.3 * j_max) / 100);
            //int target = (j_max/2) * accelertion;
            first_portion = (int)Math.Round((double)target / 3);
            second_portion = first_portion * 2;


            for (float t = t_j; t < (j_max - t_j); t += dt)
            {
                watch.Restart();
                curr_freq = max_freq;


                Write.data(ref sp, (int)curr_freq);
                Console.WriteLine(second_portion);
                watch.Stop();
                //if (flag == false)
                //{
                //    break;
                //}
                int elapsed = (int)watch.ElapsedMilliseconds;
                float elapsed_f = (float)watch.ElapsedMilliseconds;
                Console.WriteLine("Time elapsed: " + elapsed_f);
                if (elapsed < 1)
                {
                    Thread.Sleep(1 - elapsed);
                }
            }
            flag = true;
            Console.WriteLine("Max Frequency: " + curr_freq);
        }

        //note to self: impliment feeback posistion here 
        public static void Phase_three(ref SerialPort sp, int accelertion)
        {
            var watch = new System.Diagnostics.Stopwatch();

            //max_freq = accelertion;
            max_freq = (int)Math.Round((double)accelertion / 3);
            //j_max = acc_max / accelertion; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //j_max = accelertion / acc_max; ; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //t_j = (float)((33.3 * j_max) / 100);
            for (float t = t_j; t > 0.01F; t -= dt)
            {
                watch.Restart();

                curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));

                Write.data(ref sp, (int)curr_freq);
                watch.Stop();
                //if (flag == false)
                //{
                //    break;
                //}
                int elapsed = (int)watch.ElapsedMilliseconds;
                if (elapsed < 1)
                {
                    Thread.Sleep(1 - elapsed);
                }
            }
            int final_freq = 0;
            Write.data(ref sp, final_freq);
            flag = true;
            Console.WriteLine("Max Frequency: " + max_freq);
        }

    }
}