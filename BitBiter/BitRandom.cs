using System;

namespace BitBiter
{
	[Serializable]
	public class BitRandom
	{
		private const int MBIG = 2147483647;
		private const int MSEED = 161803398;
		private const int MZ = 0;

		private int inext;
		private int inextp;
		private readonly int[] SeedArray = new int[56];

		public BitRandom() : this(Environment.TickCount)
		{
		}

		public BitRandom(int Seed)
		{
			int num = (Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed);
			int num2 = MSEED - num;
			SeedArray[55] = num2;
			int num3 = 1;
			for (int i = 1; i < 55; i++)
			{
				int num4 = 21 * i % 55;
				SeedArray[num4] = num3;
				num3 = num2 - num3;
				if (num3 < MZ)
				{
					num3 += int.MaxValue;
				}
				num2 = SeedArray[num4];
			}
			for (int j = 1; j < 5; j++)
			{
				for (int k = 1; k < 56; k++)
				{
					SeedArray[k] -= SeedArray[1 + (k + 30) % 55];
					if (SeedArray[k] < MZ)
					{
						SeedArray[k] += int.MaxValue;
					}
				}
			}
			inext = 0;
			inextp = 21;
		}

		#region Samples

		private double GetSampleForLargeRange()
		{
			int num = InternalSample();
			if (InternalSample() % 2 == MZ)
			{
				num = -num;
			}
			return (num + 2147483646.0) / 4294967293.0;
		}

		protected double Sample()
		{
			return InternalSample() * 4.6566128752457969E-10;
		}

		private int InternalSample()
		{
			int num = inext;
			int num2 = inextp;
			if (++num >= 56)
			{
				num = 1;
			}
			if (++num2 >= 56)
			{
				num2 = 1;
			}
			int num3 = SeedArray[num] - SeedArray[num2];
			if (num3 == MBIG)
			{
				num3--;
			}
			if (num3 < MZ)
			{
				num3 += int.MaxValue;
			}
			SeedArray[num] = num3;
			inext = num;
			inextp = num2;
			return num3;
		}

		#endregion

		public virtual void NextBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = (byte)(InternalSample() % 256);
			}
		}

		public bool Bool()
		{
			return Sample() < 0.5;
		}

		#region Byte

		public byte Byte()
		{
			return (byte)(InternalSample() % 256);
		}

		public byte Byte(byte max)
		{
			return (byte)Int32(max);
		}

		public byte Byte(byte min, byte max)
		{
			return (byte)Int32(min, max);
		}

		#endregion

		#region Int16

		public short Int16()
		{
			return (short)InternalSample();
		}

		public short Int16(short max)
		{
			return (short)Int32(max);
		}

		public short Int16(short min, short max)
		{
			return (short)Int32(min, max);
		}

		#endregion

		#region Int32

		public int Int32()
		{
			return InternalSample();
		}

		public int Int32(int maxValue)
		{
			return (int)(Sample() * maxValue);
		}

		public int Int32(int minValue, int maxValue)
		{
			long num = maxValue - (long)minValue;
			if (num <= 2147483647L)
			{
				return (int)(Sample() * num) + minValue;
			}
			return (int)((long)(GetSampleForLargeRange() * num) + minValue);
		}

		#endregion

		#region Int64

		public long Int64()
		{
			return (long)(GetSampleForLargeRange() * long.MaxValue);
		}

		public long Int64(long max)
		{
			return (long)(GetSampleForLargeRange() * max);
		}

		public long Int64(long min, long max)
		{
			long num = max - min;
			if (num <= 2147483647L)
			{
				return (int)(Sample() * num) + min;
			}
			return (long)(GetSampleForLargeRange() * num) + min;
		}

		#endregion

		#region Single

		public float Single()
		{
			return (float)Sample();
		}

		public float Single(float max)
		{
			return (float)Sample() * max;
		}

		public float Single(float min, float max)
		{
			return (float)Sample() * (max - min) + min;
		}

		#endregion

		#region Decimal

		public decimal Decimal()
		{
			return (decimal)Sample();
		}

		public decimal Decimal(decimal max)
		{
			return (decimal)Sample() * max;
		}

		public decimal Decimal(decimal min, decimal max)
		{
			return (decimal)Sample() * (max - min) + min;
		}

		#endregion

		#region Double

		public double Double()
		{
			return Sample();
		}

		public double Double(double max)
		{
			return Sample() * max;
		}

		public double Double(double min, double max)
		{
			return Sample() * (max - min) + min;
		}

		#endregion
	}
}
