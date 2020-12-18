using System;

namespace VisualBoy.Core.Extensions
{
	public static class ValueExtensions
	{
		private static readonly int CHAR_BIT = Environment.Is64BitOperatingSystem ? 64 : 32;

		public static uint Abs(this int val)
		{
			int mask = val >> sizeof(int) * CHAR_BIT - 1;
			return (uint)((val ^ mask) - mask);
		}

		public static bool Between(this object obj, dynamic low, dynamic high)
		{
			if (!obj.GetType().IsValueType)
			{
				int hash = obj.GetHashCode();
				return hash >= low && hash <= high;
			}

			if (obj is float f)
			{
				return f >= low && f <= high;
			}
			else if (obj is long l)
			{
				return l >= low && l <= high;
			}

			return (double)obj >= low && (double)obj <= high;
		}

		public static bool Equals(this object obj, params dynamic[] vals)
		{
			if (!obj.GetType().IsValueType)
			{
				int hash = obj.GetHashCode();

				foreach (dynamic val in vals)
				{
					if (val == hash)
					{
						return true;
					}
				}
			}

			if (obj is float l)
			{
				foreach (dynamic val in vals)
				{
					if (val == l)
					{
						return true;
					}
				}
			}
			else if (obj is long d)
			{
				foreach (dynamic val in vals)
				{
					if (val == d)
					{
						return true;
					}
				}
			}

			foreach (dynamic val in vals)
			{
				if (val == (double)obj)
				{
					return true;
				}
			}

			return false;
		}
	}
}
