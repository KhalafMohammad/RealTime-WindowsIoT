using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WinSerialCommunication
{

    class UDPServer
    {
        public void server(double kine_data)
        {
            int port = 8080;  // Port to listen on
            UdpClient udpServer = new UdpClient(port);

            Console.WriteLine($"Listening on port {port}...");
            

            while (true)
            {
                // Receive bytes from any client
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                byte[] data = udpServer.Receive(ref remoteEP);

                // Convert bytes to string
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine($"Received message: {message} from {remoteEP}");


                
                byte[] responseData = Encoding.UTF8.GetBytes(kine_data.ToString());
                udpServer.Send(responseData, responseData.Length, remoteEP);
            }
        }

        public void client(double message)
        {
            UdpClient udpClient = new UdpClient();

            byte[] data = Encoding.UTF8.GetBytes(message.ToString());

            // Send data to Python server on localhost, port 8080
            udpClient.Send(data, data.Length, "127.0.0.1", 8080);

            Console.WriteLine("Message sent from C# client.");

            // Optionally, receive response
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8080);
            byte[] receivedData = udpClient.Receive(ref remoteEP);
            string receivedMessage = Encoding.UTF8.GetString(receivedData);
            Console.WriteLine($"Received response: {receivedMessage}");
        }
    }
}