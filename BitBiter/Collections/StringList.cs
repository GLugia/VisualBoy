using BitBiter.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Vitrium.BitBiter.Collections
{
	public sealed class StringList : IEnumerable<string>
	{
		public int Capacity => int.MaxValue;
		public int Length => Strings.Length;
		public long Size => sizeof(int) * Length;
		private string[] Strings;
		public string this[int key]
		{
			get => Strings[key];
			set
			{
				if (key > Length)
				{
					Array.Resize(ref Strings, key);
				}

				Strings[key] = value;
			}
		}

		public StringList()
		{
			Strings = new string[0x00];
		}

		public StringList(string[] strings)
		{
			Strings = strings ?? new string[0x00];
		}

		public void Add(string value)
		{
			if (Length == Capacity)
			{
				throw new Exception("Maximum capacity reached");
			}

			this[Length + 0x01] = value;
		}

		public void AddRange(IEnumerable<string> data)
		{
			foreach (string value in data)
			{
				Add(value);
			}
		}

		public void Insert(int index, string value)
		{
			if (Length == Capacity)
			{
				throw new Exception("Maximum capacity reached");
			}

			string[] temp = new string[Length + 0x01];
			int j = 0x00;

			for (int i = 0x00; i < Length; i++)
			{
				if (i == index)
				{
					temp[j] = value;
					j++;
				}

				temp[j] = this[i];
				j++;
			}

			Strings = temp;
		}

		public void RemoveAll(string value)
		{
			string[] temp = new string[0x00];
			int j = 0x00;

			for (int i = 0x00; i < Length; i++)
			{
				if (this[i] == value)
				{
					continue;
				}

				Array.Resize(ref temp, j + 0x01);
				temp[j] = this[i];
				j++;
			}

			Strings = temp;
		}

		public void RemoveAt(params int[] index)
		{
			for (int i = 0x00; i < index.Length; i++)
			{
				if (index[i] > Length)
				{
					throw new IndexOutOfRangeException($"index[{i}]: {index[i]}");
				}
			}

			string[] temp = new string[0x00];
			int j = 0x00;

			for (int i = 0x00; i < Length; i++)
			{
				bool flag = false;

				for (int k = 0x00; k < index.Length; k++)
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

			Strings = temp;
		}

		public void RemoveRange(int index, int count)
		{
			if (index + count > Capacity)
			{
				count = Capacity - index;
			}

			string[] temp = new string[0x00];
			int j = 0x00;

			for (int i = 0x00; i < index; i++)
			{
				Array.Resize(ref temp, j + 0x01);
				temp[j] = this[i];
				j++;
			}

			for (int i = index + count; i < Length; i++)
			{
				Array.Resize(ref temp, j + 0x01);
				temp[j] = this[i];
				j++;
			}

			Strings = temp;
		}

		public void Clear()
		{
			Strings = new string[0x00];
		}

		public void ForEach(Action<string> action)
		{
			foreach (string value in this)
			{
				action.Invoke(value);
			}
		}

		public int IndexOf(string value)
		{
			for (int i = 0x00; i < Length; i++)
			{
				if (this[i] == value)
				{
					return i;
				}
			}

			return -1;
		}

		public int LastIndexOf(string value)
		{
			for (int i = Length; i > 0; i--)
			{
				if (this[i] == value)
				{
					return i;
				}
			}

			return -1;
		}

		public bool Contains(string value)
		{
			for (int i = 0; i < Length; i++)
			{
				if (this[i].Equals(value))
				{
					return true;
				}
			}

			return false;
		}

		public override bool Equals(object obj)
		{
			return obj is StringList list && list.Strings.Equals(Strings);
		}

		public override int GetHashCode()
		{
			return Strings.GetHashCode();
		}

		public override string ToString()
		{
			return Strings.ToString<string>();
		}

		public T AsCollection<T>() where T : ICollection<string>
		{
			T col = (T)Activator.CreateInstance(typeof(T));

			foreach (string value in this)
			{
				col.Add(value);
			}

			return col;
		}

		public string[] AsArray()
		{
			return Strings;
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (string value in this)
			{
				yield return value;
			}

			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Strings.GetEnumerator();
		}
	}
}
