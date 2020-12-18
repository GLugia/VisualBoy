using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BitBiter.IO
{
	abstract class ObjectHandler
	{
		public abstract Type ObjectType { get; }
		public abstract object Default();
		public abstract object Read(BinaryReader reader);
		public abstract void Write(BinaryWriter writer, object obj);
		public abstract IList ReadList(BinaryReader reader, int size);
		public abstract void WriteList(BinaryWriter writer, IList list);
		public abstract object Clone(object obj);
		public abstract IList CloneList(IList list);
	}

	class ObjectHandler<T> : ObjectHandler
	{
		internal Func<BinaryReader, T> reader;
		internal Action<BinaryWriter, T> writer;

		public ObjectHandler(Func<BinaryReader, T> reader, Action<BinaryWriter, T> writer)
		{
			this.reader = reader;
			this.writer = writer;
		}

		public override Type ObjectType => typeof(T);

		public override object Read(BinaryReader reader)
		{
			return this.reader(reader);
		}

		public override void Write(BinaryWriter writer, object obj)
		{
			this.writer(writer, (T)obj);
		}

		public override IList ReadList(BinaryReader reader, int size)
		{
			List<T> list = new List<T>();
			for (int i = 0; i < size; i++)
			{
				list.Add(this.reader(reader));
			}
			return list;
		}

		public override void WriteList(BinaryWriter writer, IList list)
		{
			foreach (T obj in list)
			{
				this.writer(writer, obj);
			}
		}

		public override object Clone(object obj)
		{
			return obj;
		}

		public override IList CloneList(IList list)
		{
			return CloneList((IList<T>)list);
		}

		public virtual IList CloneList(IList<T> list)
		{
			return new List<T>(list);
		}

		public override object Default()
		{
			return default(T);
		}
	}

	class ClassHandler<T> : ObjectHandler<T> where T : class
	{
		internal Func<T, T> clone;
		internal Func<T> makeDefault;

		public ClassHandler(Func<BinaryReader, T> reader, Action<BinaryWriter, T> writer, Func<T, T> clone, Func<T> makeDefault = null)
			: base(reader, writer)
		{
			this.clone = clone;
			this.makeDefault = makeDefault;
		}

		public override object Clone(object obj)
		{
			return clone((T)obj);
		}

		public override IList CloneList(IList<T> list)
		{
			return list.Select(clone).ToList<T>();
		}

		public override object Default()
		{
			return makeDefault();
		}
	}
}
