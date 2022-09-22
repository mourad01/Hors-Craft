// DecompilerFi decompiler from Assembly-CSharp.dll class: RotateObjectContext
using UnityEngine;

public class RotateObjectContext : RotateContext
{
	public GameObject obj;

	public override string GetContent()
	{
		return base.GetContent() + "object: " + ((!(obj != null)) ? string.Empty : obj.name);
	}
}
