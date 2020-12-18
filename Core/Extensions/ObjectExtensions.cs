namespace VisualBoy.Core.Extensions
{
	public static class ObjectExtensions
	{
		public static int AsInt(this object obj)
		{
			if (obj.GetType().IsValueType)
			{
				return (int)obj;
			}

			return obj.GetHashCode();
		}
	}
}
