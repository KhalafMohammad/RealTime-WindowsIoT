# RealTime-WindowsIoT

## Overview

RealTime-WindowsIoTis a C# project designed to control a two-axis robotic arm via serial communication. The project includes functionalities for calculating kinematics, generating S-curve motion profiles, managing real-time process priorities, and transmitting control packets to the robot's motors. It is built using .NET 8 and C# 12.0.
youtube link video [Robot running in REALTIME on Windows]()
## Features

- **Inverse and Forward Kinematics**: Calculate motor angles for given coordinates and compute coordinates from motor angles.
- **S-Curve Motion Profiling**: Generate smooth acceleration and deceleration curves for motor movements.
- **Serial Communication**: Initialize and manage serial port communication to interface with the robotic arm.
- **UDP Communication**: Send and receive data over UDP for networked control or monitoring.
- **Real-Time Processing**: Manage process and thread priorities to ensure real-time performance.
- **Error Compensation**: Detect and compensate for positional errors in the motors.

## Use Cases

1. **Robotic Arm Control**: Control a two-axis robotic arm by specifying (x, y) coordinates, which are converted to motor angles and steps.
2. **Serial Communication Interface**: Communicate with hardware components (e.g., motors, controllers) via serial ports.
3. **Motion Profiling**: Implement smooth motion transitions for robotic movements using S-curve profiles.
4. **Real-Time Applications**: Adjust process and thread priorities for applications requiring real-time performance.
5. **Network Communication**: Send and receive data over UDP for remote control or data acquisition.

## Getting Started

### Prerequisites

- Visual Studio 2022 or later
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Installation

1. **Clone the Repository**
git clone https://github.com/yourusername/WinSerialCommunication.git


2. **Open the Solution**

   Open `WinSerialCommunication.sln` in Visual Studio 2022.

### Configuration

- **Project Targeting**

  The project targets **.NET 8**. Ensure that you have the .NET 8 SDK installed and selected in your project properties.

- **Serial Port Configuration**

  Ensure that the serial port specified in `Serial_Init.cs` matches the port connected to your robotic arm hardware.

  public static SerialPort sp = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);
- **Processor Affinity and Priority (Optional)**

  Adjust process priority and processor affinity in `RealTime.cs` if needed for your system.

## Usage

1. **Initialize the Serial Connection**

   The serial port is automatically initialized when the application starts.

2. **Create a Robot Instance**

   Robot robot = new Robot();
3. **Set Coordinates and Run**
	```robot.coordinates(13.60, 16); // Set target coordinates
	robot.Run(); // Calculate and execute motor movements```


4. **Error Handling**

   The application includes error handling for motor angles out of range and positional errors.
	```try {
		robot.coordinates(x, y);
		robot.Run(); 
	    } catch (MotorAngleException ex)
            { Console.WriteLine(ex.Message); }```

## Project Structure

### Files and Classes

- **Program.cs**

  The entry point of the application. Manages serial communication events and initializes the robot control sequence.

- **Robot.cs**

  Contains the `Robot` class, which manages kinematics calculations, motor angle conversions, and execution of movements.

- **Kinematics.cs**

  Contains the `TwoAxisRobot` class, which performs inverse and forward kinematics calculations for a two-axis robotic arm.

- **Scurvev3f.cs**

  Contains the `Scurve` class, which generates S-curve motion profiles for smooth motor movements.

- **Serial_Init.cs**

  Manages the initialization and configuration of the serial port used for communication with the robotic arm.

- **PacketList.cs**

  Handles the creation and transmission of control packets to the motors, including combining motor values and directions into byte arrays.

- **RealTime.cs**

  Provides methods for managing process and thread priorities to achieve real-time performance.

- **UdpConnect.cs**

  Contains the `UDPServer` class for sending and receiving data over UDP, which can be used for networked control or monitoring.

## Classes Overview

### Robot

- **Properties**

  - `Motor1_angle`: Angle for motor 1.
  - `Motor2_angle`: Angle for motor 2.

- **Methods**

  - `coordinates(double x, double y)`: Calculates motor angles based on target coordinates.
  - `Run()`: Executes motor movements based on calculated angles.
  - `Angle_to_steps(int angle)`: Converts an angle to motor steps.
  - `calculate_time(int steps)`: Calculates time parameters for motor movement.

### TwoAxisRobot

- **Properties**

  - `L1`, `L2`, `D`: Physical dimensions of the robotic arm.
  - `motor1_position`, `motor2_position`: Current motor positions.

- **Methods**

  - `CalculateInverseKinematics(double Xi, double Yi)`: Calculates motor angles for given (x, y) coordinates.
  - `get_xy(double angle1, double angle2)`: Computes (x, y) coordinates from motor angles.
  - `degrees(double radians)`, `radians(double degrees)`: Conversion utilities.

### Scurve

- **Purpose**

  Generates S-curve profiles for motor acceleration and deceleration, providing smoother motor movements.

- **Methods**

  - `Get_curve_values()`: Returns frequency values for the S-curve profile.

### PacketList

- **Purpose**

  Manages the packetization of motor control data for transmission over serial communication.

- **Methods**

  - `Test(int[] motor1_values, char motor1_dir, int[] motor2_values, char motor2_dir)`: Combines motor values and initiates transmission.
  - `Combine_values(...)`: Combines motor values and directions into a byte array.
  - `Transmit(byte[] combined_values_array)`: Sends control packets to the robotic arm.

### Serial_Init

- **Purpose**

  Initializes and manages the serial port communication settings.

- **Methods**

  - `serial_init()`: Configures and opens the serial port.

### RealTime

- **Purpose**

  Adjusts process and thread priorities for improved real-time performance.

- **Methods**

  - `Process_managment(...)`: Sets process priority class and processor affinity.
  - `Threads_managment(...)`: Sets thread priority levels.

### UDPServer

- **Purpose**

  Provides UDP server and client functionalities for network communication.

- **Methods**

  - `server(double kine_data)`: Starts a UDP server listening for incoming messages.
  - `client(string message)`: Sends messages to a UDP server.


## Acknowledgments

- Thanks to all contributors who have helped improve this project.
- This project utilizes .NET 8 and C# 12.0 features.

---

*Note: Ensure that all hardware connections and serial port configurations are properly set up before running the application to prevent any hardware damage.*
