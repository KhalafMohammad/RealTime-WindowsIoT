using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace WinSerialCommunication
{
    internal class Serial_Init
    {
        public SerialPort _serialport = new SerialPort("COM5", 20000, Parity.None, 8, StopBits.One);

        public void serial_init()
        {
            // get a list of available ports
            var ports = SerialPort.GetPortNames();

            // display the list to the console
            foreach (string port in ports)
            {
                Console.WriteLine($" {port}");
            }

            // create a new SerialPort object with default settings and 1khz

            // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT

            // open serial port
            _serialport.Open();
            Console.WriteLine("connected to serial port " + _serialport.PortName);
            //_serialport.ReadTimeout = 1000;

        }
    }
}
