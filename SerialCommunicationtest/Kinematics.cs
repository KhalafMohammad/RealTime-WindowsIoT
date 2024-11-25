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

        public (double xi, double sigma) CalculateInverseKinematics(double Xi, double Yi)
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

            double xi = omega + gamma;

            sigma = Math.Round(sigma * (180 / Math.PI));

            xi = xi * (180 / Math.PI);

            xi = Math.Round(180 + xi);

            M = Math.Round(M * (180 / Math.PI));

            N = Math.Round(N * (180 / Math.PI));

            sigma = 180 + sigma;
            Console.WriteLine("xi: " + xi + " sigma: " + sigma);
            UDPServer udpServer = new();
            string message = $"{xi} {sigma}";
            udpServer.client(message);
            //udpServer.client();

            return (xi, sigma); // xi for motor 1 sigma for motor 2
        }

    }
}






