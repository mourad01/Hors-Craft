// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitedInt
using System;
using UnityEngine;

[Serializable]
public class LimitedInt
{
	[SerializeField]
	private int min;

	[SerializeField]
	private int max;

	[SerializeField]
	private int current;

	public int Min
	{
		get
		{
			return min;
		}
		set
		{
			min = value;
		}
	}

	public int Max
	{
		get
		{
			return max;
		}
		set
		{
			max = Mathf.Clamp(value, min, int.MaxValue);
		}
	}

	public int Current
	{
		get
		{
			return current;
		}
		set
		{
			current = Mathf.Clamp(value, min, max);
		}
	}

	public LimitedInt(int argStartValue)
	{
		current = argStartValue;
		min = int.MinValue;
		max = int.MaxValue;
	}

	public LimitedInt(int argStartValue, int argMin, int argMax)
	{
		current = argStartValue;
		min = argMin;
		max = argMax;
	}

	public LimitedInt(LimitedInt argCopy)
	{
		min = argCopy.min;
		max = argCopy.max;
		current = argCopy.current;
	}

	public static LimitedInt operator +(LimitedInt argLHS, int argRHS)
	{
		LimitedInt limitedInt = new LimitedInt(argLHS);
		limitedInt.Current += argRHS;
		return limitedInt;
	}

	public static LimitedInt operator -(LimitedInt argLHS, int argRHS)
	{
		LimitedInt limitedInt = new LimitedInt(argLHS);
		limitedInt.Current -= argRHS;
		return limitedInt;
	}
}
