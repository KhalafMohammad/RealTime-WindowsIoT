﻿using System;
using System.Collections.Generic;
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

        public static int position; // Current stepper position

        public Robot()
        {
            //motor1 angle domain is -10 to 90
            //motor2 angle domain is 90 to 190
            Write.Steps_to_angle(1250); // test

            ZTIMK_Bot = new TwoAxisRobot(L1, L2, D);
            ZTIMK_Bot.motor1_position = 90;
            ZTIMK_Bot.motor2_position = 90;
        }

        public void coordinates(double x, double y)
        {
            (Motor1_angle ,Motor2_angle) = ZTIMK_Bot.CalculateInverseKinematics(x, y);
            Console.WriteLine(x + " " + y);
        }

        public void Run()// ref SerialPort sp
        {
            ZTIMK_Bot.CalculateForwardKinematics(-ZTIMK_Bot.M, Motor1_angle);
            //const double tool_offset = 3.300;
            Console.WriteLine($"motor1 angle:  {Motor1_angle} motor1 angle: {Motor2_angle} ");
            if (Motor1_angle > 190 || Motor1_angle < 90)
            {
                throw new MotorAngleException("Motor 1 angle is out of domain");
            }
            else if (Motor2_angle > 90 || Motor2_angle < -10)
            {
                throw new MotorAngleException("Motor 2 angle is out of domain");
            }
            else
            {
                Console.WriteLine("Motor 1 angle: " + Motor1_angle + " Motor 2 angle: " + Motor2_angle);
            }

            int Moto1_tomove_angle = ZTIMK_Bot.motor1_position - (Motor1_angle); // calculate the angle to move for motor 1

            position = Motor1_angle; // update the current position of the motor
            Console.WriteLine($"position To move = {Moto1_tomove_angle}");
            ZTIMK_Bot.motor1_position = Motor1_angle; // update the current position of the motor
            Console.WriteLine($"position after move = {ZTIMK_Bot.motor1_position}");


            int steps = Write.Angle_to_steps(Moto1_tomove_angle); // convert the angle to steps

            //Error_Compensate(position, steps, ref sp, "E1 "); // MotorID for motor1 instead of m1 use => E1 || For motor2 instead of m2 use => E2
        }

        public static void Error_Compensate(int current_position, int initial_position, ref SerialPort sp, string motorID)
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
