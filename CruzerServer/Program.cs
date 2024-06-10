// A C# Program for Server

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cruzer;

namespace Server {
	class Program {

		static void Main(string[] args)
		{
			Print(args.ToString() ?? "Bruh");
			if (args.Length == 0) {
				ExecuteServer();
			}
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

		private static void Print(string message, IPAddress? address = null) {
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

		public static void HandleClient(Socket socket) {
            if (socket.RemoteEndPoint is not IPEndPoint remoteIPEndPoint) {
				return;
			}
            Print("Connected", address: remoteIPEndPoint.Address);
			while (true) {
				byte[] bytes = new byte[1024];
				string data = "";
				while (data != "CLOSE<EOF>") {
					data = "";
					while (true) {
						int bytesLength = socket.Receive(bytes);
						data += Encoding.ASCII.GetString(bytes, 0, bytesLength);
						if (data.Last() == '\0') {
							break;
						}
					}
					if (socket.RemoteEndPoint is IPEndPoint remoteEndPoint) {
						Print($"Recieved \"{data}\"", address: remoteEndPoint.Address);
						socket.Send(Encoding.ASCII.GetBytes("Test Server"));
					}
				}
				Print("Disconnected");
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
		}
	
		public static void ExecuteServer()
		{
			IPAddress iPAddress = GetFirstIpAddress();
			IPEndPoint localEndPoint = new IPEndPoint(iPAddress, 11111);
			Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try {
				// Using Bind() method we associate a
				// network address to the Server Socket
				// All client that will connect to this 
				// Server Socket must know this network
				listener.Bind(localEndPoint);
				Print("Listening");
				listener.Listen();
				Print("Recieving request...");
				Thread thread = new(() => HandleClient(listener.Accept()));
			}
			catch (Exception exception) {
				Print(exception.ToString());
			}
		}
	}
}