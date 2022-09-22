// DecompilerFi decompiler from Assembly-CSharp.dll class: TriggerIndicator
using UnityEngine;

public class TriggerIndicator : MonoBehaviour
{
	public bool active => activeCollider != null;

	public Collider activeCollider
	{
		get;
		private set;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!collider.isTrigger)
		{
			activeCollider = collider;
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (!collider.isTrigger)
		{
			activeCollider = collider;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!collider.isTrigger)
		{
			activeCollider = null;
		}
	}
}
