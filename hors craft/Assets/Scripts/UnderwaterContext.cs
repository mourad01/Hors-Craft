// DecompilerFi decompiler from Assembly-CSharp.dll class: UnderwaterContext
using UnityEngine;

public class UnderwaterContext : FactContext
{
	public Color waterColor;

	public override string GetContent()
	{
		return base.GetContent() + "Water color: " + waterColor.ToString();
	}
}
