using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
   

    internal class TwoAxisRobot
    {
        // Link lengths and distance between motors
        public double L1 { get; set; }
        public double L2 { get; set; }
        public double D { get; set; }

        // Constructor to initialize link lengths and distance between motors
        public TwoAxisRobot(double l1, double l2, double d)
        {
            L1 = l1;
            L2 = l2;
            D = d;
        }

        // Method to calculate motor angles given a target position (Xi, Yi)
        public (double xi, double sigma) CalculateInverseKinematics(double Xi, double Yi)
        {
            // Step 1: Calculate distances h1 and h2
            double h1 = Math.Sqrt(Xi * Xi + Yi * Yi); // distance from motor 1 to target position from the origin (0,0) to the target position
            double h2 = Math.Sqrt((D - Xi) * (D - Xi) + Yi * Yi);

            // Step 2: Calculate intermediate angles gamma and beta
            double gamma = Math.Acos(Xi / h1);
            double beta = Math.Acos((D - Xi) / h2);

            // Step 3: Calculate angles omega and theta
            double omega = Math.Acos((h1 * h1 + L1 * L1 - L2 * L2) / (2 * h1 * L1));
            double theta = Math.Acos((h2 * h2 + L1 * L1 - L2 * L2) / (2 * h2 * L1));

            // Step 4: Calculate final motor angles sigma and xi
            double sigma = Math.PI - theta - beta;
            double xi = omega + gamma;

            // Convert angles to degrees for readability (optional)
            sigma = sigma * (180 / Math.PI);

            xi = xi * (180 / Math.PI);

            xi = 180 + xi;

            sigma = 180 + sigma;
            Console.WriteLine("xi: " + xi + " sigma: " + sigma);
            return (xi, sigma); // xi for motor 1 sigma for motor 2
        }
    }
}






