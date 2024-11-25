using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Pipes;


namespace WinSerialCommunication
{
    internal class Write
    {
        public static bool recieve_flag = true;
        public static int value;
        public static int position;
        public static double full_rotation = 3200;
        public static double step_rotation;
        public static void Data_to_write(ref SerialPort sp)
        {
            sp.WriteTimeout = 1000;
            RealTime.Process_managment(Process.GetCurrentProcess(), (IntPtr)0xC0, ProcessPriorityClass.RealTime);
            while (true)
            {

                //Console.Write("Enter Target position? >>>");
                //Scurve.target = Convert.ToInt32(Console.ReadLine());
                Write.data(ref sp, 0); // clear when operatig from stillness
                Console.Write("\nVoer in de steppen? >>>"); // any value is going to be the mount of pulses per second.

                value = Convert.ToInt32(Console.ReadLine());

                
                if (value == 2)
                {
                    Console.Clear();
                }
                else
                {

                    Scurve2.Phase_one(ref sp, (int)value);
                    //Scurve.Phase_two(ref sp, value);
                    //Scurve.Phase_three(ref sp, value);
                    //NewScurve.Sigmoid_curve(ref sp);

                }
            }
        }

        public static string GetTimestamp() // get the current time in HH:MM:SS:FFF format to print out milliseconds too 
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }
        public static void data(ref SerialPort sp, int data)
        {
            byte[] value_bytes = new byte[2];
            value_bytes[0] = (byte)((int)data >> 8); // shift 8 bits to the right
            value_bytes[1] = (byte)((int)data & 0xFF); // bitwise AND with 0xFF
            sp.Write(value_bytes, 0, 2); // write 1
            Console.WriteLine(Write.GetTimestamp() + " Wrote " + data + " over" + sp.PortName + ".");
        }
    }
}