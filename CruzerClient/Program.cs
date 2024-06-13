using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Cruzer;
using Modules;

namespace CruzerClient {
	class Program {
		// Main Method
		static void Main(string[] args)
		{
			string currentDirectory = Environment.CurrentDirectory;
			if (args.Contains("")) {}
			if (args.Length == 0) {
				Thread.Sleep(500);

				// Importing settings
				Sequence sequence = new("Importing settings...", "Settings.ini");
				IniFile settings = new(currentDirectory + "\\Settings.ini");
				if (settings.content.Count == 0) {
					sequence.End("Settings.ini is invalid", endType: "failed");
					Console.Read();
					Environment.Exit(1);
				}
				try { IPAddress.Parse(settings.content["defaultAddress"]); } catch {
					sequence.End("Settings.ini:defaultAddress is invalid", endType: "depend");
					Console.Read();
					Environment.Exit(1);
				}
				Thread.Sleep(100);
				sequence.End("Settings imported.");

				// Establish connection
				sequence = new("Establishing connection...", "Pinging");
				Exception? exception = Networking.PingNetwork(IPAddress.Parse(settings.content["defaultAddress"]));
				sequence = new("Establishing connection...", "Pinging");
				Thread.Sleep(100);
				sequence.End("Settings imported.");
				Console.Clear();
				if (settings.content.TryGetValue("clientVersion", out string? value)) {
					Console.WriteLine("Version . . . . . . . . . " + settings.content["clientVersion"]);
				} else {
					Console.WriteLine("Version . . . . . . . . . alpha (modified)");
				}
				Console.ReadLine();
				Networking.ExecuteClient();
			} else {
				if (args[0] == "defaultip") {
					if (args[1] != null) {
						
					}
				}
			}
		}
	}
}