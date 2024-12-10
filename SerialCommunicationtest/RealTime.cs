using System.Diagnostics;
using System.Runtime.InteropServices;


namespace WinSerialCommunication
{
    internal class RealTime
    {
        public Process process = Process.GetCurrentProcess();
        public IntPtr processorAffinity = (IntPtr)0xF0; // use only the first processor

        // set the process to real-time priority
        // 
        public static void Process_managment(Process process, IntPtr aff_mask, ProcessPriorityClass pri_class)
        {
            process.ProcessorAffinity = aff_mask; // use only the first processor

            process.PriorityClass = pri_class;

            Console.WriteLine("All Threads are automatically set to the desired cpu affinity, with normal priority!.");
            Console.WriteLine("Process CPU affinity: " + process.ProcessorAffinity);
            Console.WriteLine("Priority Class: " + process.PriorityClass);
        }


        public static void Threads_managment(Process process, ThreadPriorityLevel Threads_pri = ThreadPriorityLevel.Normal)
        {
            for (int i = 0; i < process.Threads.Count; i++) // a for loop is better than foreach in terms of real-time performance 
            {
                process.Threads[i].PriorityLevel = Threads_pri;
                Console.WriteLine("Thread ID: " + process.Threads[i].Id + " Priority: " + process.Threads[i].PriorityLevel);
            }
        }

        public static void manage_thread(Process process, int ThreadID, [Optional] ThreadPriorityLevel thread_pri, [Optional] IntPtr thread_aff_mask)
        {
            for (int i = 0; i < process.Threads.Count; i++)
            {
                if (process.Threads[i].Id == ThreadID)
                {
                    process.Threads[i].ProcessorAffinity = thread_aff_mask;
                    process.Threads[i].PriorityLevel = thread_pri;
                    Console.WriteLine("Thread ID: " + process.Threads[i].Id + " Priority: " + process.Threads[i].PriorityLevel);
                }
            }
        }
    }
}
