using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

// Change this to match your program's normal namespace
namespace Cruzer
{
	class IniFile
	{
		public readonly string currentPath;
		public Dictionary<string, string> content;
		public IniFile(string path) {
			currentPath = path;
			StreamReader streamReader = new(path);
			content = [];
			while (true) {
				string? line = streamReader.ReadLine();
				if (line == null) {
					break;
				}
				if (line.Contains('=')) {
					string[] splitLine = line.Split('=');
					string key = RemoveWhitespace(splitLine[0]);
					string value = RemoveWhitespace(splitLine[1]);
					content.Add(key, value);
				}
			}
			streamReader.Close();
		}
		public bool Push(string path) {
			try {
				StreamWriter streamWriter = new(File.Open(path, FileMode.Create));
				try {
					foreach(KeyValuePair<string, string> entry in content) {
						streamWriter.WriteLine(entry.Key + " = " + entry.Value);
					}
					streamWriter.Close();
					return true;
				} catch {
					streamWriter.Close();
					return false;
				}
			} catch { return false; }
		}
		private static string RemoveWhitespace(string input) {
			int i;
			for (i = 0; input[i] == ' ' || input[i] == '\t';) {
				i++;
			}
			input = input.Remove(0, i);
			while (input.Length != 0 && (input.Last() == ' ' || input.Last() == '\t')) {
				input = input.Remove(input.Length - 1);
			}
			if ((input[0] == '\"' && input.Last() == '\"') || (input[0] == '\"' && input.Last() == '\"')) {
				return input.Remove(input.Length - 1).Remove(0);
			}
			return input;
		}
	}
}