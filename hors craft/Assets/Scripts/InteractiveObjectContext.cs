// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveObjectContext
using System;
using UnityEngine;

public class InteractiveObjectContext : FactContext
{
	public GameObject obj;

	public Action useAction;

	public override string GetContent()
	{
		return base.GetContent() + "object: " + ((!(obj != null)) ? string.Empty : obj.name);
	}
}
