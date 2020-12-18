namespace BitBiter.IO.Serializers
{
	public class SByteSerializer : TagSerializer<sbyte, short>
	{
		public override short Serialize(sbyte value)
		{
			return value;
		}

		public override sbyte Deserialize(short tag)
		{
			return (sbyte)tag;
		}
	}
}
