using System;
using System.Collections.Generic;

namespace BitBiter.Extensions
{
	public static class ArrayExtensions
	{
		public static string ToString<T>(this T[] array)
		{
			string ret = "[ ";

			int i = -1;
			foreach (object item in array)
			{
				ret += item?.ToString() ?? "null";

				i++;
				if (i == array.Length - 1)
				{
					break;
				}

				ret += ", ";
			}

			return ret + " ]";
		}

		public static string ToString<T>(this ICollection<T> col)
		{
			string ret = "[ ";

			int i = -1;
			foreach (object item in col)
			{
				ret += item.ToString();

				i++;
				if (i == col.Count)
				{
					break;
				}

				ret += ", ";
			}

			return ret + " ]";
		}

		public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
		{
			key = tuple.Key;
			value = tuple.Value;
		}

		public static bool TryAdd<T>(this ICollection<T> col, T obj)
		{
			if (col.Contains(obj))
			{
				return false;
			}

			col.Add(obj);
			return true;
		}
		/*
		public static bool TryAdd<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 value)
		{
			if (dict.ContainsKey(key))
			{
				return false;
			}

			dict.Add(key, value);
			return true;
		}*/

		public static bool TryRemove<T>(this ICollection<T> col, T obj)
		{
			if (col.Contains(obj))
			{
				col.Remove(obj);
				return true;
			}

			return false;
		}

		public static bool TryRemove<T1, T2>(this IDictionary<T1, T2> dict, T1 key)
		{
			if (dict.ContainsKey(key))
			{
				dict.Remove(key);
				return true;
			}

			return false;
		}

		public static void ShiftRight<T>(this T[] ar)
		{
			T[] temp = new T[ar.Length];
			T remainder = ar[ar.Length - 1];

			for (int i = 1; i < ar.Length; i++)
			{
				temp[i] = ar[i - 1];
			}

			temp[0] = remainder;

			Array.Copy(temp, ar, temp.Length);
		}

		public static void ShiftLeft<T>(this T[] ar)
		{
			T[] temp = new T[ar.Length];
			T remainder = ar[0];

			for (int i = 1; i < ar.Length; i++)
			{
				temp[i - 1] = ar[i];
				continue;
			}

			temp[temp.Length - 1] = remainder;
			Array.Copy(temp, ar, temp.Length);
		}
	}
}
