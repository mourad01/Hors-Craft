// DecompilerFi decompiler from Assembly-CSharp.dll class: CrosshairFrustumDetector
using UnityEngine;

public class CrosshairFrustumDetector : MonoBehaviour
{
	public GameObject enemy;

	private void OnBecameVisible()
	{
		if (Crosshair.instance != null)
		{
			Crosshair.instance.AddEnemy(enemy);
		}
	}

	private void OnBecameInvisible()
	{
		if (Crosshair.instance != null)
		{
			Crosshair.instance.RemoveEnemy(enemy);
		}
	}
}
