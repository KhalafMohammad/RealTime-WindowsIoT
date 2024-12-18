
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
        public static Serial_Init Serial_Init = new Serial_Init();
        static void Main()
        {
            try
            {
                Console.Title = "Robot: ZTIMK-bot prototype";
                //IntPtr aff_mask = (IntPtr)0xC0; // use only the first processor
                // Set the process priority to high and the thread priority to time critical
                RealTime.Process_managment(Process.GetCurrentProcess(), 0xc0, ProcessPriorityClass.Normal);
                //RealTime.Threads_managment(Process.GetCurrentProcess(), ThreadPriorityLevel.Highest);


                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
                //Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                // Initialize the serial port
                Serial_Init.serial_init();


                var Read_Thread = new Thread(() =>
                {
                    RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0xc0);

                    Read.Data_to_read(ref Serial_Init._serialport); // read data from the serial port
                });
                //Read_Thread.Start();

                var Write_Thread = new Thread(() =>
                {
                    RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0xc0);
                    Write.Data_to_write(ref Serial_Init._serialport); // write data to the serial port
                });

                //Write_Thread.Start();



                //// initialize the robot
                //Robot newrobot = new();
                //newrobot.coordinates(4.375, 20);
                //newrobot.Run(ref Serial_Init._serialport); //ref Serial_Init._serialport




                //TimeingTest timeingTest = new TimeingTest();
                //RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0xc0);
                //timeingTest.Send_Pulse(ref Serial_Init._serialport);


                Tabletset motor1 = new Tabletset(0.208, 0.208, 0, 0, "m1");
                int[] motor1_values = motor1.Move(1250);

                char motor1_dir = 'R';

                Tabletset motor2 = new Tabletset(0.104, 0.104, 0, 0, "m1");
                int[] motor2_values = motor2.Move(1250);


                char motor2_dir = 'L';


                //for (int i = 0; i < motor1_values.Length; i++)
                //{
                //    Console.WriteLine($"Motor 1: {motor1_values[i]} Direction: {motor1_dir}");

                //}
                //for (int i = 0; i < motor2_values.Length; i++)
                //{
                //    Console.WriteLine($"Motor 2: {motor2_values[i]} Direction: {motor2_dir}");
                //}


                int max_itr = Math.Max(motor1_values.Length, motor2_values.Length);
                Console.WriteLine("Motor max itrs: " + max_itr);

                List<byte> combined_values = new List<byte>();

                for (int i = 0; i < max_itr; i++)
                {

                    // get the values from the motor1 arrays
                    ushort motor1_itr = (ushort)(i < motor1_values.Length ? motor1_values[i] : 0);
                    char motor1_direction = i < motor1_values.Length ? motor1_dir : 'N';

                    // get the values from the motor2 arrays
                    ushort motor2_itr = (ushort)(i < motor2_values.Length ? motor2_values[i] : 0);
                    char motor2_direction = i < motor2_values.Length ? motor2_dir : 'N';

                    // combine the values
                    byte[] m1_value_bytes = new byte[2];
                    m1_value_bytes[0] = (byte)((int)motor1_itr >> 8); // shift 8 bits to the right
                    m1_value_bytes[1] = (byte)((int)motor1_itr & 0xFF); // bitwise AND with 0xFF


                    byte[] m2_value_bytes = new byte[2];
                    m2_value_bytes[0] = (byte)((int)motor2_itr >> 8); // shift 8 bits to the right
                    m2_value_bytes[1] = (byte)((int)motor2_itr & 0xFF); // bitwise AND with 0xFF


                    combined_values.AddRange(m1_value_bytes);
                    combined_values.Add((byte)motor1_direction);

                    combined_values.AddRange(m2_value_bytes);
                    combined_values.Add((byte)motor2_direction);

                    //combined_values.Add((byte)carraege_return);
                }

                byte[] combined_values_array = combined_values.ToArray();

                int m1_value_bytes1 = 0;
                int m2_value_bytes1 = 0;
                for (int i = 0; i < max_itr * 6; i += 6)
                {

                    m1_value_bytes1 = (combined_values_array[i + 0] << 8); // shift 8 bits to the right
                    m1_value_bytes1 |= (combined_values_array[i + 1] & 0xFF); // bitwise AND with 0xFF

                     //m1_value_bytes1 = (combined_values_array[i + 0] << 8) | combined_values_array[i + 1]; // same code to decode the bytes must be used for both motors in ESP32 code.


                    m2_value_bytes1 = (combined_values_array[i + 3] << 8); // shift 8 bits to the right
                    m2_value_bytes1 |= (combined_values_array[i + 4] & 0xFF); // bitwise AND with 0xFF

                    //m2_value_bytes1 = (combined_values_array[i + 3] << 8) | combined_values_array[i + 4];



                    char motor1_directions = (char)combined_values_array[i + 2];

                    char motor2_directions = (char)combined_values_array[i + 5];

                    Console.WriteLine($"[{m1_value_bytes1}, {motor1_directions}] , [{m2_value_bytes1}, {motor2_directions}]");
                    Thread.Sleep(1);
                }

                //Serial_Init._serialport.Close(); // close the serial port
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