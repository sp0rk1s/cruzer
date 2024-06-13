using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Cruzer;
using System.Text;

namespace Client {
	class Networking {
		public static int headerSize = 1024;
		public static void Print(string message, IPAddress? address = null) {
			DateTime now = DateTime.Now;
			Console.Write(now.ToString("dd:MM:yy HH:mm:ss") + " [ ");
			if (address == null) {
				Console.Write("    :    :    :    :    :    :    :    ");
			} else {
				string[] splitAddress = address.MapToIPv6().ToString().Split("%")[0].Split(":");
				int length = splitAddress.Length;

				// Adding colors to the address
				// Buffer
				for (int i = 0; i < 3 - (length - 5); i++) {
					Console.Write("----:");
				}
				// Routing Prefix
				for (int i = 0; i < length - 5; i++) {
					Console.ForegroundColor = ConsoleColor.Magenta;
					Console.Write($"{splitAddress[i].PadLeft(4, '0')}");
					Console.ResetColor();
					Console.Write(":");
				}
				// Subnet ID
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.Write($"{splitAddress[length - 5].PadLeft(4, '0')}");
				Console.ResetColor();
				// Interface ID
				for (int i = splitAddress.Length - 4; i < splitAddress.Length; i++) {
					Console.Write(":");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($"{splitAddress[i].PadLeft(4, '0')}");
					Console.ResetColor();
				}
			}
			Console.WriteLine(" ] " + message);
		}
		public static IPAddress GetFirstIpAddress() {
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress iPAddress in host.AddressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return iPAddress;
				}
			}
			throw new Exception("Failed to get local IPAddress");
		}
		public static Exception? PingNetwork(IPAddress iPAddress, UInt16 port = 11111, IPEndPoint? localIPEndPoint = null) {
			Socket socket = new(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try {
				localIPEndPoint ??= new(GetFirstIpAddress(), port);
			} catch { return new Exception("Failed to get local IPAddress"); }
			try {
				socket.Connect(localIPEndPoint);
			} catch { return new Exception("Failed to connect to server"); }
			string message = ">>ping";
			socket.Send(Encoding.ASCII.GetBytes(message));
			byte[] recievedBytes = new byte[1024];
			string data = "";
			while (true) {
				int recievedBytesLength = socket.Receive(recievedBytes);
				data += Encoding.ASCII.GetString(recievedBytes, 0, recievedBytesLength);
				if (data.Last() == '\0') {
					break;
				}
			}
			return null;
		}
		public static void ExecuteClient()
		{
			try {
				IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress ipAddress = ipHost.AddressList[0];
				IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11111);
				Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				try {
					sender.Connect(localEndPoint);
					Print("Connected", address: ipAddress);

					string input = "";
					while (input != "shutdown") {
						input = Console.ReadLine() + "<EOF>" ?? "Test<EOF>";
						
						sender.Send(Encoding.ASCII.GetBytes(input));
						Print($"Sent \"{input}\"", address: ipAddress);

						byte[] messageReceived = new byte[1024];
						int byteRecv = sender.Receive(messageReceived);
						Print(Encoding.ASCII.GetString(messageReceived, 0, byteRecv), address: ipAddress);
					}
					sender.Send(Encoding.ASCII.GetBytes("CLOSE<EOF>"));
					Print($"Disonnected", address: ipAddress);
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();

				// Exceptions
				} catch (ArgumentNullException exception) {
					Console.WriteLine("ArgumentNullException : {0}", exception.ToString());
				} catch (SocketException exception) {
					Console.WriteLine("SocketException : {0}", exception.ToString());
				} catch (Exception exception) {
					Console.WriteLine("Unexpected exception : {0}", exception.ToString());
				}
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
			Console.ReadLine();
		}
	}
}