using System;
using System.IO;
using System.Linq;

namespace BitBiter.IO
{
	public class BigEndianReader : BinaryReader
	{
		public BigEndianReader(Stream input) : base(input) { }

		private byte[] ReadBigEndian(int len)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return ReadBytes(len);
			}

			return ReadBytes(len).Reverse().ToArray();
		}

		public override short ReadInt16()
		{
			return BitConverter.ToInt16(ReadBigEndian(2), 0);
		}

		public override ushort ReadUInt16()
		{
			return BitConverter.ToUInt16(ReadBigEndian(2), 0);
		}

		public override int ReadInt32()
		{
			return BitConverter.ToInt32(ReadBigEndian(4), 0);
		}

		public override uint ReadUInt32()
		{
			return BitConverter.ToUInt32(ReadBigEndian(4), 0);
		}

		public override long ReadInt64()
		{
			return BitConverter.ToInt64(ReadBigEndian(8), 0);
		}

		public override ulong ReadUInt64()
		{
			return BitConverter.ToUInt64(ReadBigEndian(8), 0);
		}

		public override float ReadSingle()
		{
			return BitConverter.ToSingle(ReadBigEndian(4), 0);
		}

		public override double ReadDouble()
		{
			return BitConverter.ToDouble(ReadBigEndian(8), 0);
		}
	}
}
