using BitBiter.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Vitrium.BitBiter.Collections
{
	public sealed class IntList : IEnumerable<int>
	{
		public int Capacity => int.MaxValue;
		public int Length => Ints.Length;
		public long Size => sizeof(int) * Length;
		private int[] Ints;
		public int this[int key]
		{
			get => key + 0x01 > Length ? 0x00 : Ints[key];
			set
			{
				if (key + 0x01 > Length)
				{
					Array.Resize(ref Ints, key + 0x01);
				}

				Ints[key] = value;
			}
		}

		public IntList()
		{
			Ints = new int[0x00];
		}

		public IntList(int[] ints)
		{
			Ints = ints;
		}

		public void Add(int value)
		{
			if (Length == int.MaxValue)
			{
				throw new Exception("Maximum capacity reached");
			}

			this[Length + 0x01] = value;
		}

		public void AddRange(IEnumerable<int> data)
		{
			foreach (int val in data)
			{
				Add(val);
			}
		}

		public void Insert(int index, int value)
		{
			if (Length == int.MaxValue)
			{
				throw new Exception("Maximum capacity reached");
			}

			int[] temp = new int[Length + 0x01];
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

			Ints = temp;
		}

		public void RemoveAll(int value)
		{
			int[] temp = new int[0x00];
			int j = 0x00;

			for (int i = 0x00; i < Length; i++)
			{
				if (this[i] == value)
				{
					continue;
				}

				temp[j] = this[i];
				j++;
			}

			Ints = temp;
		}

		public void RemoveAt(params int[] index)
		{
			for (int k = 0x00; k < index.Length; k++)
			{
				if (index[k] > Length)
				{
					throw new IndexOutOfRangeException($"index[{k}]: {index[k]}"); ;
				}
			}

			int[] temp = new int[0x00];
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

			Ints = temp;
		}

		public void RemoveRange(int index, int count)
		{
			if (index + count > int.MaxValue)
			{
				count = int.MaxValue - index;
			}

			int[] temp = new int[0x00];
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

			Ints = temp;
		}

		public void Clear()
		{
			Ints = new int[0x00];
		}

		public void ForEach(Action<int> action)
		{
			foreach (int val in this)
			{
				action.Invoke(val);
			}
		}

		public int IndexOf(int value)
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

		public int LastIndexOf(int value)
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

		public override bool Equals(object obj)
		{
			return obj is IntList list && list.Ints.Equals(Ints);
		}

		public override int GetHashCode()
		{
			return Ints.GetHashCode();
		}

		public override string ToString()
		{
			return ArrayExtensions.ToString(Ints);
		}

		public T AsCollection<T>() where T : ICollection<int>
		{
			T col = (T)Activator.CreateInstance(typeof(T));

			foreach (int value in this)
			{
				col.Add(value);
			}

			return col;
		}

		public int[] AsArray()
		{
			return Ints;
		}

		public IEnumerator<int> GetEnumerator()
		{
			foreach (int value in Ints)
			{
				yield return value;
			}

			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Ints.GetEnumerator();
		}
	}
}
