// DecompilerFi decompiler from Assembly-CSharp.dll class: PositionSignalContext
using UnityEngine;

public class PositionSignalContext : SignalFactContext
{
	public Vector3 position;

	public override string GetContent()
	{
		return base.GetContent() + position.ToString();
	}
}
