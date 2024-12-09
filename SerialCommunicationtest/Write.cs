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
using System.Security.Cryptography;

//success 
namespace WinSerialCommunication
{
    internal class Write
    {
        public static double steps_per_angle = 5000.00f / 360.0f;
        public static bool recieve_flag = true;
        public static int value;
        public static int position;
        public static double t1;
        public static double t2;
        public static double t3;
        public static void Data_to_write(ref SerialPort sp)
        {
            Exception exception;

            while (true)
            {
                try
                {
                    Console.WriteLine("\nVoer in de steppen? >>>"); // any value is going to be the mount of pulses per second.
                    value = Convert.ToInt32(Console.ReadLine());
                    //string value_string = Console.ReadLine();
                    //sp.Write(value_string);

                    double tool_offset = 3.300;
                    double Xi = 7.500;
                    double Yi = 5.350 + tool_offset; // y minimumm 10


                    TwoAxisRobot ZTIMK_Bot = new TwoAxisRobot(75.00, 162.50, 87.50);
                    (int motor1_angle, int motor2_angle) = ZTIMK_Bot.CalculateInverseKinematics(Xi, Yi);
                    //motor1 angle domain is -20 to 90
                    //motor2 angle domain is 90 to 200

                    if (motor1_angle > 90 || motor1_angle < -20)
                    {
                        exception = new Exception("Motor 1 angle is out of domain");
                    }
                    else if (motor2_angle > 200 || motor2_angle < 90)
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

                        int steps = Angle_to_steps(motor1_angle);
                        Console.WriteLine(steps + " steps");
                        calculate_time(steps);
                        Scurve Motor1 = new Scurve(t1, t3, steps, position, "m1 ");
                        Console.WriteLine($"{Motor1.t_j}, {Motor1.j_max}");
                        Motor1.Phase_one(ref sp, steps);
                        Motor1.Phase_two(ref sp, steps);
                        Motor1.Phase_three(ref sp, steps);
                    }

                    //Console.WriteLine(Angle_to_steps(value) + " steps");
                    //calculate_time(Angle_to_steps(value));
                    //Scurve2 Motor2 = new Scurve2(t1, t3, Angle_to_steps(value), position, "m2 ");
                    //Console.WriteLine($"{Motor2.t_j}, {Motor2.j_max}");
                    //Motor2.Phase_one(ref sp, Angle_to_steps(value));
                    //Motor2.Phase_two(ref sp, Angle_to_steps(value));
                    //Motor2.Phase_three(ref sp, Angle_to_steps(value));


                    //music.PlayMusic(ref sp);    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static string GetTimestamp() // get the current time in HH:MM:SS:FFF format to print out milliseconds too 
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }
        public static void data(ref SerialPort sp, int data)
        {
            byte[] value_bytes = new byte[2];
            value_bytes[0] = (byte)((int)data >> 8); // shift 8 bits to the right
            value_bytes[1] = (byte)((int)data & 0xFF); // bitwise AND with 0xFF
            sp.Write(value_bytes, 0, 2); // write 1
            Console.WriteLine(Write.GetTimestamp() + " Wrote " + data + " over" + sp.PortName + ".");
        }
        public static (double t1, double t2, double t3) calculate_time(int steps)
        {
            int transmit_speed = 2000;
            // 0.001 = 1ms per step 
            int value1 = Math.Abs(steps);
            double t_j = (double)value1 / transmit_speed;
            t1 = t_j / 3;
            t1 = Math.Round(t1, 3);
            t3 = t_j / 3;
            t3 = Math.Round(t3, 3);
            t2 = t3 + t1;
            Console.WriteLine($"t_j: {t_j:f4} t1:{t1:f4} , t2:{t2:f4}, t3:{t3:f4} ");
            //double t_j = Math.Sqrt((double)value / 2000);
            //Console.WriteLine("t_j: " + t_j);
            return (t1, t2, t3);
        }


        public static int Angle_to_steps(int angle)
        {
            
            return (int)(Math.Round(angle * steps_per_angle));
        }

        public static int Steps_to_angle(int steps)
        {
            
            Console.WriteLine(Math.Round(steps / steps_per_angle));
            return (int)(Math.Round(steps / steps_per_angle));
        }
    }
}