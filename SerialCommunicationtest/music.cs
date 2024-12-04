using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace WinSerialCommunication
{
    internal class music
    {
        public static void PlayMusic(ref SerialPort sp)
        {
            while (true)
            {
                Console.WriteLine("Playing music...");
                sp.Write("m1 293.664 L\n");
                Thread.Sleep(500);
                sp.Write("m1 329.628 L\n");
                Thread.Sleep(100);
                sp.Write("m1 370.0 L\n");
                Thread.Sleep(100);
                sp.Write("m1 392.0 L\n");
                Thread.Sleep(100);
                sp.Write("m1 440.0 L\n");
                Thread.Sleep(200);
                sp.Write("m1 392.0 L\n");
                Thread.Sleep(300);
                sp.Write("m1 329 L\n");
                Thread.Sleep(400);
                sp.Write("m1 293 L\n");
                Thread.Sleep(500);


            }
        }
    }
}
