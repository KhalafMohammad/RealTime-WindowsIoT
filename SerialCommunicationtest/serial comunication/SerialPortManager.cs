using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace WinSerialCommunication
{
    public class SerialPortManager
    {
        private SerialPort _serialPort;
        private readonly object _lock = new object();

        public SerialPortManager(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();
        }

        public async Task WriteAsync(string data)
        {
            lock (_lock)
            {
                _serialPort.WriteLine(data);
            }
        }

        public async Task<string> ReadAsync()
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    byte[] buffer = new byte[_serialPort.BytesToRead]; // create a buffer to store the data
                    _serialPort.Read(buffer, 0, buffer.Length);
                    return System.Text.Encoding.UTF8.GetString(buffer); // convert buffer to string
                }
            });
        }

        public void StartListening()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        string data = ReadAsync().Result;
                        Console.WriteLine($"Received: {data}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            });
        }
    }
}