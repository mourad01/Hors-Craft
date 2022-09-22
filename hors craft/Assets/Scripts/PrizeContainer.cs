// DecompilerFi decompiler from Assembly-CSharp.dll class: PrizeContainer
using System;
using UnityEngine;

[Serializable]
public class PrizeContainer
{
	[SerializeField]
	protected PrizeBase prizeBase;

	[SerializeField]
	protected int baseAmount;

	[SerializeField]
	protected int multiplier;

	public PrizeBase prize => prizeBase;

	public int BaseAmount => baseAmount;

	public int Multiplier => multiplier;

	public int GetCorrectAmount(int index)
	{
		return BaseAmount + index * Multiplier;
	}
}
