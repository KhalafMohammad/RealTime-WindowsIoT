﻿
//THIS DUCOMENT WAS MADE USING https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport?view=net-8.0 API DOCUMENTATION
//IT ENTRFACES WITH THE SERIAL PORTS ON THE COMPUTER AND READS DATA FROM THE SERIAL PORT FROM THE ESP32
//THE DATA IS THEN DECODED AND PRINTED TO THE CONSOLE
// [PROTOTYPE]
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Text;


namespace WinSerialCommunication
{

    internal class Program
    {
        private static StringBuilder dataBuffer = new StringBuilder(); // create een nieuwe stringbuilder object

        public static int position; // Current stepper position
        public static int prev_position; //previous
        static void Main()
        {
            try
            {
                // create a new SerialPort object with default settings and 1khz
                Serial_Init Serial_Init = new Serial_Init();

                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
                Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                Serial_Init.serial_init();

                // read from serial port [UNCOMMENT TO USE]
                //Read.Data_to_read(ref Serial_Init._serialport);


                //write to serial port [UNCOMMENT TO USE]            
                Write.Data_to_write(ref Serial_Init._serialport);


                Write.data(ref Serial_Init._serialport, 0);
                Serial_Init._serialport.Close(); // close the serial port
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead]; // create a buffer to store the data
            sp.Read(buffer, 0, sp.BytesToRead); // read the data from the serial port

            if (buffer.Length > 0) // check if the buffer has data
            {

                dataBuffer.Append(Encoding.UTF8.GetString(buffer));

                // Process de data in de buffer
                ProcessData();
            }
            prev_position = position; // update the previous position
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

                }

            }
            // Clear the buffer after processing
            dataBuffer.Clear();
        }
    }
}