// A C# Program for Server
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
 
namespace Server {
 
class Program {
 
	static void Main(string[] args)
	{
		var stdout = Console.OpenStandardOutput();
		var console = new StreamWriter(stdout, Encoding.ASCII);
		console.AutoFlush = true;
		Console.SetOut(console);
		ExecuteServer();
	}

private static void print(string message, IPAddress? address = null, int color = 33) {
		DateTime now = DateTime.Now;
		string formattedAddress;
		if (address == null) {
			formattedAddress = "    :    :    :    :    :    ";
		} else {
			formattedAddress = address.MapToIPv6().ToString();
		}
		Console.WriteLine(now.ToString("dd:MM:yy HH:mm:ss") + " [ " + formattedAddress + " ] -> " + message);
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
		Socket listener = new Socket(ipAddr.AddressFamily,
					SocketType.Stream, ProtocolType.Tcp);
	
		try {
			
			// Using Bind() method we associate a
			// network address to the Server Socket
			// All client that will connect to this 
			// Server Socket must know this network
			// Address
			listener.Bind(localEndPoint);
	
			// Using Listen() method we create 
			// the Client list that will want
			// to connect to Server
			listener.Listen(10);
	
			while (true) {
				
				print("Awaiting connection ... ");
	
				// Suspend while waiting for
				// incoming connection Using 
				// Accept() method the server 
				// will accept connection of client
				Socket clientSocket = listener.Accept();
	
				// Data buffer
				byte[] bytes = new Byte[1024];
				string? data = null;
	
				while (true) {
					int numByte = clientSocket.Receive(bytes);
					data += Encoding.ASCII.GetString(bytes, 0, numByte);
					if (data.IndexOf("<EOF>") > -1)
						break;
				}
				if (clientSocket.RemoteEndPoint != null) {
					IPEndPoint iPEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
					print("Text received -> " + clientSocket.RemoteEndPoint.ToString() + data.ToString());
					byte[] message = Encoding.ASCII.GetBytes("Test Server");
					clientSocket.Send(message);
				}
				clientSocket.Shutdown(SocketShutdown.Both);
				clientSocket.Close();
			}
		}
		catch (Exception exception) {
			Console.WriteLine(exception.ToString());
		}
	}
}
}