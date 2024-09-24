using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    //read data from the incoming serial port
    internal class Read
    {
        public static void Data_to_read(ref SerialPort sp)
        {

            sp.ReadTimeout = 1000; // set the read timeout to 1 second
            while (true)
            {
                byte[] buffer = new byte[sp.BytesToRead];
                sp.Read(buffer, 0,buffer.Length);

                //byte p = (byte)sp.ReadByte(); // read one byte
               // Console.WriteLine("Data Received:" + p.ToString("X2"));

                //if (p == 0x0A) //
                //{
                //    Debug.WriteLine("LED ON"); 
                //}
                //else { Debug.WriteLine("LED OFF"); }

                //if (buffer.Length >= 4) // check if the buffer is greater than 4 bytes 
                //{
                //    long decode_data = BitConverter.ToInt16(buffer, 0); // convert the buffer to integer
                //    Console.WriteLine("Data Received:" + decode_data); 
                //}else
                 if (buffer.Length >= 8) // check if the buffer is greater than 8 bytes
                {
                    string decode_string = Encoding.ASCII.GetString(buffer); // convert the buffer to string
                    Console.WriteLine("string Received:" + decode_string);
                }
                //else
                //{ 
                //    foreach (byte b in buffer) // loop through the buffer [old meythod :)]
                //    {
                //        Console.WriteLine("Data buffer is: 0x" + b.ToString("X2")); // print the buffer in hex
                //    }

                //}
            }

        }

    }
}
