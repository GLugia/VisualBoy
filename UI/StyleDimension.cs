namespace VisualBoy.UI
{
	public struct StyleDimension
	{
		public static StyleDimension Fill = new StyleDimension(0f, 1f);
		public static StyleDimension Empty = new StyleDimension(0f, 0f);

		public float Pixels;
		public float Percent;

		public StyleDimension(float pixels, float percent)
		{
			Pixels = pixels;
			Percent = percent;
		}

		public void Set(float pixels, float percent)
		{
			Pixels = pixels;
			Percent = percent;
		}

		public float GetValue(float containerSize)
		{
			return Pixels + (Percent * containerSize);
		}

		public static bool operator ==(StyleDimension left, StyleDimension right)
		{
			return left.Pixels == right.Pixels && left.Percent == right.Percent;
		}

		public static bool operator !=(StyleDimension left, StyleDimension right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return obj is StyleDimension dim && dim == this;
		}

		public override int GetHashCode()
		{
			return Pixels != 0f ? Pixels.GetHashCode() : Percent.GetHashCode();
		}
	}
}
