// DecompilerFi decompiler from Assembly-CSharp.dll class: CrosshairTarget
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
	private void Start()
	{
		CrosshairFrustumDetector componentInChildren = GetComponentInChildren<CrosshairFrustumDetector>();
		if (componentInChildren == null)
		{
			Renderer componentInChildren2 = GetComponentInChildren<Renderer>();
			if ((bool)componentInChildren2)
			{
				CrosshairFrustumDetector crosshairFrustumDetector = componentInChildren2.gameObject.AddComponent<CrosshairFrustumDetector>();
				crosshairFrustumDetector.enemy = base.gameObject;
			}
		}
	}

	private void Update()
	{
	}
}
