using Microsoft.Xna.Framework;

namespace BitBiter.IO.Serializers
{
	public class PointSerializer : TagSerializer<Point, SharpTag>
	{
		public override SharpTag Serialize(Point value)
		{
			return new SharpTag
			{
				{ "x", value.X},
				{ "y", value.Y }
			};
		}

		public override Point Deserialize(SharpTag tag)
		{
			return new Point(tag.GetInt("x"), tag.GetInt("y"));
		}
	}
}
