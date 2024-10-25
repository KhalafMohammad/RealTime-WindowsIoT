// Purpose: Contains the Scurve class which is responsible for the S-curve motion profile.
// The S-curve motion profile is a motion profile that is used to move a motor from a standstill to a desired speed and back to a standstill.
// used to prevent the motor from jerking and to ensure a smooth motion profile.
// divided into three phases: acceleration, constant speed, and deceleration.
// implemented using the following formula:
// curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));
// the speed is the same as the the target steps e.g 3200 maximum speed is 3200


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
        public static int dir;
        const double targetPeriodMs = 1.0;


        public static void Phase_one(ref SerialPort sp, int accelertion)
        {

            int acc_b = Math.Abs(accelertion); // absolute value of the acceleration

            if (accelertion < 0) // ditermin the direction of the motor
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }

            var watch = new System.Diagnostics.Stopwatch(); // start stopwatch

            max_freq = (int)Math.Round((double)acc_b / 3);
            first_portion = (int)Math.Round((double)target / 3);


            for (float t = 0; t < 0.45; t += dt)
            {
                watch.Restart();
                curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / 0.5), 2))); // S-curve formula
                if (dir == -1)
                {
                    curr_freq = -curr_freq;
                    Write.data(ref sp, (int)curr_freq);
                }
                else
                {
                    Write.data(ref sp, (int)curr_freq);
                }

                watch.Stop();
                int elapsed = (int)watch.ElapsedMilliseconds;
                if (elapsed < 1)
                {
                    Thread.Sleep(1 - elapsed);
                }
            }
            watch.Stop();
            flag = true;
            Phase_two(ref sp, accelertion); // call the next phase (Phase_two)
        }
        public static void Phase_two(ref SerialPort sp, int accelertion)
        {
            var watch = new System.Diagnostics.Stopwatch();


            int acc_b = Math.Abs(accelertion); // absolute value of the acceleration

            if (accelertion < 0)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }
            // cheange this to 0.45 or 0.5
            for (float t = 0.5F; t < (j_max - 0.5F); t += dt)
            {
                watch.Restart();
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
                


                watch.Stop();

                int elapsed = (int)watch.ElapsedMilliseconds;
                if (elapsed < 1)
                {
                    Thread.Sleep(1 - elapsed);
                }
            }
            Phase_three(ref sp, accelertion);
        }

        public static void Phase_three(ref SerialPort sp, int accelertion)
        {
            var watch = new System.Diagnostics.Stopwatch();
            int acc_b = Math.Abs(accelertion); // absolute value of the acceleration

            if (accelertion < 0)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }

            max_freq = (int)Math.Round((double)acc_b / 3);


            for (float t = t_j; t > 0; t -= dt)
            {
                watch.Restart();
                if (flag == false)
                {
                    Write.data(ref sp, 0);
                    break;
                }
                //else
                //{
                //    t_j += 0.05F;

                //}

                curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));
                if (dir == -1)
                {
                    curr_freq = -curr_freq;
                    Write.data(ref sp, (int)curr_freq);
                }
                else
                {
                    Write.data(ref sp, (int)curr_freq);
                }
                
                watch.Stop();
                int elapsed = (int)watch.ElapsedMilliseconds;
                if (elapsed < 1)
                {
                    Thread.Sleep(1 - elapsed);
                }

            }
            if(flag == false)
            {
                Write.data(ref sp, 0);
            }

            int final_freq = 0;
            Write.data(ref sp, final_freq);
            flag = true;
        }
    }
}