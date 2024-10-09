
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
        public static int position; // Current stepper position
        public static int prev_position; //previous
        static void Main(string[] args)
        {
            // create a new SerialPort object with default settings and 1khz
            Serial_Init Serial_Init = new Serial_Init();
            // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
            Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            Serial_Init.serial_init();

            // read from serial port [UNCOMMENT TO USE]
            //Read.Data_to_read(ref _serialport);


            //write to serial port [UNCOMMENT TO USE]            
            Write.Data_to_write(ref Serial_Init._serialport);

            //Scurve.Phase_one(1000);
            //Scurve.Phase_two(1000);
            //Scurve.Phase_three(1000);


            //Thread.Sleep(Timeout.Infinite);
        }
        // private static byte[] buffer = new byte[_serialport.BytesToRead];
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            //string indata = sp.ReadExisting();
            try
            {
                byte[] buffer = new byte[sp.BytesToRead];
                //Console.WriteLine("Data Received:" + indata);
                sp.Read(buffer, 0, sp.BytesToRead);
                if (buffer.Length > 0)
                {
                    var decode_string = Encoding.UTF8.GetString(buffer);
                    position = Int16.Parse(decode_string);
                    Console.Write(temp_Write.GetTimestamp() + " string Received: >>> " + decode_string);
                    Console.WriteLine("previous position: " + prev_position + "current position: " + position);
                    Write.recieve_flag = true;
                }
                prev_position = position; // update the previous position
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            //if (buffer.Length >= 8)
            //{
            //    long decode_data = BitConverter.ToInt16(buffer, 0);
            //    Console.WriteLine("Data Received:" + decode_data); // works but commented
            //}
            //################################################################
            //                          read the 1st byte
            //################################################################
            //byte p = (byte)sp.ReadByte(); // read one byte
            //if (p == 0x0A)
            //{
            //    Console.WriteLine("LED ON");
            //}
            //else { Console.WriteLine("LED OFF"); }
            //Console.WriteLine("Data Received:" + p.ToString("X2"))
            //if (buffer.Length > 0)
            //{
            //    foreach (byte b in buffer)
            //    {
            //        Console.WriteLine("Data buffer is: 0x" + b.ToString("X2"));
            //    }
            //}
        }
    }
}