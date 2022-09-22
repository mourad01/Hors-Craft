// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraNearbyGroundCheck
using UnityEngine;

public class CameraNearbyGroundCheck : MonoBehaviour
{
	public bool groundNearby;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 30)
		{
			groundNearby = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 30)
		{
			groundNearby = false;
		}
	}
}
