// DecompilerFi decompiler from Assembly-CSharp.dll class: DestroyUtil
using UnityEngine;

public class DestroyUtil
{
	public static void DestroyIfNotNull(GameObject go)
	{
		if (go != null)
		{
			UnityEngine.Object.Destroy(go);
		}
	}
}
