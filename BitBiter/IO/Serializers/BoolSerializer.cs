namespace BitBiter.IO.Serializers
{
	public class BoolSerializer : TagSerializer<bool, byte>
	{
		public override byte Serialize(bool value)
		{
			return value ? (byte)1 : (byte)0;
		}

		public override bool Deserialize(byte tag)
		{
			return tag > 0;
		}
	}
}
