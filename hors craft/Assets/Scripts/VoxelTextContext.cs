// DecompilerFi decompiler from Assembly-CSharp.dll class: VoxelTextContext
using UnityEngine;

public class VoxelTextContext : FactContext
{
	public Vector2 screenPosition;

	public Vector3 scale = Vector3.one;

	public string text;

	public bool hasToShow;

	public override string GetContent()
	{
		return base.GetContent() + "pos: " + screenPosition + " scale: " + scale + " value: " + text;
	}
}
