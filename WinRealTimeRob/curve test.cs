using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    internal class curve_test
    {
        private static double curr_freq;

        private static double acc_b = 2000;
        private static double t_j = 0.208f;
        private static double dt = 0.001f;


        public static void scurve_test()
        {
            //for (double t = 0.000f; t < t_j; t += dt)
            //{

            //    curr_freq = Math.Round(acc_b * (1 - (float)Math.Pow((1 - t / t_j), 2)));
            //    Console.WriteLine($"({t:f3},{curr_freq})");
            //    Thread.Sleep(1);
            //}
            for (double t = t_j; t < t_j * 2; t += dt)
            {

                curr_freq = acc_b;
                Console.WriteLine($"({t:f3},{curr_freq})");
                Thread.Sleep(1);
            }
            double i = t_j * 2;
            for (double t = t_j; t > 0.000f; t -= dt)
            {
                
                i += dt;
                curr_freq = Math.Round(acc_b * (1 - (float)Math.Pow((1 - t / t_j), 2)));
                Console.WriteLine($"({i:f3},{curr_freq})");
                Thread.Sleep(1);
            }

        }
    }
}
