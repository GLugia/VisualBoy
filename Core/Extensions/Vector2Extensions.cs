using Microsoft.Xna.Framework;
using System;

namespace VisualBoy.Core.Extensions
{
	public static class Vector2Extensions
	{
		public static Vector2 Abs(this Vector2 vec)
		{
			return new Vector2(Math.Abs(vec.X), Math.Abs(vec.Y));
		}

		public static float Distance(this Vector2 vec, Vector2 other)
		{
			return Vector2.Distance(vec, other);
		}

		public static float DistanceSquared(this Vector2 vec, Vector2 other)
		{
			return Vector2.DistanceSquared(vec, other);
		}

		public static Point ToTileCoordinates(this Vector2 vec)
		{
			return new Point((((int)vec.X >> 4) * 16) / 32, (((int)vec.Y >> 4) * 16) / 32);
		}

		public static Vector2 ToWorldCoordinates(this Point point)
		{
			return new Vector2(point.X * 32f, point.Y * 32f);
		}

		public static void RoundToTile(this ref Vector2 val)
		{
			val.X.RoundToTile();
			val.Y.RoundToTile();
		}

		public static void RoundToTile(this ref float val)
		{
			int a = (int)(val % 32);
			int b = (int)val >> 4;
			if (a > 15)
			{
				val = (b + 1) * 16f;

			}
			else
			{
				val = b * 16f;
			}
		}

		public static bool AnyEqual(this Vector2 vec, params float[] vals)
		{
			foreach (float val in vals)
			{
				if (vec.X == val || vec.Y == val)
				{
					return true;
				}
			}

			return false;
		}

		public static bool AnyEqual(this Point vec, params int[] vals)
		{
			foreach (int val in vals)
			{
				if (vec.X == val || vec.Y == val)
				{
					return true;
				}
			}

			return false;
		}

		public static bool AnyGreater(this Vector2 vec, params float[] vals)
		{
			foreach (float val in vals)
			{
				if (vec.X >= val || vec.Y >= val)
				{
					return true;
				}
			}

			return false;
		}

		public static bool AnyGreater(this Point vec, params int[] vals)
		{
			foreach (int val in vals)
			{
				if (vec.X >= val || vec.Y >= val)
				{
					return true;
				}
			}

			return false;
		}

		public static bool AnyLess(this Vector2 vec, params float[] vals)
		{
			foreach (float val in vals)
			{
				if (vec.X <= val || vec.Y <= val)
				{
					return true;
				}
			}

			return false;
		}

		public static bool AnyLess(this Point vec, params int[] vals)
		{
			foreach (int val in vals)
			{
				if (vec.X <= val || vec.Y <= val)
				{
					return true;
				}
			}

			return false;
		}

		public static bool AnyBetween(this Vector2 vec, float low, float high)
		{
			return vec.X.Between(low, high) || vec.Y.Between(low, high);
		}

		public static bool AnyBetween(this Point vec, int low, int high)
		{
			return vec.X.Between(low, high) || vec.Y.Between(low, high);
		}

		public static bool AnyModulo(this Vector2 vec, int divisor, int remainder)
		{
			return (vec.X % divisor == remainder) || (vec.Y % divisor == remainder);
		}

		public static Vector2 SafeNormalize(this Vector2 vec)
		{
			Vector2 nvec = vec;

			try
			{
				nvec.Normalize();
			}
			catch (Exception)
			{
				nvec = default;
			}

			if (float.IsNaN(nvec.X) || float.IsInfinity(nvec.X))
			{
				nvec.X = 0f;
			}

			if (float.IsNaN(nvec.Y) || float.IsInfinity(nvec.Y))
			{
				nvec.Y = 0f;
			}

			return nvec;
		}
	}
}
