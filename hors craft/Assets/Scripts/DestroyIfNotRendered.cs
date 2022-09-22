// DecompilerFi decompiler from Assembly-CSharp.dll class: DestroyIfNotRendered
using UnityEngine;

public class DestroyIfNotRendered : MonoBehaviour
{
	[Header("Leave empty to find first child")]
	public Renderer rend;

	private void Start()
	{
		if (rend == null)
		{
			rend = GetComponentInChildren<Renderer>();
			if (rend == null)
			{
				UnityEngine.Debug.LogWarning("DestroyIfNotRendered couldn't find renderer!");
				UnityEngine.Object.Destroy(this);
			}
		}
	}

	private void Update()
	{
		if (!rend.isVisible)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
