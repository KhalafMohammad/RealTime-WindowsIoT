using System.Diagnostics;

namespace WinSerialCommunication
{
    internal class Scurve
    {
        public int accelertion = 2000;
        public double curr_freq = 0;
        public double j_max;
        public double t_j;
        public double dt = 0.001F;
        public int steps; // target position
        private char dir;
        const double error = 0.005f;
        public long frequency;
        public Scurve(double j_max, double j, int target)
        {
            this.j_max = j_max;
            t_j = j;
            this.steps = target;
        }

        public (int[] motor_values, char dir) Get_curve_values()
        {
            double max = (double)(t_j * 3) * 1000 +10;
            Console.WriteLine("Max: " + max);
            int itr = 0;
            int[] motor = new int[(int)max];

            #region direction
            if (steps < 0) // ditermin the direction of the motor
            {
                dir = 'L';
            }
            else
            {
                dir = 'R';
            }
            #endregion direction

            #region Phase 1

            for (double t = 0.000f; t < t_j; t += dt)
            {
                curr_freq = Math.Round(accelertion * (1 - (float)Math.Pow((1 - t / t_j), 2))); // S-curve formula 
                motor[itr] = (int)curr_freq;
                itr++;
            }

            #endregion Phase 1

            #region Phase 2
            for (double t = 0; t < j_max + error; t += dt)
            {
                curr_freq = accelertion;

                motor[itr] = (int)curr_freq;
                itr++;


            }

            #endregion Phase 2

            #region Phase 3
            for (double t = t_j; t > 0; t -= dt)
            {
                curr_freq = Math.Round(accelertion * (1 - (float)Math.Pow((1 - t / t_j), 2))); //0.50F
                motor[itr] = (int)curr_freq;
                itr++;
            }
            return (motor, dir);

            #endregion Phase 3

        }
    }
}
