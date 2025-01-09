
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
        private static int error1;
        private static int error2;
        public static int m1_steps;
        public static int m2_steps;
        public static int m1_curr_pos;
        public static int m2_curr_pos;
        public static bool error_flag = false;


        static void Main()
        {
            try
            {
                Serial_Init.serial_init();

                Console.Title = "Robot: ZTIMK-bot prototype";
                // Set the process priority to high and the thread priority to time critical
                RealTime.Process_managment(Process.GetCurrentProcess(), 0xc0, ProcessPriorityClass.RealTime);
                //RealTime.Threads_managment(Process.GetCurrentProcess(), ThreadPriorityLevel.Highest);


                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
                Serial_Init.sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                // Initialize the serial port



                Robot newrobot = new();
                //// initialize the robot

                //while (true)
                //{
                    newrobot.coordinates(13.60, 16); //  13.60, 16
                    newrobot.Run(); //ref Serial_Init._serialport
                    //Thread.Sleep(500);

                    //newrobot.coordinates(-5, 16); //  -5 , 16
                    //newrobot.Run(); //ref Serial_Init._serialport
                    //Thread.Sleep(500);
                //}


                //Serial_Init.sp.Close(); // close the serial port
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

            string[] data_parts = incoming_data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Console.WriteLine("Incoming data: " + data_parts[0] + data_parts[1] + data_parts[2] + data_parts[3]);
            if (data_parts[0] == "m1" && data_parts[2] == "m2")
            {
                Console.WriteLine($"{data_parts[0]} pos: {data_parts[1]}");
                m1_curr_pos = int.Parse(data_parts[1]);
                error1 = m1_steps - m1_curr_pos;

                Console.WriteLine("motor1 Error: " + error1 + " steps: " + m1_steps);


                Console.WriteLine($"{data_parts[2]} pos: {data_parts[3]}");
                m2_curr_pos = int.Parse(data_parts[3]);
                error2 = m2_steps - m2_curr_pos;
                Console.WriteLine("motor2 Error: " + error2 + " steps: " + m2_steps);

                if (error1 == 0 && error2 == 0)
                {
                    Console.WriteLine("Done");
                    error_flag = true;
                    m1_steps = 0;
                    m2_steps = 0;
                    m1_curr_pos = 0;
                    m2_curr_pos = 0;
                    return;
                }
                else
                {
                    error_flag = false;
                    // Send the error to the robot
                    char m1_dir = error1 > 0 ? 'R' : 'L';
                    char m2_dir = error2 > 0 ? 'R' : 'L';
                    error2 = Math.Abs(error2);
                    error1 = Math.Abs(error1);
                    // Send the stop command to the robot
                    byte[] stop_array = PacketList.combine_stop_error((ushort)error1, m1_dir, (ushort)error2, m2_dir);
                    Serial_Init.sp.Write(stop_array, 0, 7);
                    Console.WriteLine("error command sent");
                    stop_array = null;
                    error1 = 0;
                    error2 = 0;
                    m1_dir = ' ';
                    m2_dir = ' ';
                    //m1_steps = 0;
                    //m2_steps = 0;
                    m1_curr_pos = 0;
                    m2_curr_pos = 0;
                }
            }
            else
            {
                Console.WriteLine("positie na error: " + incoming_data);
            }

            // Clear the buffer after processing
            dataBuffer.Clear();
        }


    }
}