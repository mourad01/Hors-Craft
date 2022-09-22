// DecompilerFi decompiler from Assembly-CSharp.dll class: Toast
using UnityEngine;

public class Toast : TopNotification
{
	public override void PrepareToShow(bool setAtTop)
	{
		base.PrepareToShow(setAtTop);
		base.transform.localPosition = Vector3.zero;
	}
}
