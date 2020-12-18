namespace BitBiter.IO.Serializers
{
	public class ULongSerializer : TagSerializer<ulong, long>
	{
		public override long Serialize(ulong value)
		{
			return (long)value;
		}

		public override ulong Deserialize(long tag)
		{
			return (ulong)tag;
		}
	}
}
