using System;
using System.IO;
using System.Linq;

namespace BitBiter.IO
{
	public class BigEndianWriter : BinaryWriter
	{
		public BigEndianWriter(Stream output) : base(output) { }

		private void WriteBigEndian(byte[] bytes)
		{
			if (BitConverter.IsLittleEndian)
			{
				bytes = bytes.Reverse().ToArray();
			}

			Write(bytes);
		}

		public override void Write(short value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(ushort value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(int value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(uint value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(long value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(ulong value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(float value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}

		public override void Write(double value)
		{
			WriteBigEndian(BitConverter.GetBytes(value));
		}
	}
}
