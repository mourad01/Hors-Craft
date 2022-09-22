// DecompilerFi decompiler from Assembly-CSharp.dll class: MaybeMonad
using System;

public static class MaybeMonad
{
	public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator) where TInput : class where TResult : class
	{
		return (o != null) ? evaluator(o) : ((TResult)null);
	}

	public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
	{
		return (o != null) ? evaluator(o) : failureValue;
	}

	public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, Func<TInput, TResult> failureValue) where TInput : class
	{
		return (o != null) ? evaluator(o) : failureValue(o);
	}

	public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
	{
		if (o == null)
		{
			return (TInput)null;
		}
		return (!evaluator(o)) ? ((TInput)null) : o;
	}

	public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
	{
		if (o == null)
		{
			return (TInput)null;
		}
		return (!evaluator(o)) ? o : ((TInput)null);
	}

	public static TInput Do<TInput>(this TInput o, Action<TInput> action) where TInput : class
	{
		if (o == null)
		{
			return (TInput)null;
		}
		action(o);
		return o;
	}

	public static TResult IfNotNull<T, TResult>(this T target, Func<T, TResult> getValue)
	{
		return (target == null) ? default(TResult) : getValue(target);
	}
}
