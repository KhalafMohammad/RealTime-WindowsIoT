using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics = System.Diagnostics;

namespace WinSerialCommunication
{
    public class SerialPortManager
    {
        private SerialPort _serialPort;
        private static StringBuilder dataBuffer = new StringBuilder(); // create een nieuwe stringbuilder object

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
                        dataBuffer.Append(data);
                        string incoming_data = dataBuffer.ToString(); // get the data from the buffer

                        string[] data_parts = incoming_data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string part in data_parts)
                        {
                            string[] strings = part.Split(' ');
                            foreach (string str in strings)
                            {
                                if (int.TryParse(str, out int result))
                                {
                                    Console.WriteLine("Integer: " + result);
                                }
                                else
                                {
                                    Console.WriteLine("String: " + str);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            });
        }

    }



    /* This AX-Fast Serial Library
       Developer: Ahmed Mubarak - RoofMan

       This Library Provide The Fastest & Efficient Serial Communication
       Over The Standard C# Serial Component
    */
    public class DataStreamEventArgs : EventArgs
    {
        #region Defines
        private byte[] _bytes;
        #endregion

        #region Constructors
        public DataStreamEventArgs(byte[] bytes)
        {
            _bytes = bytes;
        }
        #endregion

        #region Properties
        public byte[] Response
        {
            get { return _bytes; }
        }
        #endregion
    }

    public class SerialClient : IDisposable
    {
        #region Defines
        private string _port;
        private int _baudRate;
        private SerialPort _serialPort;
        private Thread serThread;
        private double _PacketsRate;
        private DateTime _lastReceive;
        /*The Critical Frequency of Communication to Avoid Any Lag*/
        private const int freqCriticalLimit = 20;
        #endregion

        #region Constructors
        public SerialClient(string port)
        {
            _port = port;
            _baudRate = 9600;
            _lastReceive = DateTime.MinValue;

            serThread = new Thread(new ThreadStart(SerialReceiving));
            serThread.Priority = ThreadPriority.Normal;
            serThread.Name = "SerialHandle" + serThread.ManagedThreadId;
        }
        public SerialClient(string Port, int baudRate)
            : this(Port)
        {
            _baudRate = baudRate;
        }
        #endregion

        #region Custom Events
        public event EventHandler<DataStreamEventArgs> OnReceiving;
        #endregion

        #region Properties
        public string Port
        {
            get { return _port; }
        }
        public int BaudRate
        {
            get { return _baudRate; }
        }
        public string ConnectionString
        {
            get
            {
                return String.Format("[Serial] Port: {0} | Baudrate: {1}",
                    _serialPort.PortName, _serialPort.BaudRate.ToString());
            }
        }
        #endregion

        #region Methods
        #region Port Control
        public bool OpenConn()
        {
            try
            {
                if (_serialPort == null)
                    _serialPort = new SerialPort(_port, _baudRate, Parity.None);

                if (!_serialPort.IsOpen)
                {
                    _serialPort.ReadTimeout = -1;
                    _serialPort.WriteTimeout = -1;

                    _serialPort.Open();

                    if (_serialPort.IsOpen)
                        serThread.Start(); /*Start The Communication Thread*/
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public bool OpenConn(string port, int baudRate)
        {
            _port = port;
            _baudRate = baudRate;

            return OpenConn();
        }
        public void CloseConn()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                serThread.Abort();

                if (serThread.ThreadState == ThreadState.Aborted)
                    _serialPort.Close();
            }
        }
        public bool ResetConn()
        {
            CloseConn();
            return OpenConn();
        }
        #endregion
        #region Transmit/Receive
        public void Transmit(byte[] packet)
        {
            _serialPort.Write(packet, 0, packet.Length);
        }

        public void Transmit(string packet)
        {
            _serialPort.Write(packet);
        }
        public int Receive(byte[] bytes, int offset, int count)
        {
            int readBytes = 0;

            if (count > 0)
            {
                readBytes = _serialPort.Read(bytes, offset, count);
            }

            return readBytes;
        }
        #endregion
        #region IDisposable Methods
        public void Dispose()
        {
            CloseConn();

            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }
        #endregion
        #endregion

        #region Threading Loops
        private void SerialReceiving()
        {
            while (true)
            {
                int count = _serialPort.BytesToRead;

                /*Get Sleep Inteval*/
                TimeSpan tmpInterval = (DateTime.Now - _lastReceive);

                /*Form The Packet in The Buffer*/
                byte[] buf = new byte[count];
                int readBytes = Receive(buf, 0, count);

                if (readBytes > 0)
                {
                    OnSerialReceiving(buf);
                }

                #region Frequency Control
                _PacketsRate = ((_PacketsRate + readBytes) / 2);

                _lastReceive = DateTime.Now;

                if ((double)(readBytes + _serialPort.BytesToRead) / 2 <= _PacketsRate)
                {
                    if (tmpInterval.Milliseconds > 0)
                        Thread.Sleep(tmpInterval.Milliseconds > freqCriticalLimit ? freqCriticalLimit : tmpInterval.Milliseconds);

                    /*Testing Threading Model*/
                    Diagnostics.Debug.Write(tmpInterval.Milliseconds.ToString());
                    Diagnostics.Debug.Write(" - ");
                    Diagnostics.Debug.Write(readBytes.ToString());
                    Diagnostics.Debug.Write("\r\n");
                }
                #endregion
            }

        }
        #endregion

        #region Custom Events Invoke Functions
        private void OnSerialReceiving(byte[] res)
        {
            if (OnReceiving != null)
            {
                OnReceiving(this, new DataStreamEventArgs(res));
            }
        }
        #endregion
    }
}
