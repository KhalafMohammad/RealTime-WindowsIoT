using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    internal class Write
    {
        public static void Data_to_write(ref SerialPort sp)
        {
            
            // setup write timeout
            // because we are writing to the UART it's recommended to set a write timeout
            // otherwise the write operation doesn't return until the requested number of bytes has been written
            sp.WriteTimeout = 500;

            while (true) {
                
                //// write string followed by new line to Serial Device 
                //sp.Write("$" + DateTime.UtcNow + " serial test by mohammad!\r");
                //Debug.WriteLine(DateTime.UtcNow + " serial test by mohammad!\r");
                //Thread.Sleep(750);

                //sp.Write(new byte[] { 0x31 }, 0, 1); // write 1


                //sp.Write(new byte[] { 0x30 }, 0, 1); // write 0
                sp.WriteLine("Z"); 
                Console.WriteLine(GetTimestamp() + " Wrote 'Hello' over " + sp.PortName + "."); 
                Thread.Sleep(750); //0.5millisecond

                sp.Write("T\n");
                Console.WriteLine(GetTimestamp() + " Wrote 1 over " + sp.PortName + ".");
                Thread.Sleep(750); //0.5millisecond
            }
        }

        public static string GetTimestamp() // get the current time in HH:MM:SS:FFF format to print out milliseconds too 
        {
            return DateTime.Now.ToString("HH:mm:ss:fff"); 
        }
    }
}
