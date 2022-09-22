// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectDumper
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

public class ObjectDumper
{
	private StringBuilder sb = new StringBuilder();

	private TextWriter writer;

	private int pos;

	private int level;

	private int depth;

	private ObjectDumper(int depth)
	{
		writer = Console.Out;
		this.depth = depth;
	}

	private ObjectDumper(int depth, bool ConsoleOut)
	{
		if (ConsoleOut)
		{
			writer = Console.Out;
		}
		this.depth = depth;
	}

	public static void Write(object o)
	{
		Write(o, 0);
	}

	public static void Write(object o, int depth)
	{
		ObjectDumper objectDumper = new ObjectDumper(depth);
		objectDumper.WriteObject(null, o);
	}

	public static string GetObjectValue(object o)
	{
		ObjectDumper objectDumper = new ObjectDumper(1, ConsoleOut: false);
		objectDumper.WriteObject(null, o);
		return objectDumper.sb.ToString();
	}

	private void WriterWrite(string s)
	{
		if (writer != null)
		{
			writer.Write(s);
		}
		sb.Append(s);
	}

	private void Write(string s)
	{
		if (s != null)
		{
			WriterWrite(s);
			pos += s.Length;
		}
	}

	private void WriteIndent()
	{
		for (int i = 0; i < level; i++)
		{
			WriterWrite("  ");
		}
	}

	private void WriteLine()
	{
		if (writer != null)
		{
			writer.WriteLine();
		}
		sb.Append("\r\n");
		pos = 0;
	}

	private void WriteTab()
	{
		WriterWrite("  ");
		while (pos % 8 != 0)
		{
			Write(" ");
		}
	}

	private void WriteObject(string prefix, object o)
	{
		try
		{
			if (o == null || o is ValueType || o is string || o is XElement)
			{
				WriteIndent();
				Write(prefix);
				WriteValue(o);
				WriteLine();
			}
			else if (o is IEnumerable)
			{
				IEnumerator enumerator = ((IEnumerable)o).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						try
						{
							if (current is IEnumerable && !(current is string))
							{
								WriteIndent();
								Write(prefix);
								Write("...");
								WriteLine();
								if (level < depth)
								{
									level++;
									WriteObject(prefix, current);
									level--;
								}
							}
							else
							{
								WriteObject(prefix, current);
							}
						}
						catch (Exception)
						{
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			else
			{
				MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
				WriteIndent();
				Write(prefix);
				bool flag = false;
				MemberInfo[] array = members;
				foreach (MemberInfo memberInfo in array)
				{
					try
					{
						FieldInfo fieldInfo = memberInfo as FieldInfo;
						PropertyInfo propertyInfo = memberInfo as PropertyInfo;
						if (fieldInfo != null || propertyInfo != null)
						{
							if (flag)
							{
								WriteTab();
							}
							else
							{
								flag = true;
							}
							Write(memberInfo.Name);
							Write("=");
							Type type = (!(fieldInfo != null)) ? propertyInfo.PropertyType : fieldInfo.FieldType;
							if (type.IsValueType || type == typeof(string))
							{
								WriteValue((!(fieldInfo != null)) ? propertyInfo.GetValue(o, null) : fieldInfo.GetValue(o));
							}
							else if (typeof(IEnumerable).IsAssignableFrom(type))
							{
								Write("...");
							}
							else
							{
								Write("{ }");
							}
							WriteLine();
						}
					}
					catch (Exception)
					{
					}
				}
				if (flag)
				{
					WriteLine();
				}
				if (level < depth)
				{
					MemberInfo[] array2 = members;
					foreach (MemberInfo memberInfo2 in array2)
					{
						try
						{
							FieldInfo fieldInfo2 = memberInfo2 as FieldInfo;
							PropertyInfo propertyInfo2 = memberInfo2 as PropertyInfo;
							if (fieldInfo2 != null || propertyInfo2 != null)
							{
								Type type2 = (!(fieldInfo2 != null)) ? propertyInfo2.PropertyType : fieldInfo2.FieldType;
								if (!type2.IsValueType && !(type2 == typeof(string)))
								{
									object obj = (!(fieldInfo2 != null)) ? propertyInfo2.GetValue(o, null) : fieldInfo2.GetValue(o);
									if (obj != null)
									{
										level++;
										WriteObject(memberInfo2.Name + ": ", obj);
										level--;
									}
								}
							}
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void WriteValue(object o)
	{
		if (o == null)
		{
			Write("null");
		}
		else if (o is DateTime)
		{
			Write(((DateTime)o).ToShortDateString());
		}
		else if (o is ValueType || o is string || o is XElement)
		{
			Write(o.ToString());
		}
		else if (o is IEnumerable)
		{
			Write("...");
		}
		else
		{
			Write("{ }");
		}
	}
}
