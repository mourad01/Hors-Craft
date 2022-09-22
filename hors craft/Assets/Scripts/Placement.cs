// DecompilerFi decompiler from Assembly-CSharp.dll class: Placement
using System;
using UnityEngine;

[Serializable]
public struct Placement
{
	public DressupSkin.Placement placement;

	public Transform[] transforms;

	public Transform GetPlacement()
	{
		int num = UnityEngine.Random.Range(0, transforms.Length);
		return transforms[num];
	}
}
