
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

        public static int position; // Current stepper position
        public static int prev_position; //previous
        static void Main()
        {
            try
            {
                //IntPtr aff_mask = (IntPtr)0xC0; // use only the first processor
                // Set the process priority to high and the thread priority to time critical
                RealTime.Process_managment(Process.GetCurrentProcess(), 0xC0, ProcessPriorityClass.RealTime);
                RealTime.Threads_managment(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical);


                // Initialize the serial port
                //Serial_Init Serial_Init = new Serial_Init();

                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
                //Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                // Initialize the serial port
                //Serial_Init.serial_init();


                //write to serial port [UNCOMMENT TO USE]            
                //Write.Data_to_write(ref Serial_Init._serialport);

                Exception exception;

                const double L1 = 7.500;
                const double L2 = 16.250;
                const double D = 8.750;


                const double tool_offset = 3.300;
                double Xi = 4.375; // x mini
                double Yi = 9.90; // y minimumm 10


                TwoAxisRobot ZTIMK_Bot = new TwoAxisRobot(L1, L2, D);
                (int motor1_angle, int motor2_angle) = ZTIMK_Bot.CalculateInverseKinematics(Xi, Yi);

                //motor1 angle domain is -10 to 90
                //motor2 angle domain is 90 to 190
                Write.Steps_to_angle(1250);


                if (motor1_angle > 190 || motor1_angle < 90)
                {
                    exception = new Exception("Motor 1 angle is out of domain");
                }
                else if (motor2_angle > 90 || motor2_angle < -10)
                {
                    exception = new Exception("Motor 2 angle is out of domain");
                }
                else
                {
                    exception = null;
                }

                if (exception != null)
                {
                    throw exception;
                }
                else
                {
                    Console.WriteLine("Motor 1 angle: " + motor1_angle + " Motor 2 angle: " + motor2_angle);

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ProcessData(ref SerialPort sp)
        {
            string data = dataBuffer.ToString(); // get the data from the buffer
            string[] dataParts = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); // get data for the recieve buffer and split it into parts

            foreach (string part in dataParts) // loop door de data parts
            {
                if (int.TryParse(part, out int result)) //  CHECK als de data een integer is
                {
                    Console.WriteLine(temp_Write.GetTimestamp() + " Integer Received: <<< " + result);
                }

            }
            // Clear the buffer after processing
            dataBuffer.Clear();
        }
    }
}