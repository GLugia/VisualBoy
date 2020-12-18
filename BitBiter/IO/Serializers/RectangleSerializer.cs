using Microsoft.Xna.Framework;

namespace BitBiter.IO.Serializers
{
	public class RectangleSerializer : TagSerializer<Rectangle, SharpTag>
	{
		public override SharpTag Serialize(Rectangle value)
		{
			return new SharpTag
			{
				{ "x", value.X },
				{ "y", value.Y },
				{ "w", value.Width },
				{ "h", value.Height }
			};
		}

		public override Rectangle Deserialize(SharpTag tag)
		{
			return new Rectangle(tag.GetInt("x"), tag.GetInt("y"), tag.GetInt("w"), tag.GetInt("h"));
		}
	}
}
