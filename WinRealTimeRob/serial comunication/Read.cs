﻿using System;
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
        public static string data;
        public static string[] dataParts; //current


        public static void Data_to_read(ref SerialPort sp)
        {

            byte[] buffer;
            sp.ReadTimeout = 1000; // set the read timeout to 1 second
            while (true)
            {

                buffer = new byte[sp.BytesToRead]; // create a buffer to store the data
                sp.Read(buffer, 0, buffer.Length);

                if (buffer.Length > 0) // check if the buffer is greater than 8 bytes
                {
                    //string decode_string = Encoding.ASCII.GetString(buffer); // convert the buffer to string
                    //Console.WriteLine("string Received:" + decode_string);
                    dataBuffer.Append(Encoding.UTF8.GetString(buffer)); // append the data to the buffer
                    ProcessData(); // process the data
                }
                buffer = null; // clear the buffer

            }
        }

        

        private static void ProcessData()
        {
            data = dataBuffer.ToString(); // get the data from the buffer
            dataParts = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); // get data for the recieve buffer and split it into parts

            foreach (string part in dataParts) // loop door de data parts
            {
                if (int.TryParse(part, out int result)) //  CHECK als de data een integer is
                {
                    position = result; // update de positie
                    Console.WriteLine(temp_Write.GetTimestamp() + " Integer Received: >>> " + result);
                }
                else
                {
                    // Console.WriteLine("Invalid data received: " + part);
                }
            }

            // Clear the buffer after processing
            dataParts = null;
            dataBuffer.Clear();

        }

    }
}
