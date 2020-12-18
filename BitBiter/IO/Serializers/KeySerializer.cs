using BitBiter.IO;
using Microsoft.Xna.Framework.Input;

namespace BitBiter.IO.Serializers
{
	public class KeySerializer : TagSerializer<Keys, int>
	{
		public override int Serialize(Keys value)
		{
			return (int)value;
		}

		public override Keys Deserialize(int tag)
		{
			return (Keys)tag;
		}
	}
}
