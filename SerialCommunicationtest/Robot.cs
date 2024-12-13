using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    internal class Robot
    {
        const double L1 = 7.500;
        const double L2 = 16.250;
        const double D = 8.750;

        private TwoAxisRobot ZTIMK_Bot;
        public int Motor1_angle { get; private set; }
        public int Motor2_angle { get; private set; }

        public static int m1_position; // Current stepper1 position
        public static int m2_position; // Current stepper2 position

        public Robot()
        {
            //motor1 angle domain is -10 to 90
            //motor2 angle domain is 90 to 190
            ZTIMK_Bot = new TwoAxisRobot(L1, L2, D);
            ZTIMK_Bot.motor1_position = 90;
            ZTIMK_Bot.motor2_position = 90;
        }

        public void coordinates(double x, double y)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("###############################################");
            Console.WriteLine("kinematics calculation");
            Console.ForegroundColor = ConsoleColor.Green;
            (Motor1_angle, Motor2_angle) = ZTIMK_Bot.CalculateInverseKinematics(x, y);

            (double f_x, double f_y) = ZTIMK_Bot.get_xy(Motor1_angle, 180 - -ZTIMK_Bot.M); // forward kinematics
            Console.WriteLine($" X_arm1 => {f_x:f3}, Y_arm1 => {f_y:f3}");

            (f_x, f_y) = ZTIMK_Bot.get_xy(Motor2_angle, 180 - ZTIMK_Bot.N);
            Console.WriteLine($" X_arm2 =>{f_x + D:f3}, Y_arm2 => {f_y:f3}");
            Console.WriteLine("###############################################");

            Console.ResetColor();
        }

        public void Run(ref SerialPort sp)// ref SerialPort sp
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("###############################################");
            Console.WriteLine("Motors angles and movment calculations!!");
            //const double tool_offset = 3.300;
            if (Motor1_angle > 190 || Motor1_angle < 90)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                throw new MotorAngleException("Motor 1 angle is out of domain");
            }
            else if (Motor2_angle > 90 || Motor2_angle < -10)
            {
                throw new MotorAngleException("Motor 2 angle is out of domain");
            }
            else if (Motor1_angle > 190 || Motor1_angle < 90 && Motor2_angle > 90 || Motor2_angle < -10)
            {
                throw new MotorAngleException("Motor 1 and Motor 2 angle is out of domain");
            }
            else
            {
                Console.WriteLine("Motor 1 angle: " + Motor1_angle + " Motor 2 angle: " + Motor2_angle);
            }
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("###############################################");
            int Motor1_tomove_angle = ZTIMK_Bot.motor1_position - (Motor1_angle); // calculate the angle to move for motor 1
            int Motor2_tomove_angle = ZTIMK_Bot.motor2_position - (Motor2_angle); // calculate the angle to move for motor 2

            m1_position = Motor1_angle; // update the current position of the motor
            m2_position = Motor2_angle; // update the current position of the motor

            Console.WriteLine($"motor1 position To move = {Motor1_tomove_angle}\nmotor2 position To move = {Motor2_tomove_angle} ");
            ZTIMK_Bot.motor1_position = Motor1_angle; // update the current position of the motor
            ZTIMK_Bot.motor2_position = Motor2_angle; // update the current position of the motor
            Console.WriteLine($"motor1 after move = {ZTIMK_Bot.motor1_position}\nmotor2 after move = {ZTIMK_Bot.motor2_position}");


            int m1_steps = Write.Angle_to_steps(Motor1_tomove_angle); // convert the angle to steps
            int m2_steps = Write.Angle_to_steps(Motor2_tomove_angle); // convert the angle to steps

            Thread m1_thread = new Thread(() =>
            {
                RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0x80); // 0x80 core 8 affinity for motor1 
                (double t1, double t2, double t3) = Write.calculate_time(m1_steps);
                Scurve2 Motor1 = new Scurve2(t1, t3, m1_steps, m1_position, "m1 ");
                Motor1.Move(ref sp, m1_steps); //error in this line
            });

            Thread m2_thread = new Thread(() =>
            {
                RealTime.manage_thread(Process.GetCurrentProcess(), ThreadPriorityLevel.TimeCritical, (IntPtr)0x40); // 0x40 core 7 affinity for motor2
                (double t1, double t2, double t3) = Write.calculate_time(m2_steps);
                Scurve2 Motor2 = new Scurve2(t1, t3, m2_steps, m2_position, "m2 ");
                Motor2.Move(ref sp, m2_steps); //error in this line
            }); 

            //Error_Compensate(position, steps, ref sp, "E1 "); // MotorID for motor1 instead of m1 use => E1 || For motor2 instead of m2 use => E2
            Console.WriteLine("###############################################");
            Console.ResetColor();
        }

        public void Error_Compensate(int current_position, int initial_position, ref SerialPort sp, string motorID)
        {
            if (current_position > initial_position || current_position < initial_position)
            {
                Console.WriteLine("Error in position detected");
                int position_compensation = initial_position - current_position;
                if (position_compensation < 0)
                {
                    string new_position = position_compensation.ToString();
                    sp.Write(motorID + new_position + " L\n");
                }
                else if (position_compensation > 0)
                {
                    string new_position = position_compensation.ToString();
                    sp.Write(motorID + new_position + " R\n");
                }
            }
            else
            {
                Console.WriteLine("No error in position detected");
            }
        }


    }

    [Serializable]
    public class MotorAngleException : Exception
    {
        public MotorAngleException()
        { }

        public MotorAngleException(string message)
            : base(message)
        { }

        public MotorAngleException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
