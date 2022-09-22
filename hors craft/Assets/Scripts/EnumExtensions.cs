// DecompilerFi decompiler from Assembly-CSharp.dll class: EnumExtensions
using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumExtensions
{
	public static T ToEnum<T>(this string s, bool ignoreCase) where T : struct
	{
		if (s.IsNullOrEmpty())
		{
			return default(T);
		}
		Type typeFromHandle = typeof(T);
		if (!typeFromHandle.IsEnum)
		{
			return default(T);
		}
		try
		{
			return (T)Enum.Parse(typeFromHandle, s, ignoreCase);
		}
		catch (Exception)
		{
			Array values = Enum.GetValues(typeFromHandle);
			using (IEnumerator<T> enumerator = (from T en in values
				where en.ToString().Equals(s, StringComparison.InvariantCultureIgnoreCase) || (en as Enum).ToString().Equals(s, StringComparison.InvariantCultureIgnoreCase)
				select en).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return default(T);
		}
	}

	public static T ToEnum<T>(this string s) where T : struct
	{
		return s.ToEnum<T>(ignoreCase: false);
	}
}
