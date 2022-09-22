// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitedFloat
using System;
using UnityEngine;

[Serializable]
public class LimitedFloat
{
	[SerializeField]
	private float min;

	[SerializeField]
	private float max;

	[SerializeField]
	private float current;

	public float Min
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

	public float Max
	{
		get
		{
			return max;
		}
		set
		{
			max = Mathf.Clamp(value, min, float.MaxValue);
		}
	}

	public float Current
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

	public LimitedFloat(float argStartValue)
	{
		current = argStartValue;
		min = float.MinValue;
		max = float.MaxValue;
	}

	public LimitedFloat(float argStartValue, float argMin, float argMax)
	{
		current = argStartValue;
		min = argMin;
		max = argMax;
	}

	public LimitedFloat(LimitedFloat argCopy)
	{
		min = argCopy.Min;
		max = argCopy.Max;
		current = argCopy.Current;
	}

	public static LimitedFloat operator +(LimitedFloat argLHS, float argRHS)
	{
		LimitedFloat limitedFloat = new LimitedFloat(argLHS);
		limitedFloat.Current += argRHS;
		return limitedFloat;
	}

	public static LimitedFloat operator -(LimitedFloat argLHS, float argRHS)
	{
		LimitedFloat limitedFloat = new LimitedFloat(argLHS);
		limitedFloat.Current -= argRHS;
		return limitedFloat;
	}
}
