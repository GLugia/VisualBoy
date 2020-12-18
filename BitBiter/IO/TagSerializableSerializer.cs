using System;
using System.Reflection;

namespace BitBiter.IO
{
	internal class TagSerializableSerializer<T> : TagSerializer<T, SharpTag> where T : TagSerializable
	{
		private Func<SharpTag, T> deserializer;

		public TagSerializableSerializer()
		{
			Type typeFromHandle = typeof(T);
			FieldInfo field = typeFromHandle.GetField("DESERIALIZER");

			if (field != null)
			{
				if (field.FieldType != typeof(Func<SharpTag, T>))
				{
					throw new ArgumentException(string.Format("Invalid deserializer field type '{0}' in {1} expected {2}.", field.FieldType, typeFromHandle.FullName, typeof(Func<SharpTag, T>)));
				}

				deserializer = (Func<SharpTag, T>)field.GetValue(null);
			}
		}

		public override SharpTag Serialize(T value)
		{
			SharpTag tag = value.SerializeData();
			tag["<type>"] = value.GetType().FullName;
			return tag;
		}

		public override T Deserialize(SharpTag tag)
		{
			if (tag.ContainsKey("<type>") && tag.GetString("<type>") != Type.FullName)
			{
				Type type = TagSerializer.GetType(tag.GetString("<type>"));

				if (type != null && Type.IsAssignableFrom(type) && TagSerializer.TryGetSerializer(type, out TagSerializer serializer))
				{
					return (T)serializer.Deserialize(tag);
				}
			}

			if (deserializer == null)
			{
				throw new ArgumentException(string.Format("Missing deserializer for type '{0}'", Type.FullName));
			}

			return deserializer(tag);
		}
	}
}
