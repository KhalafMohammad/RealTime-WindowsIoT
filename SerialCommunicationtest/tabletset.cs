using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinSerialCommunication
{
    internal class Tabletset
    {
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
        public Tabletset(double j_max, double j, int target, int positie, string motor)
        {
            this.j_max = j_max;
            t_j = j;
            this.target = target;
            this.positie = positie;
            this.motor = motor;
        }

        public int[] Move(int steps)
        {
            double max = (double)(t_j * 3) * 1000;
            Console.WriteLine("Max: " + max);
            int itr = 0;
            int[] motor1 = new int[(int)max];
            if (steps < 0) // ditermin the direction of the motor
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }
            #region Phase 1
#if _kernel_timer

            QueryPerformanceCounter(out long start1);
#endif
            for (double t = 0.000f; t < t_j; t += dt)
            {

                curr_freq = Math.Round(accelertion * (1 - (float)Math.Pow((1 - t / t_j), 2))); // S-curve formula 

                if (dir == -1)
                {
                    curr_freq = -curr_freq;

                    //sp.Write(motor + curr_freq + " L\n");
                }
                else
                {
                    //sp.Write(motor + curr_freq + " R\n");
                }
                motor1[itr] = (int)curr_freq;
                itr++;

            }

            #endregion Phase 1

            #region Phase 2
            for (double t = 0; t < j_max; t += dt)
            {
                curr_freq = accelertion;
                if (dir == -1)
                {
                    curr_freq = -curr_freq;

                    //sp.Write(motor + curr_freq + " L\n");
                }
                else
                {
                    //sp.Write(motor + curr_freq + " R\n");
                }
                motor1[itr] = (int)curr_freq;
                itr++;


            }

            #endregion Phase 2

            #region Phase 3



            for (double t = t_j; t > 0; t -= dt)
            {


                curr_freq = Math.Round(accelertion * (1 - (float)Math.Pow((1 - t / t_j), 2))); //0.50F

                if (dir == -1)
                {
                    curr_freq = -curr_freq;

                    //sp.Write(motor + curr_freq + " L\n");
                }
                else
                {
                    //sp.Write(motor + curr_freq + " R\n");
                }
                motor1[itr] = (int)curr_freq;
                itr++;


            }
            return motor1;

            #endregion Phase 3

        }
    }
}
