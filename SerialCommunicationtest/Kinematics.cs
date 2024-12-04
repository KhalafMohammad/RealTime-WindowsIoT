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

            double M     = Math.Acos((L1 * L1 + L2 * L2 - h1 * h1) / (2 * L1 * L2));

            double N     = Math.Acos((L1 * L1 + L2 * L2 - h2 * h2) / (2 * L1 * L2));

            double sigma = Math.PI - theta - beta;

            double zeta = omega + gamma;

            sigma = Math.Round(sigma * (180 / Math.PI));

            zeta = zeta * (180 / Math.PI);

            //zeta = Math.Round(180 + zeta);

            M = Math.Round(M * (180 / Math.PI)); // joint angle motor 1

            N = Math.Round(N * (180 / Math.PI)); // joint angle motor 2



            // blender calculations and udp sending to blender
            //sigma = 180 + sigma;
            //Console.WriteLine("xi: " + zeta + " sigma: " + sigma);
            //UDPServer udpServer = new();
            //string message = $"{zeta} {sigma}";
            //udpServer.client(message);
            ////udpServer.client();

            return ((int)zeta, (int)sigma); // xi for motor 1 sigma for motor 2
        }
        //public (double Xi, double Yi) CalculateForwardKinematics(int zeta, int sigma)
        //{
        //    double zetaRad = zeta * (Math.PI / 180);
        //    double sigmaRad = sigma * (Math.PI / 180);

        //    double x1 = L1 * Math.Cos(zetaRad);
        //    double y1 = L1 * Math.Sin(zetaRad);

        //    double x2 = x1 + L2 * Math.Cos(zetaRad + sigmaRad);
        //    double y2 = y1 + L2 * Math.Sin(zetaRad + sigmaRad);
        //    Console.WriteLine($"x1: {x1} y1: {y1} x2: {x2} y2: {y2}");
        //    return (x2, y2);
        //}
    }
}






