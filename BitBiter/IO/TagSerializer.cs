using BitBiter.IO.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BitBiter.IO
{
	public abstract class TagSerializer
	{
		public abstract Type Type { get; }
		public abstract Type TagType { get; }
		public abstract object Serialize(object value);
		public abstract object Deserialize(object tag);
		public abstract IList SerializeList(IList value);
		public abstract IList DeserializeList(IList value);

		static TagSerializer()
		{
			TagSerializer.Reload();
		}

		private static IDictionary<Type, TagSerializer> serializers = new Dictionary<Type, TagSerializer>();
		private static IDictionary<string, Type> typeNameCache = new Dictionary<string, Type>();

		internal static void Reload()
		{
			TagSerializer.serializers.Clear();
			TagSerializer.typeNameCache.Clear();
			TagSerializer.AddSerializer(new BoolSerializer());
			TagSerializer.AddSerializer(new UShortSerializer());
			TagSerializer.AddSerializer(new UIntSerializer());
			TagSerializer.AddSerializer(new ULongSerializer());
			TagSerializer.AddSerializer(new Vector2Serializer());
			TagSerializer.AddSerializer(new ColorSerializer());
			TagSerializer.AddSerializer(new PointSerializer());
			TagSerializer.AddSerializer(new RectangleSerializer());
			TagSerializer.AddSerializer(new KeySerializer());
		}

		public static bool TryGetSerializer(Type type, out TagSerializer serializer)
		{
			if (TagSerializer.serializers.TryGetValue(type, out serializer))
			{
				return true;
			}

			if (typeof(TagSerializable).IsAssignableFrom(type))
			{
				Type type2 = typeof(TagSerializableSerializer<>).MakeGenericType(new Type[] { type });
				IDictionary<Type, TagSerializer> dict = TagSerializer.serializers;
				TagSerializer value;
				serializer = (value = (TagSerializer)type2.GetConstructor(new Type[0]).Invoke(new object[0]));
				dict[type] = value;
				return true;
			}

			return false;
		}

		public static void AddSerializer(TagSerializer serializer)
		{
			TagSerializer.serializers.Add(serializer.Type, serializer);
		}

		public static Type GetType(string name)
		{
			if (TagSerializer.typeNameCache.TryGetValue(name, out Type type))
			{
				return type;
			}

			type = Type.GetType(name);

			if (type != null)
			{
				return TagSerializer.typeNameCache[name] = type;
			}

			return null;
		}
	}

	public abstract class TagSerializer<T, S> : TagSerializer
	{
		public override Type Type => typeof(T);
		public override Type TagType => typeof(S);

		public abstract S Serialize(T value);
		public abstract T Deserialize(S tag);

		public override object Serialize(object value)
		{
			return Serialize((T)value);
		}

		public override object Deserialize(object tag)
		{
			return Deserialize((S)tag);
		}

		public override IList SerializeList(IList value)
		{
			return ((IList<T>)value).Select(new Func<T, S>(Serialize)).ToList();
		}

		public override IList DeserializeList(IList value)
		{
			return ((IList<S>)value).Select(new Func<S, T>(Deserialize)).ToList();
		}
	}
}
