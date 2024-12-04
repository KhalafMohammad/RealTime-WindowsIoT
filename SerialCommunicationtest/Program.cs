
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
                Process process = Process.GetCurrentProcess();
                process.ProcessorAffinity = 0xF00; // use only the first processor
                process.PriorityClass = ProcessPriorityClass.Normal;
                Console.WriteLine("Processor affinity: " + process.ProcessorAffinity);
                for (int i = 0; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
                {
                    process.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
                }

                // create a new SerialPort object with default settings and 1khz
                Serial_Init Serial_Init = new Serial_Init();

                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT on Interrupt
                //Thread newthread = new Thread(() => Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler));
                //newthread.Start();
                Serial_Init._serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                Serial_Init.serial_init();


                //write to serial port [UNCOMMENT TO USE]            
                Write.Data_to_write(ref Serial_Init._serialport);

        

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
            try
            {

                SerialPort sp = (SerialPort)sender;

                //string line = sp.ReadLine();
                //int value_abs = Math.Abs(Write.value);

                //if (int.TryParse(line, out int result)) // Try to parse the line as an integer
                //{

                //    Console.WriteLine(result);
                //}
                //else
                //{
                //    Console.WriteLine("Invalid integer format");
                //}

                byte[] buffer = new byte[sp.BytesToRead]; // create a buffer to store the data
                sp.Read(buffer, 0, buffer.Length); // read the data from the serial port
                if (buffer.Length > 0) // check if the buffer has data
                {
                    //string decode_string = Encoding.ASCII.GetString(buffer); // convert the buffer to string
                    //if (int.TryParse(decode_string, out int result)) // convert the string to an integer
                    //{
                    //    Console.WriteLine("Integer Received: " + result);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Invalid integer format");
                    //}


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
                    //int value_abs = Math.Abs(Write.value);
                    //if (result >= value_abs || result > value_abs - 5 && result < value_abs + 5) //  || result > value_abs - 5 && result < value_abs + 5
                    //{
                    //    Scurve.flag = false;
                    //    Scurve2.flag = false;
                    //}
                    Console.WriteLine(temp_Write.GetTimestamp() + " Integer Received: <<< " + result);
                }

            }
            // Clear the buffer after processing
            dataBuffer.Clear();
        }

        

    }
}