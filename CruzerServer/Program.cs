// A C# Program for Server

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CruzerServer {
 
	class Program {
	
		static void Main(string[] args)
		{
			ExecuteServer();
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
	
		public static void ExecuteServer()
		{
			// Establish the local endpoint 
			// for the socket. Dns.GetHostName
			// returns the name of the host 
			// running the application.
			IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
			// Creation TCP/IP Socket using 
			// Socket Class Constructor
			Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try {
				// Using Bind() method we associate a
				// network address to the Server Socket
				// All client that will connect to this 
				// Server Socket must know this network
				listener.Bind(localEndPoint);
				listener.Listen(10);
				while (true) {
					Print("Awaiting connection ... ");
					Socket clientSocket = listener.Accept();
                    byte[] bytes = new byte[1024];
                    string data = "";
					while (data != "CLOSE<EOF>") {
						data = "";
						while (true) {
							int numByte = clientSocket.Receive(bytes);
							data += Encoding.ASCII.GetString(bytes, 0, numByte);
							if (data.IndexOf("<EOF>") > -1) {
								break;
							}
						}
						if (clientSocket.RemoteEndPoint is IPEndPoint remoteEndPoint) {
						Print($"Recieved \"{data}\"", address: remoteEndPoint.Address);
						clientSocket.Send(Encoding.ASCII.GetBytes("Test Server"));
					}
					}
					clientSocket.Shutdown(SocketShutdown.Both);
					clientSocket.Close();
				}
			}
			catch (Exception exception) {
				Print(exception.ToString());
			}
		}
	}
}