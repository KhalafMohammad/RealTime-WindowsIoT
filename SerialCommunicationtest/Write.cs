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
        public static void Data_to_write(ref SerialPort sp)
        {

            // setup write timeout
            // because we are writing to the UART it's recommended to set a write timeout
            // otherwise the write operation doesn't return until the requested number of bytes has been written
            sp.WriteTimeout = 500;

            while (true)
            {
                //if (recieve_flag)
                //{
                //recieve_flag = false;
                Console.WriteLine("Enter steps?");
                int value = Convert.ToInt32(Console.ReadLine());
                Scurve.Phase_one(ref sp, value);
                Scurve.Phase_two(ref sp, value);
                Scurve.Phase_three(ref sp, value);

                //################################################################################
                //##  This is the code to write to the serial port. It is commented out because it is not needed in this example
                //##  The code is left here for reference
                //################################################################################
                //byte[] value_bytes = new byte[2];
                //value_bytes[0] = (byte)(0 >> 8); // shift 8 bits to the right
                //value_bytes[1] = (byte)(0 & 0xFF); // bitwise AND with 0xFF

                //sp.Write(value_bytes, 0, 2); // write 1
                //Console.WriteLine(GetTimestamp() + " Wrote " + value + " over" + sp.PortName + ".");
                //#####################################################################################################################
                //}
            }
        }

        public static string GetTimestamp() // get the current time in HH:MM:SS:FFF format to print out milliseconds too 
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }

    }
}