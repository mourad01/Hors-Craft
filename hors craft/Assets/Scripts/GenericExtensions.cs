// DecompilerFi decompiler from Assembly-CSharp.dll class: GenericExtensions
using System.Collections.Generic;
using System.Linq;

public static class GenericExtensions
{
	public static bool IsIn<T>(this T source, params T[] list) where T : class
	{
		return source != null && !list.IsNullOrEmpty() && list.Contains(source);
	}

	public static bool IsIn<T>(this T source, params T?[] list) where T : struct
	{
		return !list.IsNullOrEmpty() && list.Contains(source);
	}

	public static bool IsIn(this int source, params int[] list)
	{
		return !list.IsNullOrEmpty() && list.Contains(source);
	}

	public static bool IsNotIn<T>(this T source, params T[] list) where T : class
	{
		if (source == null)
		{
			return false;
		}
		if (list.IsNullOrEmpty())
		{
			return true;
		}
		return !list.Contains(source);
	}

	public static bool IsNotIn<T>(this T source, params T?[] list) where T : struct
	{
		if (list.IsNullOrEmpty())
		{
			return true;
		}
		return !list.Contains(source);
	}

	public static bool IsNotIn(this int source, params int[] list)
	{
		if (list.IsNullOrEmpty())
		{
			return true;
		}
		return !list.Contains(source);
	}

	public static List<T> AsList<T>(this T tobject)
	{
		List<T> list = new List<T>();
		list.Add(tobject);
		return list;
	}

	public static bool IsTNull<T>(this T tObj)
	{
		return EqualityComparer<T>.Default.Equals(tObj, default(T));
	}
}
