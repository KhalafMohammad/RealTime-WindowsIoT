// Purpose: Contains the Scurve class which is responsible for the S-curve motion profile.
// The S-curve motion profile is a motion profile that is used to move a motor from a standstill to a desired speed and back to a standstill.
// used to prevent the motor from jerking and to ensure a smooth motion profile.
// divided into three phases: acceleration, constant speed, and deceleration.
// implemented using the following formula:
// curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));


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
        public static float dt = 0.001F;
        public static int positie;

        public static void Phase_one(ref SerialPort sp, int accelertion)
        {
            acc_max = positie;
            max_freq = accelertion;
            //j_max = acc_max / accelertion; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //j_max = accelertion / acc_max; ; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //t_j = (float)((33.3 * j_max) / 100);

            for (float t = 0.001F; t < t_j; t += dt)
            { 
                // t_j = (float)((33.3 * j_max) / 100);
                curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2))); // S-curve formula
                byte[] value_bytes = new byte[2];
                value_bytes[0] = (byte)((int)curr_freq >> 8); // shift 8 bits to the right
                value_bytes[1] = (byte)((int)curr_freq & 0xFF); // bitwise AND with 0xFF

                sp.Write(value_bytes, 0, 2); // write 1
                //Console.WriteLine(Write.GetTimestamp() + " Wrote " + curr_freq + " over" + sp.PortName + ".");

            }
            Console.WriteLine("Max Frequency: " + curr_freq);
        }
        public static void Phase_two(ref SerialPort sp, int accelertion)
        {

            max_freq = accelertion;
            //j_max = acc_max / accelertion; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //j_max = accelertion / acc_max; ; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //t_j = (float)((33.3 * j_max) / 100);
            //int distance = (j_max/2) * accelertion;

            for (float t = t_j; t < (j_max - t_j); t += dt)
            {

                curr_freq = max_freq;
                //Console.WriteLine("wTime: " + t + " Frequency: " + curr_freq);
                byte[] value_bytes = new byte[2];
                value_bytes[0] = (byte)((int)curr_freq >> 8); // shift 8 bits to the right
                value_bytes[1] = (byte)((int)curr_freq & 0xFF); // bitwise AND with 0xFF
                sp.Write(value_bytes, 0, 2); // write 1
            }
            Console.WriteLine("Max Frequency: " + curr_freq);
        }

        //note to self: impliment feeback posistion here 
        public static void Phase_three(ref SerialPort sp, int accelertion)
        {
            max_freq = accelertion;
            //j_max = acc_max / accelertion; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //j_max = accelertion / acc_max; ; //acceleration = snelheid/tijd ##  tijd = snelheid/acceleratie
            //t_j = (float)((33.3 * j_max) / 100);
            for (float t = t_j; t > 0.001F; t -= dt)
            {
                curr_freq = Math.Round(max_freq * (1 - (float)Math.Pow((1 - t / t_j), 2)));
                //Console.WriteLine("ETime: " + t + " Frequency: " + curr_freq);
                byte[] value_bytes = new byte[2];
                value_bytes[0] = (byte)((int)curr_freq >> 8); // shift 8 bits to the right
                value_bytes[1] = (byte)((int)curr_freq & 0xFF); // bitwise AND with 0xFF
                sp.Write(value_bytes, 0, 2); // write 1
                //Console.WriteLine(Write.GetTimestamp() + " Wrote " + curr_freq + " over" + sp.PortName + ".");
            }
            //Read.Data_to_read(ref sp);
            int final_freq = 0;
            Write.data(ref sp, final_freq);
            Console.WriteLine("Max Frequency: " + max_freq);
        }

    }
}