
//THIS DUCOMENT WAS MADE USING https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport?view=net-8.0 API DOCUMENTATION
//IT ENTRFACES WITH THE SERIAL PORTS ON THE COMPUTER AND READS DATA FROM THE SERIAL PORT FROM THE ESP32
//THE DATA IS THEN DECODED AND PRINTED TO THE CONSOLE
// [PROTOTYPE]


using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Security.Cryptography;
using System.Text;


namespace WinSerialCommunication
{
    internal class Program
    {
        private static StringBuilder dataBuffer = new StringBuilder(); // create een nieuwe stringbuilder object


        public static int current_position; //current
        public static int prev_position; //previous
        // Initialize the serial port
        //public static Serial_Init Serial_Init = new Serial_Init();
        public IntPtr aff_mask = (IntPtr)0xC0; // use only the first processor

        static void Main()
        {
            try
            {
                Console.Title = "Robot: ZTIMK-bot prototype";
                // Set the process priority to high and the thread priority to time critical
                RealTime.Process_managment(Process.GetCurrentProcess(), 0xc0, ProcessPriorityClass.RealTime);
                //RealTime.Threads_managment(Process.GetCurrentProcess(), ThreadPriorityLevel.Highest);


                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
                //Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                // Initialize the serial port


                var Read_Thread = new Thread(() =>
                {
                    RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0xc0);

                    //Read.Data_to_read(ref Serial_Init._serialport); // read data from the serial port
                });
                //Read_Thread.Start();

                var Write_Thread = new Thread(() =>
                {
                    RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0xc0);
                    //Write.Data_to_write(ref Serial_Init._serialport); // write data to the serial port
                });

                //Write_Thread.Start();


                //// initialize the robot
                Robot newrobot = new();
                newrobot.coordinates(4.375, 20);
                newrobot.Run(); //ref Serial_Init._serialport




                //TimeingTest timeingTest = new TimeingTest();
                //RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0xc0);
                //timeingTest.Send_Pulse(ref Serial_Init._serialport);

                Thread thread = new Thread(() =>
                {
                    RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0x40);

                    //PacketList packetListener = new PacketList();
                    //packetListener.Test()
                });
                //thread.Start();


                //Serial_Init._serialport.Close(); // close the serial port
                //Serial_Init._serialport.Dispose(); // dispose the serial port
                Thread.Sleep(Timeout.Infinite);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                SerialPort sp = (SerialPort)sender;
                byte[] buffer = new byte[sp.BytesToRead]; // create a buffer to store the data
                sp.Read(buffer, 0, buffer.Length); // read the data from the serial port
                if (buffer.Length > 0) // check if the buffer has data
                {

                    dataBuffer.Append(Encoding.UTF8.GetString(buffer));
                    // Process de data in de buffer
                    ProcessData(ref sp);
                }
                //buffer = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ProcessData(ref SerialPort sp)
        {
            string incoming_data = dataBuffer.ToString(); // get the data from the buffer

            string[] data_parts = incoming_data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in data_parts)
            {
                string[] strings = part.Split(' ');
                foreach (string str in strings)
                {
                    if (int.TryParse(str, out int result))
                    {
                        Console.WriteLine("Integer: " + result);
                    }
                    else
                    {
                        Console.WriteLine("String: " + str);
                    }

                    if (str == "m1")
                    {
                        current_position = result;
                    }
                    else if (str == "m2")
                    {
                        current_position = result;
                    }
                }
            }
            // Clear the buffer after processing
            dataBuffer.Clear();
        }
    }
}