using System;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;

namespace Cruzer {
	class Settings {
		public static Encoding EncodingScheme = Encoding.ASCII;
		public static int MaxPacketSize = 4096;
	}
	public enum PacketType: uint {
		Null = 0,
		Ping = 1,
		Connect = 2,
		Disconnect = 3,
		Heartbeat = 4,
		Settings = 5,
		GetEnviorment = 6,
		UpdateEnviorment = 7,
		SendCommand = 8,
		SendResponse = 9,
	}
	class Packet {
		public PacketHeader Header;
		public byte[] Content;

		/// <summary>
		/// Generates a <c>packet</c> normally.
		/// </summary>
		public Packet(PacketHeader header, byte[] content) {
			Header = header;
			Content = content;
		}

		/// <summary>
		/// Decodes a <c>packet</c> from a byte array.
		/// </summary>
		public Packet(byte[] bytes) {
			Header = new(bytes.Take(512).ToArray());
			Content = bytes.Skip(512).Take(bytes.Length - 512).ToArray();
		}

		/// <summary>
		/// Generates empty <c>packet</c>.
		/// </summary>
		public Packet() {
			Header = new();
			Content = [];
		}

		/// <summary>
		/// Recieves and constructs a <c>packet</c> from the provided <c>socket</c>.
		/// </summary>
		public Packet(Socket socket) {
			byte[] data = new byte[Settings.MaxPacketSize];
			int totalLength = 0;

			// Recieving Header
			while (totalLength < 512) {
				byte[] bytes = new byte[Settings.MaxPacketSize + 512];
				int bytesLength = socket.Receive(bytes);
				Buffer.BlockCopy(bytes, 0, data, totalLength, bytesLength);
				totalLength += bytesLength;
			}
			Header = new(data.Take(512).ToArray());

			// Recieving content
			while (totalLength <= Header.Size + 512) {
				byte[] bytes = new byte[Settings.MaxPacketSize + 512];
				int bytesLength = socket.Receive(bytes);
				Buffer.BlockCopy(bytes, 0, data, totalLength, bytesLength);
				totalLength += bytesLength;
			}
			Content = [];
			Buffer.BlockCopy(data, 0, Content, 0, Header.Size);
		}
		
		/// <summary>
		/// Send a <c>packet</c> using the provided <c>socket</c>. 
		/// </summary>
		public void Send(Socket socket) {
			socket.Send(ToByte());
		}
		
		/// <summary>
		/// Encodes a <c>header</c> and <c>content</c> into a byte array.
		/// </summary>
		public static byte[] GetRaw(PacketHeader header, byte[] content) {
			return new Packet(header, content).ToByte();
		}
		
		/// <summary>
		/// Encodes a <c>packet</c> into a byte array.
		/// </summary>
		public byte[] ToByte() {
			byte[] raw = new byte[Content.Length + 512];
			Buffer.BlockCopy(Content, 0, raw, 512, Content.Length);
			Buffer.BlockCopy(Header.ToByte(), 0, raw, 512, Content.Length);
			return raw;
		}
		
		/// <summary>
		/// Returns a string that represent a packet.<br/>
		/// Decodes the content and header attributes.
		/// <br/><br/>
		/// Returns:<br/>
		///   A string that represent a packet.
		/// </summary>
		public override string ToString(){
			string result = $"AccountID: {Header.AccountID}\nPacketSize: {Header.Size}\nPacketNumber: {Header.Number}\nPacketType: {Header.Type}\nRespond: {Header.Respond}\nIgnoreAccount: {Header.IgnoreAccount}";
			foreach (short key in Header.Attributes.Keys) {
				result += key + ": " + Header.Attributes[key];
			}
			result += "\nContent: " + (Encoding.ASCII).GetString(Content);
			return result;
		}
		
		/// <summary>
		/// Returns a string that represent a packet.<br/>
		/// Decodes the content and header attributes.
		/// <br/><br/>
		/// Returns:<br/>
		///   A string that represent a packet.
		/// </summary>
		public string ToString(Encoding contentEncoder, Encoding? attributeEncoder = null){
			string result = Header.ToString(attributeEncoder);
			foreach (short key in Header.Attributes.Keys) {
				result += key + ": " + contentEncoder.GetString(BitConverter.GetBytes(Header.Attributes[key]));
			}
			result += "\nContent: " + (contentEncoder ?? Encoding.ASCII).GetString(Content);
			return result;
		}
	}
	class PacketHeader {
		/* RAW FORMATTING
		22 Byte 		Basic Data
			8 Byte		Account ID
			4 Byte		Packet Size
			4 Byte		Packet Number
			4 Byte		Packet Type
			1 Byte		Respond (If false server wont respond)
			1 Byte		Ignore Account (If true server will ignore AccountID)

		490 Byte		49 Custom Attributes
			2 Byte		Attribute key
			8 Byte		Attribute value
		*/
		public ulong AccountID;
		public int Size;
		public uint Number;
		public PacketType Type;
		public bool Respond;
		public bool IgnoreAccount;
		public Dictionary<short, long> Attributes;
		public PacketHeader(ulong accountID, int packetSize, ushort packetNumber, PacketType packetType, bool respond, bool ignoreAccount, Dictionary<short, long>? attributes = null) {
			AccountID = accountID;
			Size = packetSize;
			Number = packetNumber;
			Type = packetType;
			Respond = respond;
			IgnoreAccount = ignoreAccount;
			Attributes = attributes ?? [];
        }
		public PacketHeader(byte[] raw) {
			AccountID = BitConverter.ToUInt64(raw, 0);
			Size = BitConverter.ToInt32(raw, 8);
			Number = BitConverter.ToUInt32(raw, 12);
			Type = (PacketType)BitConverter.ToUInt32(raw, 16);
			Respond = BitConverter.ToBoolean(raw, 20);
			IgnoreAccount = BitConverter.ToBoolean(raw, 21);
			Attributes = [];
			int valueNumber = 0;
			while (valueNumber < 49) {
				short key = BitConverter.ToInt16(raw, 22 + valueNumber * 10);
				if (key == 0) {
					break;
				}
				Attributes.Add(key, BitConverter.ToInt64(raw, 24 + valueNumber * 10));
			}
        }
		public PacketHeader() {
			AccountID = 0;
			Size = 0;
			Number = 0;
			Type = PacketType.Null;
			Attributes = [];
        }
		public static byte[] GetRaw(ulong accountID, int packetSize, ushort packetNumber, PacketType packetType, bool respond, bool ignoreAccount, Dictionary<short, long>? attributes) {
			PacketHeader header = new(accountID, packetSize, packetNumber, packetType, respond, ignoreAccount, attributes);
			return header.ToByte();
		}
		public byte[] ToByte() {
			byte[] raw = new byte[512];
			Buffer.BlockCopy(BitConverter.GetBytes(AccountID), 0, raw, 0, 8);
			Buffer.BlockCopy(BitConverter.GetBytes(Size), 0, raw, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(Number), 0, raw, 12, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((ushort)Type), 0, raw, 16, 4);
			raw[21] = Convert.ToByte(Respond);
			raw[22] = Convert.ToByte(IgnoreAccount);
			if (Attributes != null) {
				int offset = 23;
				foreach (short key in Attributes.Keys) {
					Buffer.BlockCopy(BitConverter.GetBytes(key), 0, raw, offset, 2);
					Buffer.BlockCopy(BitConverter.GetBytes(Attributes[key]), 0, raw, offset + 2, 8);
					offset += 10;
				}
			}
			return raw;
		}
		public string ToString(Encoding? attributeEncoder = null){
			string result = $"AccountID: {AccountID}\nPacketSize: {Size}\nPacketNumber: {Number}\nPacketType: {Type}\nRespond: {Respond}\nIgnoreAccount: {IgnoreAccount}";
			if (attributeEncoder != null) {
				foreach (short key in Attributes.Keys) {
					result += key + ": " + attributeEncoder.GetString(BitConverter.GetBytes(Attributes[key]));
				}
			} else {
				foreach (short key in Attributes.Keys) {
					result += key + ": " + Attributes[key];
				}
			}
			return result;
		}
	}
	class CruzerEnvironment {

	}
}