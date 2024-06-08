using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Cruzer;
using Modules;

namespace Client {
	class Program {
		// Main Method
		static void Main(string[] args)
		{
			string currentDirectory = Environment.CurrentDirectory;
			IniFile settings = new(currentDirectory + "\\Settings.ini");
			if (args.Length == 0) {
				Console.WriteLine("     ******     ");
				Console.WriteLine("  ************  ");
				Console.WriteLine(" *****    ***** ");
				Console.WriteLine("****        ****");
				Console.WriteLine("****    \\\\\\\\   ");
				Console.WriteLine(" *****   \\\\\\\\   ");
				Console.WriteLine("  ******  \\\\\\\\  ");
				Console.WriteLine("     ***   \\\\\\\\ ");
				Console.WriteLine();
				Sequence loading = new("Bruh", "SIGMA!!!");
				Console.ReadKey();
				loading.End("Sadge!");
				loading = new("Bruh", "SIGMA!!!");
				Console.ReadKey();
				loading.End("Sadge!");
				loading = new("Bruh", "SIGMA!!!");
				Console.ReadKey();
				loading.End("Sadge!");
				loading = new("Bruh", "SIGMA!!!");
				Console.ReadKey();
				loading.End("Sadge!");
				Thread.Sleep(500);
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