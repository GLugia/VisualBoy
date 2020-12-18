namespace BitBiter.IO.Serializers
{
	public class UIntSerializer : TagSerializer<uint, int>
	{
		public override int Serialize(uint value)
		{
			return (int)value;
		}

		public override uint Deserialize(int tag)
		{
			return (uint)tag;
		}
	}
}
