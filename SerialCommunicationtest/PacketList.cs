using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace WinSerialCommunication
{
    internal class PacketList
    {
        private const char packet_end = ';';
        private int max_itr;
        private List<byte> combined_values = [];
        private ushort motor1_itr;
        private ushort motor2_itr;
        private char motor1_direction;
        private char motor2_direction;
        private byte[] m1_value_bytes = new byte[2];
        private byte[] m2_value_bytes = new byte[2];
        private byte[] combined_values_array;
        private int m1_value_int;
        private int m2_value_int;
        private int curr_packet_size = 7;
        const double targetPeriodMs = 1.000;
        public Serial_Init Serial_Init = new Serial_Init();
        private static StringBuilder dataBuffer = new StringBuilder(); // create een nieuwe stringbuilder object



        public void Test(int[] motor1_values, char motor1_dir, int[] motor2_values, char motor2_dir)
        {
            Serial_Init.sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            Serial_Init.serial_init(); // initialize the serial port

            combined_values_array = Combine_values(motor1_values, motor1_dir, motor2_values, motor2_dir).ToArray();


            Stopwatch sw = new Stopwatch();
            //send every 7 bytes
            while (true)
            {
                Thread thread = new Thread(() =>
                {
                    RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0x80);

                    Transmit(combined_values_array);
                });
                thread.Start();
                Thread.Sleep(1000);
            }

            //Print_values(combined_values_array, "D");


            Serial_Init.sp.Close(); // close the serial port
            Serial_Init.sp.Dispose(); // dispose the serial port
        }

        /// <summary>
        /// Combine the values of the two motors
        /// <example>
        /// For example:
        /// <code>
        /// byte[] array = Combine_values(motor1_values, motor1_dir, motor2_values, motor2_dir).ToArray();
        /// </code>
        /// </example>
        /// <param name="motor1_values"></param>
        /// <param name="motor1_dir"></param>
        /// <param name="motor2_values"></param>
        /// <param name="motor2_dir"></param>
        /// <returns></returns>
        /// </summary>
        private List<byte> Combine_values(int[] motor1_values, char motor1_dir, int[] motor2_values, char motor2_dir)
        {
            max_itr = Math.Max(motor1_values.Length, motor2_values.Length); // choose the max length of the two arrays
            Console.WriteLine("Motor max itrs: " + max_itr);

            for (int i = 0; i < max_itr; i++)
            {

                // get the values from the motor1 arrays
                motor1_itr = (ushort)(i < motor1_values.Length ? motor1_values[i] : 0);
                motor1_direction = i < motor1_values.Length ? motor1_dir : 'N';

                // get the values from the motor2 arrays
                motor2_itr = (ushort)(i < motor2_values.Length ? motor2_values[i] : 0);
                motor2_direction = i < motor2_values.Length ? motor2_dir : 'N';

                // combine the values
                m1_value_bytes[0] = (byte)((int)motor1_itr >> 8); // shift 8 bits to the right
                m1_value_bytes[1] = (byte)((int)motor1_itr & 0xFF); // bitwise AND with 0xFF


                m2_value_bytes[0] = (byte)((int)motor2_itr >> 8); // shift 8 bits to the right
                m2_value_bytes[1] = (byte)((int)motor2_itr & 0xFF); // bitwise AND with 0xFF

                // add the values to the combined list 
                combined_values.AddRange(m1_value_bytes);
                combined_values.Add((byte)motor1_direction);

                combined_values.AddRange(m2_value_bytes);
                combined_values.Add((byte)motor2_direction);

                combined_values.Add((byte)packet_end);
            }
            return combined_values;
        }

        /// <summary>
        /// Calculate the remaining time to reach 1ms period
        ///<example>
        /// For example:
        /// <code>
        /// Stopwatch sw = new Stopwatch();
        /// sw.restart();
        /// ... packet here
        /// sw.stop();
        /// Wait_Time(sw.elapsed.totalmilliseconds);
        /// </code>
        /// </example>
        /// <param name="executionTimeMs"></param>
        /// <returns></returns>
        /// </summary>
        private double Wait_Time(double executionTimeMs)
        {
            // Calculate remaining time to reach 1ms period
            double remainingTimeMs = targetPeriodMs - executionTimeMs;

            if (remainingTimeMs > 0)
            {
                // Precise waiting for the remaining time
                var waitTimer = new Stopwatch();
                waitTimer.Start();
                while (waitTimer.Elapsed.TotalMilliseconds < remainingTimeMs)
                {
                    Thread.SpinWait(1);
                }
                return waitTimer.Elapsed.TotalMilliseconds;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>Print the values of the motors:
        /// <example>
        /// For example:
        /// <code>
        /// Print_vals(array, "D"); //for decimal values
        /// Print_vals(array, "X"); //for hexadecimal values
        /// </code>
        /// </example>
        /// <param name="combined_values_array"></param>
        /// <param name="vals"></param>
        /// </summary>
        private void Print_values(byte[] combined_values_array, string vals)
        {
            if (vals.Equals("D"))
            {
                for (int i = 0; i < combined_values_array.Length; i += curr_packet_size)
                {

                    /* 7 bytes per iteration
                     * (2 bytes for motor1, 1 byte for motor1 direction, 2 bytes for motor2, 1 byte for motor2 direction, 1 byte for carraege return) 
                     * THIS IS THE CODE TO DECODE THE BYTES IN THE ESP32 CODE ITS HAS
                     * THE LENGTH OF THE TOTAL BYTES PER PACKET (7) * THE MAXIMUM ARRAY LENGTH OF THE TWO MOTORS
                     */

                    //m1_value_bytes1 = (combined_values_array[i + 0] << 8); // shift 8 bits to the right
                    //m1_value_bytes1 |= (combined_values_array[i + 1] & 0xFF); // bitwise AND with 0xFF

                    m1_value_int = (combined_values_array[i + 0] << 8) | combined_values_array[i + 1]; // same code to decode the bytes must be used for both motors in ESP32 code.

                    ////m2_value_bytes1 = (combined_values_array[i + 3] << 8); // shift 8 bits to the right
                    ////m2_value_bytes1 |= (combined_values_array[i + 4] & 0xFF); // bitwise AND with 0xFF

                    m2_value_int = (combined_values_array[i + 3] << 8) | combined_values_array[i + 4];


                     motor1_direction = (char)combined_values_array[i + 2];
                     motor2_direction = (char)combined_values_array[i + 5];
                     char packet_end = (char)combined_values_array[i + 6];

                    Console.WriteLine($"[{i}] : [{m1_value_int}, {motor1_direction}] , [{m2_value_int}, {motor2_direction}]{packet_end}");
                    Thread.Sleep(1); // sleep for 1ms
                }
            }else if (vals.Equals("X"))
            {
                for (int i = 0; i < combined_values_array.Length; i += curr_packet_size)
                {
                    for (int j = 0; j < curr_packet_size; j++)
                    {
                        Console.Write($"{combined_values_array[i + j]:X2} ");
                    }
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Transmit the values of the motors
        /// <example>
        /// For example:
        /// <code>
        /// byte[] array;
        /// Transmit(array);
        /// </code>
        /// sends the values of the motors to the ESP32 in 1ms period each packet
        /// </example>
        /// <param name="combined_values_array"></param>
        /// </summary>
        private void Transmit(byte[] combined_values_array)
        {
            Stopwatch sw = new Stopwatch();
            // send every 7 bytes
            sw.Reset();
            for (int i = 0; i < combined_values_array.Length - 7; i += curr_packet_size)
            {
                /* 7 bytes per iteration
                 * (2 bytes for motor1, 1 byte for motor1 direction, 2 bytes for motor2, 1 byte for motor2 direction, 1 byte for carraege return) 
                 * THIS IS THE CODE TO DECODE THE BYTES IN THE ESP32 CODE ITS HAS
                 * THE LENGTH OF THE TOTAL BYTES PER PACKET (7) * THE MAXIMUM ARRAY LENGTH OF THE TWO MOTORS
                 */
                
                sw.Restart();

                Serial_Init.sp.Write(combined_values_array, i, curr_packet_size);

                double executionTimeMs = sw.Elapsed.TotalMilliseconds;
                Wait_Time(executionTimeMs);

                //double remainingTimeMs = kk + executionTimeMs;
                //Console.WriteLine($"Execution time: {executionTimeMs} ms, Remaining time: {remainingTimeMs} ms");

            }
            byte[] stop_array = { 0x00, 0x00, 0x52, 0x00, 0x00, 0x4C, 0x3B};
            Serial_Init.sp.Write(stop_array, 0, 7);
            Console.WriteLine("Done");
        }

        /// <summary>
        /// Get the max iterations of the two motors
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        /// </summary>
        private int Get_Max_itrations(int[] array1, int[] array2)
        {
            return Math.Max(array1.Length, array2.Length);
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
                        //current_position = result;
                    }
                    else if (str == "m2")
                    {
                        //current_position = result;
                    }
                }
            }
            // Clear the buffer after processing
            dataBuffer.Clear();
        }
    }
}
