using VisualBoy.Core.Modules;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BitBiter.IO
{
	public class SharpTag : IEnumerable<KeyValuePair<string, object>>, IEnumerable, ICloneable
	{
		public int Count => Tags.Count;
		private readonly Dictionary<string, object> Tags = new Dictionary<string, object>();

		public object this[string key]
		{
			get => Tags[key];
			set => Set(key, value, true);
		}

		public void Add(string key, object value)
		{
			Set(key, value);
		}

		public void Add(KeyValuePair<string, object> kvp)
		{
			Set(kvp.Key, kvp.Value);
		}

		public T Get<T>(string key)
		{
			Tags.TryGetValue(key, out object val);
			T ret;
			try
			{
				ret = BitIO.Deserialize<T>(val);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("STAG Deserialization (type={0}),", typeof(T)) + "entry=" + /*TagOut.Print(new KeyValuePair<string, object>(key, val)) +*/ ")", ex);
			}
			return ret;
		}

		public void Set(string key, object val)
		{
			Set(key, val, false);
		}

		public void Set(string key, object val, bool replace = false)
		{
			if (val == null)
			{
				Remove(key);
				return;
			}

			object obj;
			try
			{
				obj = BitIO.Serialize(val);
			}
			catch (IOException io)
			{
				string text = "value=" + ((val != null) ? val.ToString() : null);
				if (val.GetType().ToString() != val.ToString())
				{
					string str = "type=";
					Type type = val.GetType();
					text = str + (type?.ToString()) + "," + text;
				}
				throw new IOException(string.Concat(new string[]
				{
					"NBT Serialization (key=",
					key,
					",",
					text,
					")"
				}), io);
			}

			if (replace)
			{
				Tags[key] = obj;
				return;
			}

			Tags.Add(key, obj);
		}

		public bool ContainsKey(string key)
		{
			return Tags.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return Tags.Remove(key);
		}

		public bool GetBool(string key)
		{
			return Get<bool>(key);
		}

		public byte GetByte(string key)
		{
			return Get<byte>(key);
		}

		public sbyte GetSByte(string key)
		{
			return Get<sbyte>(key);
		}

		public short GetShort(string key)
		{
			return Get<short>(key);
		}

		public ushort GetUShort(string key)
		{
			return Get<ushort>(key);
		}

		public int GetInt(string key)
		{
			return Get<int>(key);
		}

		public uint GetUInt(string key)
		{
			return Get<uint>(key);
		}

		public long GetLong(string key)
		{
			return Get<long>(key);
		}

		public ulong GetULong(string key)
		{
			return Get<ulong>(key);
		}

		public float GetFloat(string key)
		{
			return Get<float>(key);
		}

		public decimal GetDecimal(string key)
		{
			return Get<decimal>(key);
		}

		public double GetDouble(string key)
		{
			return Get<double>(key);
		}

		public byte[] GetByteArray(string key)
		{
			return Get<byte[]>(key);
		}

		public int[] GetIntArray(string key)
		{
			return Get<int[]>(key);
		}

		public string GetString(string key)
		{
			return Get<string>(key);
		}

		public IList<T> GetList<T>(string key)
		{
			return Get<IList<T>>(key);
		}

		public SharpTag GetSharpTag(string key)
		{
			return Get<SharpTag>(key);
		}

		public Vector2 GetVector2(string key)
		{
			return Get<Vector2>(key);
		}

		public Point GetPoint(string key)
		{
			return Get<Point>(key);
		}

		public Module GetModule(string key)
		{
			return Get<Module>(key);
		}

		public void Clear()
		{
			Tags.Clear();
		}

		public object Clone()
		{
			SharpTag tag = new SharpTag();
			foreach ((string key, object val) in this)
			{
				tag.Set(key, BitIO.Clone(val));
			}
			return tag;
		}

		public override string ToString()
		{
			return TagOut.Print(this);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return Tags.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
