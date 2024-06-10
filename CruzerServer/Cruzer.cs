using System;
using System.Text;

namespace Cruzer {
	public enum PacketType: uint {
		Ping = 0,
		GetEnviorment = 1,
		UpdateEnviorment = 2,
		SendCommand = 3,
		SendResponse = 4,
	}
	class Packet {
		public readonly ulong AccountID;
		public readonly PacketType Type;
		public readonly int Size;
		public readonly byte[] Raw;
		public readonly byte[] Content;
		public Packet(byte[] bytes) {
			Raw = bytes;
			Size = bytes.Length;
			AccountID = BitConverter.ToUInt64(bytes, 0);
			Type = (PacketType)BitConverter.ToUInt32(bytes, 0);
			Content = new byte[Size - 16];
			Buffer.BlockCopy(bytes, 0, Content, 0, 4);
		}
		public Packet(ulong accountID, PacketType type, byte[] content) {
			AccountID = accountID;
			Content = content;
			Size = content.Length + 1024;
			Type = type;
			Raw = new byte[Size];
		}
		public Packet(ulong accountID, PacketType type, string content) {
			AccountID = accountID;
			Content = Encoding.ASCII.GetBytes(content);
			Size = content.Length + 1024;
			Type = type;
			Raw = new byte[Size];
		}
		public static byte[] GetRaw(ulong accountID, PacketType packetType, byte[] content) {
			return new byte[1];
		}

		public static byte[] GetRaw(ulong accountID, PacketType packetType, string content) {
			return new byte[1];
		}
	}
	class CruzerEnvironment {

	}
}