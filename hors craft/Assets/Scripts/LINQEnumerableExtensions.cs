// DecompilerFi decompiler from Assembly-CSharp.dll class: LINQEnumerableExtensions
using System;
using System.Collections.Generic;
using System.Linq;

public static class LINQEnumerableExtensions
{
	public static IEnumerable<T> Generate<T>(Func<T> generator) where T : class
	{
		if (generator == null)
		{
			throw new ArgumentNullException("generator");
		}
		while (true)
		{
			T val;
			T t = val = generator();
			if (val != null)
			{
				yield return t;
				continue;
			}
			break;
		}
	}

	public static IEnumerable<T> Generate<T>(Func<T?> generator) where T : struct
	{
		if (generator == null)
		{
			throw new ArgumentNullException("generator");
		}
		while (true)
		{
			T? val;
			T? t = val = generator();
			T? val2 = val;
			if (val2.HasValue)
			{
				yield return t.Value;
				continue;
			}
			break;
		}
	}

	public static IEnumerable<T> FromEnumerator<T>(IEnumerator<T> enumerator)
	{
		if (enumerator == null)
		{
			throw new ArgumentNullException("enumerator");
		}
		while (enumerator.MoveNext())
		{
			yield return enumerator.Current;
		}
	}

	public static IEnumerable<T> Single<T>(T value)
	{
		return Enumerable.Repeat(value, 1);
	}

	public static IEnumerable<T> Single<T>(T value, int count)
	{
		return Enumerable.Repeat(value, count);
	}

	public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		foreach (T elem in source)
		{
			action(elem);
			yield return elem;
		}
	}

	public static IEnumerable<TOut> Combine<TIn1, TIn2, TOut>(this IEnumerable<TIn1> in1, IEnumerable<TIn2> in2, Func<TIn1, TIn2, TOut> func)
	{
		if (in1 == null)
		{
			throw new ArgumentNullException("in1");
		}
		if (in2 == null)
		{
			throw new ArgumentNullException("in2");
		}
		if (func == null)
		{
			throw new ArgumentNullException("func");
		}
		IEnumerator<TIn1> e3 = in1.GetEnumerator();
		try
		{
			IEnumerator<TIn2> e2 = in2.GetEnumerator();
			try
			{
				while (e3.MoveNext() && e2.MoveNext())
				{
					yield return func(e3.Current, e2.Current);
				}
			}
			finally
			{
               // base._003C_003E__Finally0();
			}
		}
		finally
		{
			//base._003C_003E__Finally1();
		}
	}

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
	{
		return from n in source
			orderby Guid.NewGuid()
			select n;
	}

	public static IEnumerable<T> UseLINQDisposible<T>(this T obj) where T : IDisposable
	{
		try
		{
			yield return obj;
		}
		finally
		{
			//base._003C_003E__Finally0();
		}
	}

	public static IEnumerable<T> DoWhile<T>(this IEnumerable<T> source, Action<T> action, Func<bool> compareFunc)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		foreach (T elem in source)
		{
			do
			{
				action(elem);
			}
			while (compareFunc());
			yield return elem;
		}
	}

	public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> @this, Func<T, TKey> keySelector)
	{
		return (from grps in @this.GroupBy(keySelector)
			select (grps)).Select(Enumerable.First<T>);
	}

	public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> collection, Predicate<T> endCondition)
	{
		return collection.TakeWhile((T item) => !endCondition(item));
	}
}
