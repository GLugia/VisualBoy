using Microsoft.Xna.Framework;

namespace BitBiter.IO.Serializers
{
	public class Vector2Serializer : TagSerializer<Vector2, SharpTag>
	{
		public override SharpTag Serialize(Vector2 value)
		{
			return new SharpTag
			{
				{ "x", value.X},
				{ "y", value.Y }
			};
		}

		public override Vector2 Deserialize(SharpTag tag)
		{
			return new Vector2(tag.GetFloat("x"), tag.GetFloat("y"));
		}
	}
}
