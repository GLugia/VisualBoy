using System.Globalization;

namespace BitBiter.Extensions
{
	public static class StringExtensions
	{
		public static string AutoSpace(this string text)
		{
			string ret = "";
			bool first = true;

			foreach (char c in text)
			{
				if (char.IsUpper(c) && !first)
				{
					ret += " " + c;
				}
				else
				{
					ret += c;
					first = false;
				}
			}

			return ret;
		}

		public static bool IsSimilarTo(this string a, string b)
		{
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(a, b, CompareOptions.IgnoreCase) >= 0;
		}

		public static bool Contains(this string str, params string[] strings)
		{
			foreach (string s in strings)
			{
				if (str.Contains(s) || str.IsSimilarTo(s))
				{
					return true;
				}
			}

			return false;
		}
	}
}
