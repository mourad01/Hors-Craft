// DecompilerFi decompiler from Assembly-CSharp.dll class: ComparableExtensions
using System;

public static class ComparableExtensions
{
	public static bool IsBetweenInclusive<T>(this T actual, T lower, T upper) where T : IComparable<T>
	{
		return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) <= 0;
	}

	public static bool IsBetweenExclusive<T>(this T actual, T lower, T upper) where T : IComparable<T>
	{
		return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
	}
}
