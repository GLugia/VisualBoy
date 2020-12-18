namespace BitBiter.IO.Serializers
{
	public class UShortSerializer : TagSerializer<ushort, short>
	{
		public override short Serialize(ushort value)
		{
			return (short)value;
		}

		public override ushort Deserialize(short tag)
		{
			return (ushort)tag;
		}
	}
}
