using BitBiter.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BitBiter.Collections
{
	/// <summary>
	/// An array list containing byte values. Capacity is <see cref="byte.MaxValue"/>.
	/// </summary>
	public sealed class BitList : IEnumerable<byte> // @TODO fix this shit
	{
		public byte Capacity => byte.MaxValue;
		/// <summary>
		/// The length of bytes stored in the contained array
		/// </summary>
		public byte Length => (byte)(Bytes.Length);

		/// <summary>
		/// The size of the bytes stored in the list
		/// </summary>
		public long Size => sizeof(byte) * Length;
		private byte[] Bytes;
		public byte this[byte key]
		{
			get => Bytes[key];
			set
			{
				if (key + 1 > Length)
				{
					Array.Resize(ref Bytes, key + 1);
				}

				Bytes[key] = value;
			}
		}

		public BitList()
		{
			Bytes = new byte[0x00];
		}

		public BitList(byte[] bits)
		{
			Bytes = bits;
		}

		/// <summary>
		/// Adds a value to the end of the list
		/// </summary>
		/// <param name="value"></param>
		public void Add(byte value)
		{
			if (Length == 0xff)
			{
				throw new Exception("Maximum capacity reached");
			}

			this[(byte)(Length + 0x01)] = value;
		}

		/// <summary>
		/// Adds all bits from the enumerable data
		/// </summary>
		/// <param name="data">The data to add to the list</param>
		public void AddRange(IEnumerable<byte> data)
		{
			foreach (byte bit in data)
			{
				Add(bit);
			}
		}

		/// <summary>
		/// Inserts a value at the specified index
		/// </summary>
		/// <param name="index">The index of the array</param>
		/// <param name="value">The value to insert</param>
		public void Insert(byte index, byte value)
		{
			if (Length == 0xff)
			{
				throw new Exception("Maximum capacity reached");
			}

			byte[] temp = new byte[Length + 0x01];
			byte j = 0x00;

			for (byte i = 0x00; i < Length; i++)
			{
				if (i == index)
				{
					temp[j] = value;
					j++;
				}

				temp[j] = this[i];
				j++;
			}

			Bytes = temp;
		}

		/// <summary>
		/// Removes all values that equal the parameter
		/// </summary>
		/// <param name="value"></param>
		public void RemoveAll(byte value)
		{
			byte[] temp = new byte[0x00];
			byte j = 0x00;

			for (byte i = 0x00; i < Length; i++)
			{
				if (this[i] == value)
				{
					continue;
				}

				temp[j] = this[i];
				j++;
			}

			Bytes = temp;
		}

		/// <summary>
		/// Removes a value at the given indices
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(params byte[] index)
		{
			for (int k = 0x00; k < index.Length; k++)
			{
				if (index[k] > Length)
				{
					throw new IndexOutOfRangeException($"index[{k}]: {index[k]}");
				}
			}

			byte[] temp = new byte[0x00];
			byte j = 0x00;

			for (byte i = 0x00; i < Length; i++)
			{
				bool flag = false;

				for (byte k = 0x00; k < index.Length; k++)
				{
					if (i == index[k])
					{
						flag = true;
						break;
					}
				}

				if (!flag)
				{
					Array.Resize(ref temp, j + 0x01);
					temp[j] = this[i];
					j++;
				}
			}

			Bytes = temp;
		}

		/// <summary>
		/// Removes all bits from (index) to (index + count)
		/// </summary>
		/// <param name="index">The index of the list</param>
		/// <param name="count">How many bits should be removed</param>
		public void RemoveRange(byte index, byte count)
		{
			if (index + count > 0xff)
			{
				count = (byte)(0xff - index);
			}

			byte[] temp = new byte[0x00];
			byte j = 0x00;

			for (byte i = 0x00; i < index; i++)
			{
				Array.Resize(ref temp, j + 0x01);
				temp[j] = this[i];
				j++;
			}

			for (byte i = (byte)(index + count); i < Length; i++)
			{
				Array.Resize(ref temp, j + 0x01);
				temp[j] = this[i];
				j++;
			}

			Bytes = temp;
		}

		/// <summary>
		/// Clears the list
		/// </summary>
		public void Clear()
		{
			Bytes = new byte[0x00];
		}

		/// <summary>
		/// Execute an action for every bit in this list
		/// </summary>
		/// <param name="action"></param>
		public void ForEach(Action<byte> action)
		{
			foreach (byte bit in this)
			{
				action.Invoke(bit);
			}
		}

		/// <summary>
		/// Find the first instance of a bit and return its index. Returns <see cref="byte.MaxValue"/> if none found.
		/// </summary>
		/// <param name="bit">The instance of a bit to find</param>
		/// <returns></returns>
		public byte IndexOf(byte bit)
		{
			for (byte i = 0x00; i < Length; i++)
			{
				if (this[i] == bit)
				{
					return i;
				}
			}

			return 0xff;
		}

		/// <summary>
		/// Find the last instance of a bit and return its index. Returns <see cref="byte.MaxValue"/> if none found.
		/// </summary>
		/// <param name="bit">The instance of a bit to find</param>
		/// <returns></returns>
		public byte LastIndexOf(byte bit)
		{
			for (byte i = Length; i > 0; i--)
			{
				if (this[i] == bit)
				{
					return i;
				}
			}

			return 0xff;
		}

		public override bool Equals(object obj)
		{
			return obj is BitList list && list.Bytes.Equals(Bytes);
		}

		/// <summary>
		/// The Hash code of this list
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Bytes.GetHashCode();
		}

		/// <summary>
		/// Converts the array to a string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ArrayExtensions.ToString(Bytes);
		}

		/// <summary>
		/// Allows the user to convert this list into any collection. ie List
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T AsCollection<T>() where T : ICollection<byte>
		{
			T col = (T)Activator.CreateInstance(typeof(T));

			foreach (byte bit in this)
			{
				col.Add(bit);
			}

			return col;
		}

		/// <summary>
		/// Gets the underlying array of this list
		/// </summary>
		/// <returns></returns>
		public byte[] AsArray()
		{
			return Bytes;
		}

		/// <summary>
		/// Get the enumerator of this list
		/// </summary>
		/// <returns></returns>
		public IEnumerator<byte> GetEnumerator()
		{
			foreach (byte value in Bytes)
			{
				yield return value;
			}

			yield break;
		}

		/// <summary>
		/// This is a private and default method
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Bytes.GetEnumerator();
		}
	}
}
