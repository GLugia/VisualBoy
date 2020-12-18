using Microsoft.Xna.Framework;

namespace BitBiter.IO.Serializers
{
	public class ColorSerializer : TagSerializer<Color, int>
	{
		public override int Serialize(Color value)
		{
			return (int)value.PackedValue;
		}

		public override Color Deserialize(int tag)
		{
			return new Color(tag & 255, tag >> 8 & 255, tag >> 16 & 255, tag >> 24 & 255);
		}
	}
}
