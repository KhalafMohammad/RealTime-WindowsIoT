using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{

    internal class TwoAxisRobot
    {
        public double L1 { get; set; }
        public double L2 { get; set; }
        public double D { get; set; }
        public int motor1_position { get; set; }
        public int motor2_position { get; set; }
        public double M;

        public double N;


        public TwoAxisRobot(double l1, double l2, double d)
        {
            L1 = l1;
            L2 = l2;
            D = d;
        }

        public (int zeta, int sigma) CalculateInverseKinematics(double Xi, double Yi)
        {
            double h1 = Math.Sqrt(Xi * Xi + Yi * Yi); // distance from motor 1 to target position from the origin (0,0) to the target position
            double h2 = Math.Sqrt((D - Xi) * (D - Xi) + Yi * Yi);

            double gamma = Math.Acos(Xi / h1);
            double beta = Math.Acos((D - Xi) / h2);

            double omega = Math.Acos((h1 * h1 + L1 * L1 - L2 * L2) / (2 * h1 * L1));

            double theta = Math.Acos((h2 * h2 + L1 * L1 - L2 * L2) / (2 * h2 * L1));

            M = Math.Acos((L1 * L1 + L2 * L2 - h1 * h1) / (2 * L1 * L2));

            N = Math.Acos((L1 * L1 + L2 * L2 - h2 * h2) / (2 * L1 * L2));

            double sigma = Math.PI - theta - beta;

            double zeta = omega + gamma;
            Console.WriteLine("zeta: " + zeta + " sigma: " + sigma + " N " + N + " M" + M );
            

            sigma = Math.Round(sigma * (180 / Math.PI));

            zeta = Math.Round(zeta * (180 / Math.PI));

            //zeta = Math.Round(180 + zeta);

            M = Math.Round(M * (180 / Math.PI)); // joint angle motor 1 with zeta

            N = Math.Round(N * (180 / Math.PI)); // joint angle motor 2 with sigma
           

            Console.WriteLine($"zeta: {zeta} sigma: {sigma} M: {M} N: {N}");

            // blender calculations and udp sending to blender
            //sigma = 180 + sigma;
            //Console.WriteLine("xi: " + zeta + " sigma: " + sigma);
            //UDPServer udpServer = new();
            //string message = $"{zeta} {sigma}";
            //udpServer.client(message);
            ////udpServer.client();

            return ((int)zeta, (int)sigma); // xi for motor 1 sigma for motor 2
        }
        public void CalculateForwardKinematics(double angle2, double angle1)
        {
            double sigmaRad = angle1 * (2* Math.PI / 360);
            double m1Rad = angle2 * (2 * Math.PI / 360);

            double x1 = L1 * Math.Cos(sigmaRad);
            double y1 = L1 * Math.Sin(sigmaRad);

            double x2 = x1 + L2 * Math.Cos(sigmaRad + m1Rad);
            double y2 = y1 + L2 * Math.Sin(sigmaRad + m1Rad);
            Console.WriteLine($"x1: {x1} y1: {y1} x2: {x2} y2: {y2}");
        }
    }
}






