using System.Dynamic;
using System.IO.Ports;

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


        public static double steps_per_angle = 5000.00f / 360.0f;
        public static bool recieve_flag = true;
        public static int value;
        public static double t1;
        public static double t2;
        public static double t3;
        

        public Robot()
        {
            //motor1 angle domain is -10 to 90
            //motor2 angle domain is 90 to 190
            ZTIMK_Bot = new TwoAxisRobot(L1, L2, D);
            ZTIMK_Bot.motor1_position = 90;
            ZTIMK_Bot.motor2_position = 90;
           
        }

        /// <summary>
        /// Calculate the motor angles and the movement
        /// </summary>
        /// <param name="x"> x coordinates </param>
        /// <param name="y"> y coordinates </param>
        public void coordinates(double x, double y)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("###############################################");
            Console.WriteLine("kinematics calculation");
            Console.ForegroundColor = ConsoleColor.Green;
            (Motor2_angle, Motor1_angle) = ZTIMK_Bot.CalculateInverseKinematics(x, y);

            //Console.WriteLine("calculate the forward kinematics");
            //Console.WriteLine("################################################");
            //(double f_x, double f_y) = ZTIMK_Bot.get_xy(Motor2_angle, 180 - -ZTIMK_Bot.M); // forward kinematics
            //Console.WriteLine($" X_arm1 => {f_x:f3}, Y_arm1 => {f_y:f3}");

            //(f_x, f_y) = ZTIMK_Bot.get_xy(Motor1_angle, 180 - ZTIMK_Bot.N);
            //Console.WriteLine($" X_arm2 =>{f_x + D:f3}, Y_arm2 => {f_y:f3}");
            //Console.WriteLine("###############################################");

            Console.ResetColor();
        }



        public void Run()// ref SerialPort sp
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("###############################################");
            Console.WriteLine("Motors angles and movment calculations!!");
            //const double tool_offset = 3.300;
            if (Motor2_angle > 190 || Motor2_angle < 90)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                throw new MotorAngleException("Motor 1 angle is out of domain");
            }
            else if (Motor1_angle > 90 || Motor1_angle < -10)
            {
                throw new MotorAngleException("Motor 2 angle is out of domain");
            }
            else if (Motor2_angle > 190 || Motor2_angle < 90 && Motor1_angle > 90 || Motor1_angle < -10)
            {
                throw new MotorAngleException("Motor 1 and Motor 2 angle is out of domain");
            }
            else
            {
                Console.WriteLine("Motor 1 angle: " + Motor1_angle + " Motor 2 angle: " + Motor2_angle);

                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("###############################################");
                int Motor1_tomove_angle = Motor1_angle - ZTIMK_Bot.motor1_position; // calculate the angle to move for motor 1
                int Motor2_tomove_angle = Motor2_angle - ZTIMK_Bot.motor2_position; // calculate the angle to move for motor 2

                m1_position = Motor1_angle; // update the current position of the motor
                m2_position = Motor2_angle; // update the current position of the motor

                Console.WriteLine($"motor1 position To move = {Motor1_tomove_angle}\nmotor2 position To move = {Motor2_tomove_angle} ");



                int m1_steps = Angle_to_steps(Motor1_tomove_angle); // convert the angle to steps
                (double t1, double t2, double t3) = calculate_time(m1_steps); // calculate the time for the motor to move
                Scurve motor1 = new Scurve(t1, t3, m1_steps); // create a new instance of the motor
                (int[] motor1_values, char motor1_dir) = motor1.Get_curve_values(); // get the values and direction of the motor


                int m2_steps = Angle_to_steps(Motor2_tomove_angle); // convert the angle to steps
                (t1, t2, t3) = calculate_time(m2_steps); // calculate the time for the motor to move 
                Scurve motor2 = new Scurve(t1, t3, m2_steps); // create a new instance of the motor
                (int[] motor2_values, char motor2_dir) = motor2.Get_curve_values(); // get the values and direction of the motor

                Console.WriteLine("Motor 1 steps: " + m1_steps);
                Console.WriteLine("Motor 2 steps: " + m2_steps);
                Program.m1_steps = Math.Abs(m1_steps);
                Program.m2_steps = m2_steps;
                PacketList packetList = new PacketList();
                packetList.Test(motor1_values, motor1_dir, motor2_values, motor2_dir);
                
                ZTIMK_Bot.motor1_position = Motor1_angle; // update the current position of the motor
                ZTIMK_Bot.motor2_position = Motor2_angle; // update the current position of the motor
                Console.WriteLine($"motor1 after move = {ZTIMK_Bot.motor1_position}\nmotor2 after move = {ZTIMK_Bot.motor2_position}");

                Console.WriteLine("###############################################");
                motor1_values = null;
                motor2_values = null;
                motor1 = null;
                motor2 = null;
            }
            //ResetVariables();

            Console.ResetColor();

        }

        /// <summary>
        ///     Convert the angle to steps
        /// <param name="current_position"></param>
        /// <param name="initial_position"></param>
        /// <param name="sp"></param>
        /// <param name="motorID"></param>
        /// </summary>
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

        /// <summary>
        /// Get the current time in HH:MM:SS:FFF format to print out milliseconds too
        /// <returns></returns>
        ///</summary>
        public static string GetTimestamp() // get the current time in HH:MM:SS:FFF format to print out milliseconds too 
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }
        /// <summary>
        ///     Write data to the serial port
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="data"></param>
        public static void Data(ref SerialPort sp, int data)
        {
            byte[] value_bytes = new byte[2];
            value_bytes[0] = (byte)((int)data >> 8); // shift 8 bits to the right
            value_bytes[1] = (byte)((int)data & 0xFF); // bitwise AND with 0xFF
            sp.Write(value_bytes, 0, 2); // write 1
            Console.WriteLine(GetTimestamp() + " Wrote " + data + " over" + sp.PortName + ".");
        }

        /// <summary>
        ///     Convert the angle to steps
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
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

        /// <summary>
        ///    Convert the angle to steps
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static int Angle_to_steps(int angle)
        {

            return (int)(Math.Round(angle * steps_per_angle));
        }

        /// <summary>
        ///     Convert the steps to angle
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static int Steps_to_angle(int steps)
        {

            Console.WriteLine(Math.Round(steps / steps_per_angle));
            return (int)(Math.Round(steps / steps_per_angle));
        }
        private void ResetVariables()
        {
            //m1_position = 0;
            //m2_position = 0;
            steps_per_angle = 5000.00f / 360.0f;
            value = 0;
            t1 = 0;
            t2 = 0;
            t3 = 0;
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
