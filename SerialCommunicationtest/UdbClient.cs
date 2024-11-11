//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net.Sockets;

//namespace WinSerialCommunication
//{
//    public class UdpClient
//    {
//        public readonly int port;
//        public readonly string host;
//        public UdpClient(int port)
//        {
//            this.port = port;
//        }
//        public static async void SendMessage(string message, string host = "127.0.0.1", int port = 12345)
//        {
//            using (var client = new System.Net.Sockets.UdpClient())
//            {
//                try
//                {
//                    // Send data
//                    var endpoint = new IPEndPoint(IPAddress.Parse(host), port);
//                    byte[] data = Encoding.UTF8.GetBytes(message);
//                    await client.SendAsync(data, data.Length, endpoint);

//                    // Receive response
//                    var result = await client.ReceiveAsync();
//                    string response = Encoding.UTF8.GetString(result.Buffer);
//                    Console.WriteLine($"Received response: {response}");
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine($"Error: {e.Message}");
//                }
//            }
//        }
//    }
//}
