using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitBiter.IO
{
	public class TagOut
	{
		private string indent = "";
		private readonly StringBuilder sb = new StringBuilder();

		public override string ToString()
		{
			return sb.ToString();
		}

		private string TypeString(Type type)
		{
			if (type == typeof(SharpTag))
			{
				return "object";
			}
			if (type == typeof(IList))
			{
				return "list";
			}

			string name = type.Name.ToLowerInvariant();
			return name switch
			{
				"byte" => name,
				"byte[]" => name,
				"sbyte" => name,
				"int16" => "short",
				"uint16" => "ushort",
				"int32" => "int",
				"int[]" => name,
				"int32[]" => "int[]",
				"uint32" => "uint",
				"int64" => "long",
				"uint64" => "ulong",
				"single" => "float",
				"double" => name,
				"string" => name,
				_ => throw new ArgumentException("Unknown type: " + type != null ? type.ToString() : null)
			};
		}

		private void WriteList<T>(char start, char end, bool multiline, IEnumerable<T> list, Action<T> write)
		{
			sb.Append(start);
			indent += "    ";
			bool flag = true;
			foreach (T obj in list)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					sb.Append(multiline ? "," : ", ");
				}

				if (multiline)
				{
					sb.AppendLine().Append(indent);
				}

				write(obj);
			}

			indent = indent.Substring(2);

			if (multiline && !flag)
			{
				sb.AppendLine().Append(indent);
			}

			sb.Append(end);
		}

		private void WriteEntry(KeyValuePair<string, object> entry)
		{
			if (entry.Value == null)
			{
				sb.Append('"').Append(entry.Key).Append("\" = null");
				return;
			}

			Type type = entry.Value.GetType();
			bool flag = entry.Value is IList && !(entry.Value is Array);
			sb.Append(TypeString(flag ? type.GetGenericArguments()[0] : type));
			sb.Append(" \"").Append(entry.Key).Append("\" ");
			if (type != typeof(SharpTag) && !flag)
			{
				sb.Append("= ");
			}
			WriteValue(entry.Value);
		}

		private void WriteValue(object value)
		{
			if (value is SharpTag tag)
			{
				WriteList('{', '}', true, tag, new Action<KeyValuePair<string, object>>(WriteEntry));
				return;
			}

			if (value is IList list)
			{
				Type type = value.GetType().GetGenericArguments()[0];
				WriteList('[', ']', type == typeof(string) || type == typeof(SharpTag) || typeof(IList).IsAssignableFrom(type), list.Cast<object>(), delegate (object o)
				{
					if (type == typeof(IList))
					{
						sb.Append(TypeString(o.GetType().GetGenericArguments()[0])).Append(' ');
					}

					WriteValue(o);
				});

				return;
			}

			string name = value.GetType().Name.ToLowerInvariant();
			switch (name)
			{
				case "byte": sb.Append((byte)value); return;
				case "byte[]": sb.Append('[').Append(string.Join(",", (byte[])value)).Append(']'); return;
				case "sbyte": sb.Append((sbyte)value); return;
				case "int16": sb.Append((short)value); return;
				case "uint16": sb.Append((ushort)value); return;
				case "int32": sb.Append((int)value); return;
				case "int[]":
				case "int32[]": sb.Append('[').Append(string.Join(",", (int[])value)).Append(']'); return;
				case "uint32": sb.Append((uint)value); return;
				case "int64": sb.Append((long)value); return;
				case "uint64": sb.Append((ulong)value); return;
				case "single": sb.Append((float)value); return;
				case "double": sb.Append((double)value); return;
				case "string": sb.Append((string)value); return;
				default: throw new ArgumentException(string.Format("Failed to convert unknown value '{0}' to string: {1}", name, value));
			}
		}

		public static string Print(SharpTag tag)
		{
			TagOut tout = new TagOut();
			tout.WriteValue(tag);
			return tout.ToString();
		}

		public static string Print(KeyValuePair<string, object> entry)
		{
			TagOut tout = new TagOut();
			tout.WriteEntry(entry);
			return tout.ToString();
		}
	}
}
