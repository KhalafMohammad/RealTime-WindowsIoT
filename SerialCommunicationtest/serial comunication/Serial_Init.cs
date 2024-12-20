using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

namespace WinSerialCommunication
{
    internal class Serial_Init
    {
        public SerialPort sp = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);

        public void serial_init()
        {
            try
            {

                // USE THIS INSTEAD OF THE READ FUNCTION TO READ DATA FROM THE SERIAL PORT
                sp.WriteTimeout = -1;
                sp.ReadTimeout = -1;

                // open serial port
                sp.Open();
                Console.WriteLine("connected to serial port " + sp.PortName);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
