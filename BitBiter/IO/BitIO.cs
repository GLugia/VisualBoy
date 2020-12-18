using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BitBiter.IO
{
	public static class BitIO
	{
		private static readonly ObjectHandler[] ObjectHandlers = new ObjectHandler[]
		{
			null,
			new ObjectHandler<byte>((BinaryReader reader) => reader.ReadByte(), delegate(BinaryWriter writer, byte value)
			{
				writer.Write(value);
			}),
			new ObjectHandler<short>((BinaryReader reader) => reader.ReadInt16(), delegate(BinaryWriter writer, short value)
			{
				writer.Write(value);
			}),
			new ObjectHandler<int>((BinaryReader reader) => reader.ReadInt32(), delegate(BinaryWriter writer, int value)
			{
				writer.Write(value);
			}),
			new ObjectHandler<long>((BinaryReader reader) => reader.ReadInt64(), delegate(BinaryWriter writer, long value)
			{
				writer.Write(value);
			}),
			new ObjectHandler<float>((BinaryReader reader) => reader.ReadSingle(), delegate(BinaryWriter writer, float value)
			{
				writer.Write(value);
			}),
			new ObjectHandler<double>((BinaryReader reader) => reader.ReadDouble(), delegate(BinaryWriter writer, double value)
			{
				writer.Write(value);
			}),
			new ClassHandler<byte[]>((BinaryReader reader) => reader.ReadBytes(reader.ReadInt32()), delegate(BinaryWriter writer, byte[] value)
			{
				writer.Write(value.Length);
				writer.Write(value);
			}, (byte[] value) => (byte[])value.Clone(), () => new byte[0]),
			new ClassHandler<string>((BinaryReader reader) => Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt16())), delegate(BinaryWriter writer, string value)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value);
				writer.Write((short)bytes.Length);
				writer.Write(bytes);
			}, (string value) => (string)value.Clone(), () => ""),
			new ClassHandler<IList>((BinaryReader reader) => GetHandler(reader.ReadByte()).ReadList(reader, reader.ReadInt32()), delegate(BinaryWriter writer, IList value)
			{
				int typeid;
				try
				{
					typeid = GetHandlerID(value.GetType().GetGenericArguments()[0]);
				}
				catch (IOException)
				{
					string err = "Invalid list type: ";
					Type type = value.GetType();
					throw new IOException(err + ((type != null) ? type.ToString() : null));
				}
				writer.Write((byte)typeid);
				writer.Write(value.Count);
				ObjectHandlers[typeid].WriteList(writer, value);
			}, delegate(IList list)
			{
				IList ret;
				try
				{
					ret = GetHandler(GetHandlerID(list.GetType().GetGenericArguments()[0])).CloneList(list);
				}
				catch (IOException)
				{
					string err = "Invalid list type: ";
					Type type = list.GetType();
					throw new ArgumentException(err + (type != null ? type.ToString() : null));
				}
				return ret;
			}, null),
			new ClassHandler<SharpTag>(delegate(BinaryReader reader)
			{
				SharpTag tag = new SharpTag();

				object value;
				while ((value = ReadTag(reader, out string key)) != null)
				{
					tag.Set(key, value);
				}

				return tag;
			}, delegate(BinaryWriter writer, SharpTag tag)
			{
				foreach ((string key, object value) in tag)
				{
					if (value != null)
					{
						WriteTag(key, value, writer);
					}
				}

				writer.Write(0);
			}, (SharpTag tag) => (SharpTag)tag.Clone(), () => new SharpTag()),
			new ClassHandler<int[]>(delegate(BinaryReader reader)
			{
				int[] array = new int[reader.ReadInt32()];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = reader.ReadInt32();
				}
				return array;
			}, delegate(BinaryWriter writer, int[] array)
			{
				writer.Write(array.Length);
				foreach (int value in array)
				{
					writer.Write(value);
				}
			}, (int[] array) => (int[])array.Clone(), () => new int[0])
		};

		private static readonly Dictionary<Type, int> ObjectIDs = Enumerable.Range(1, ObjectHandlers.Length - 1).ToDictionary((int i) => ObjectHandlers[i].ObjectType);
		private static ObjectHandler<string> StringHandler = (ObjectHandler<string>)ObjectHandlers[8];

		private static ObjectHandler GetHandler(int id)
		{
			if (id < 1 || id >= ObjectHandlers.Length)
			{
				throw new IOException("Invalid handler id: " + id.ToString());
			}

			return ObjectHandlers[id];
		}

		private static int GetHandlerID(Type type)
		{
			if (ObjectIDs.TryGetValue(type, out int id))
			{
				return id;
			}

			if (typeof(IList).IsAssignableFrom(type))
			{
				return 9;
			}

			throw new IOException(string.Format("Invalid type '{0}'", type));
		}

		public static object Serialize(object value)
		{
			Type type = value.GetType();

			if (TagSerializer.TryGetSerializer(type, out TagSerializer serializer))
			{
				return serializer.Serialize(value);
			}

			if (GetHandlerID(type) != 9)
			{
				return value;
			}

			Type type2 = type.GetGenericArguments()[0];

			if (TagSerializer.TryGetSerializer(type2, out TagSerializer serializer2))
			{
				return serializer2.SerializeList((IList)value);
			}

			if (GetHandlerID(type2) != 9)
			{
				return value;
			}

			IList<IList> list = (value as IList<IList>) ?? ((IList)value).Cast<IList>().ToList();
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = (IList)Serialize(list[i]);
			}

			return list;
		}

		public static T Deserialize<T>(object tag)
		{
			if (tag is T)
			{
				return (T)tag;
			}

			return (T)Deserialize(typeof(T), tag);
		}

		public static object Deserialize(Type type, object tag)
		{
			if (type.IsInstanceOfType(tag))
			{
				return tag;
			}

			if (TagSerializer.TryGetSerializer(type, out TagSerializer serializer))
			{
				if (tag == null)
				{
					tag = Deserialize(serializer.TagType, null);
				}

				return serializer.Deserialize(tag);
			}

			if (tag == null)
			{
				if (type.GetGenericArguments().Length == 0)
				{
					return GetHandler(GetHandlerID(type)).Default();
				}

				if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return null;
				}
			}

			if ((tag == null || tag is IList) && type.GetGenericArguments().Length == 1)
			{
				Type type2 = type.GetGenericArguments()[0];
				Type type3 = typeof(List<>).MakeGenericType(new Type[] { type2 });

				if (type.IsAssignableFrom(type3))
				{
					if (tag == null)
					{
						return type3.GetConstructor(new Type[0]).Invoke(new object[0]);
					}

					if (TagSerializer.TryGetSerializer(type2, out TagSerializer serializer2))
					{
						return serializer2.DeserializeList((IList)tag);
					}

					IList list = (IList)tag;
					IList list2 = (IList)type3.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { list.Count });

					foreach (object tag2 in list)
					{
						list2.Add(Deserialize(type2, tag2));
					}

					return list2;
				}
			}

			if (tag == null)
			{
				throw new IOException(string.Format("Invalid object type '{0}'", type));
			}

			throw new InvalidCastException(string.Format("Unable to cast object of type '{0}' to type '{1}'", tag.GetType(), type));
		}

		public static T Clone<T>(T other)
		{
			return (T)GetHandler(GetHandlerID(other.GetType())).Clone(other);
		}

		public static object ReadTag(BinaryReader reader, out string name)
		{
			int num = reader.ReadByte();
			if (num == 0)
			{
				name = null;
				return null;
			}

			name = StringHandler.reader(reader);
			return ObjectHandlers[num].Read(reader);
		}

		public static void WriteTag(string name, object tag, BinaryWriter writer)
		{
			int objectid = GetHandlerID(tag.GetType());
			writer.Write((byte)objectid);
			StringHandler.writer(writer, name);
			ObjectHandlers[objectid].Write(writer, tag);
		}

		public static SharpTag FromFile(string path, bool compressed)
		{
			SharpTag ret;
			try
			{
				using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					ret = FromStream(stream, compressed);
				}
			}
			catch (IOException io)
			{
				throw new IOException("Failed to read STAG file: " + path, io);
			}

			return ret;
		}

		public static SharpTag FromStream(Stream stream, bool compressed = true)
		{
			if (compressed)
			{
				stream = new GZipStream(stream, CompressionLevel.Fastest);
			}

			return Read(new BigEndianReader(stream));
		}

		public static SharpTag Read(BinaryReader reader)
		{
			object obj = ReadTag(reader, out string name);

			if (!(obj is SharpTag))
			{
				throw new IOException("Root STAG is not a STAG");
			}

			return (SharpTag)obj;
		}

		public static void ToFile(SharpTag tag, string path, bool compress = true)
		{
			try
			{
				using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
				{
					ToStream(tag, stream, compress);
				}
			}
			catch (IOException io)
			{
				throw new IOException("Failed to read STAG file: " + path, io);
			}
		}

		public static void ToStream(SharpTag tag, Stream stream, bool compress = true)
		{
			if (compress)
			{
				stream = new GZipStream(stream, CompressionLevel.Optimal, true);
			}

			Write(tag, new BigEndianWriter(stream));

			if (compress)
			{
				stream.Close();
			}
		}

		public static void Write(SharpTag tag, BinaryWriter writer)
		{
			WriteTag("", tag, writer);
		}
	}
}
