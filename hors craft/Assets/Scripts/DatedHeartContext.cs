// DecompilerFi decompiler from Assembly-CSharp.dll class: DatedHeartContext
using UnityEngine;

public class DatedHeartContext : FactContext
{
	public Vector3 heartStartPosition;

	public float progress;

	public float value;

	public override string GetContent()
	{
		return base.GetContent() + "pos: " + heartStartPosition + " progress: " + progress + " value: " + value;
	}
}
