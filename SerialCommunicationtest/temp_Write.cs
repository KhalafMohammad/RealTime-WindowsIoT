using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Pipes;


namespace WinSerialCommunication
{
    internal class temp_Write
    {
        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        public static bool recieve_flag = false;
        public static void Data_to_write(ref SerialPort sp)
        {
            bool is_pressed_UP = false;
            bool is_pressed_DWN = false;
            bool is_pressed_RHT = false;
            bool is_pressed_LFT = false;
            bool is_pressed_SPACE = false;
            recieve_flag = false;

            // setup write timeout
            // because we are writing to the UART it's recommended to set a write timeout
            // otherwise the write operation doesn't return until the requested number of bytes has been written
            sp.WriteTimeout = 500;

            while (true)
            {
                // check if a key is available


                if (Keystate(0x26)) // UP ARROW
                {

                    if (!is_pressed_UP && !recieve_flag) // if the key is pressed and the recieve flag is false
                    {
                        Console.WriteLine($"1- recieve_flag is {recieve_flag}\r");
                        is_pressed_UP = true;
                        recieve_flag = true;
                        int value = 360;
                        byte[] value_bytes = new byte[2];
                        value_bytes[0] = (byte)(value >> 8); // shift 8 bits to the right
                        value_bytes[1] = (byte)(value & 0xFF); // bitwise AND with 0xFF

                        sp.Write(value_bytes, 0, 2); // write 1
                        Console.WriteLine(GetTimestamp() + " Wrote " + value + " over" + sp.PortName + ".");

                        Console.WriteLine("UP ARROW PRESSED");
                        
                        Console.WriteLine($"2- recieve_flag is {recieve_flag}\r");
                    }

                }
                else
                {
                    if (is_pressed_UP)
                    {
                        is_pressed_UP = false;
                        Console.WriteLine($"3 - recieve_flag is {recieve_flag}\r");
                        Console.WriteLine("UP ARROW RELEASED");
                    }
                }


                if (Keystate(0x28)) // DOWN ARROW
                {
                    if (!is_pressed_DWN && !recieve_flag) // if the key is pressed and the recieve flag is false
                    {
                        recieve_flag = true;
                        is_pressed_DWN = true;
                        short value = -360;
                        byte[] value_bytes = BitConverter.GetBytes(value); // convert to byte array
                        Array.Reverse(value_bytes); // reverse the array
                        sp.Write(value_bytes, 0, 2); // write 
                        Console.WriteLine(GetTimestamp() + " Wrote -" + value + "  over" + sp.PortName + ".");
                    }

                }
                else
                {
                    if (is_pressed_DWN)
                    {
                        is_pressed_DWN = false;
                        Console.WriteLine($"3 - recieve_flag is {recieve_flag}\r");
                        Console.WriteLine("Down ARROW RELEASED");
                    }
                }

                if (Keystate(0x27)) // RIGHT ARROW
                {

                    if (!is_pressed_RHT && !recieve_flag) // if the key is pressed and the recieve flag is false
                    {
                        Console.WriteLine($"1- recieve_flag is {recieve_flag}\r");
                        is_pressed_RHT = true;
                        recieve_flag = true;
                        int value = 180;
                        byte[] value_bytes = new byte[2];
                        value_bytes[0] = (byte)(value >> 8); // shift 8 bits to the right
                        value_bytes[1] = (byte)(value & 0xFF); // bitwise AND with 0xFF

                        sp.Write(value_bytes, 0, 2); // write 1
                        Console.WriteLine(GetTimestamp() + " Wrote " + value + " over" + sp.PortName + ".");

                        Console.WriteLine("Right ARROW PRESSED");

                        Console.WriteLine($"2- recieve_flag is {recieve_flag}\r");
                    }

                }
                else
                {
                    if (is_pressed_RHT)
                    {
                        is_pressed_RHT = false;
                        Console.WriteLine($"3 - recieve_flag is {recieve_flag}\r");
                        Console.WriteLine("Right ARROW RELEASED");
                    }
                }

                if (Keystate(0x25)) // LEFT ARROW
                {

                    if (!is_pressed_LFT && !recieve_flag) // if the key is pressed and the recieve flag is false
                    {
                        Console.WriteLine($"1- recieve_flag is {recieve_flag}\r");
                        is_pressed_LFT = true;
                        recieve_flag = true;
                        int value = 90;
                        byte[] value_bytes = new byte[2];
                        value_bytes[0] = (byte)(value >> 8); // shift 8 bits to the right
                        value_bytes[1] = (byte)(value & 0xFF); // bitwise AND with 0xFF

                        sp.Write(value_bytes, 0, 2); // write 1

                        Console.WriteLine(GetTimestamp() + " Wrote " + value + " over" + sp.PortName + ".");

                        Console.WriteLine("left ARROW PRESSED");

                        Console.WriteLine($"2- recieve_flag is {recieve_flag}\r");
                    }

                }
                else
                {
                    if (is_pressed_LFT)
                    {
                        is_pressed_LFT = false;
                        Console.WriteLine($"3 - recieve_flag is {recieve_flag}\r");
                        Console.WriteLine("left ARROW RELEASED");
                    }
                }

                if (Keystate(0x20)) // space bar
                {

                    if (!is_pressed_SPACE && !recieve_flag) // if the key is pressed and the recieve flag is false
                    {
                        Console.WriteLine($"1- recieve_flag is {recieve_flag}\r");
                        is_pressed_SPACE = true;
                        recieve_flag = true;
                        

                        Console.WriteLine("space bar PRESSED");

                        Console.WriteLine($"2- recieve_flag is {recieve_flag}\r");
                    }
                    else // if key is pressed and hold 
                    {
                        int value = 1;
                        byte[] value_bytes = new byte[2];
                        value_bytes[0] = (byte)(value >> 8); // shift 8 bits to the right
                        value_bytes[1] = (byte)(value & 0xFF); // bitwise AND with 0xFF

                        sp.Write(value_bytes, 0, 1); // write 1

                        Console.WriteLine(GetTimestamp() + " Wrote " + value + " over" + sp.PortName + ".");
                    }

                }
                else
                {
                    if (is_pressed_SPACE)
                    {
                        is_pressed_SPACE = false;
                        Console.WriteLine($"3 - recieve_flag is {recieve_flag}\r");
                        Console.WriteLine("space bar RELEASED");
                    }
                }

                //int value0 = 0;
                //byte[] value_bytes0 = new byte[2];
                //value_bytes0[0] = (byte)(value0 >> 8); // shift 8 bits to the right
                //value_bytes0[1] = (byte)(value0 & 0xFF); // bitwise AND with 0xFF

                //sp.Write(value_bytes0, 0, 2); // write 1

                //// write string followed by new line to Serial Device 
                //sp.Write("$" + DateTime.UtcNow + " serial test by mohammad!\r");
                //Debug.WriteLine(DateTime.UtcNow + " serial test by mohammad!\r");
                //Thread.Sleep(750);

                //sp.WriteLine("Z"); 
                //Console.WriteLine(GetTimestamp() + " Wrote 'Z' over " + sp.PortName + "."); 
                //Thread.Sleep(750); //0.5millisecond

                //sp.WriteLine("T");
                //Console.WriteLine(GetTimestamp() + " Wrote 'T' over " + sp.PortName + ".");
                //Thread.Sleep(750); //0.5millisecond
            }
        }

        public static string GetTimestamp() // get the current time in HH:MM:SS:FFF format to print out milliseconds too 
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }

        public static bool Keystate(int vKey)
        {
            return (GetKeyState(vKey) & 0x8000) != 0;
        }
    }
}
