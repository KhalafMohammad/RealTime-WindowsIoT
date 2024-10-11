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
        public static StringBuilder dataBuffer = new StringBuilder(); // create een nieuwe stringbuilder object
        public static int position; // Current stepper position
        public static int prev_position; //previous


        public static void Data_to_read(ref SerialPort sp)
        {

            sp.ReadTimeout = 1000; // set the read timeout to 1 second
            while (true)
            {

                Console.WriteLine("Enter steps?");
                int value = Convert.ToInt32(Console.ReadLine());
                if (value == 2)
                {
                    Console.Clear();
                }
                else
                {
                    Scurve.Phase_one(ref sp, value);
                    Scurve.Phase_two(ref sp, value);
                    Scurve.Phase_three(ref sp, value);
                }

                byte[] buffer = new byte[sp.BytesToRead]; // create a buffer to store the data
                sp.Read(buffer, 0, buffer.Length);

                if (buffer.Length > 0 ) // check if the buffer is greater than 8 bytes
                {
                    //string decode_string = Encoding.ASCII.GetString(buffer); // convert the buffer to string
                    //Console.WriteLine("string Received:" + decode_string);
                    dataBuffer.Append(Encoding.UTF8.GetString(buffer)); // append the data to the buffer
                    ProcessData(); // process the data
                }
            }

        }

        private static void ProcessData()
        {
            string data = dataBuffer.ToString(); // get the data from the buffer
            string[] dataParts = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); // get data for the recieve buffer and split it into parts

            foreach (string part in dataParts) // loop door de data parts
            {
                if (int.TryParse(part, out int result)) //  CHECK als de data een integer is
                {
                    position = result; // update de positie
                    Scurve.positie = position;
                    Console.WriteLine(temp_Write.GetTimestamp() + " Integer Received: >>> " + result);
                    if (result == 12)
                    {
                        Console.WriteLine("I GOT 3200 I GOT 3200 I GOT 3200 I GOT 3200 I GOT 3200 ");
                    }
                    //Write.recieve_flag = true;
                }
                else
                {
                    // Console.WriteLine("Invalid data received: " + part);
                }
            }

            // Clear the buffer after processing
            dataBuffer.Clear();
        }

    }
}
