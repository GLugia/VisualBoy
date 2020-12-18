using Microsoft.Xna.Framework;

namespace VisualBoy.UI
{
	public struct CalculatedStyle
	{
		public float X, Y, Width, Height;
		public Rectangle AsRectangle => new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
		public Vector2 Position => new Vector2(X, Y);
		public Vector2 Center => new Vector2(X + Width / 2f, Y + Height / 2f);
		public float Right => X + Width;
		public float Left => X;
		public float Top => Y;
		public float Bottom => Y + Height;

		public CalculatedStyle(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return $"Style {{ {X}, {Y}, {Width}, {Height} }}";
		}
	}
}
