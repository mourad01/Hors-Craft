// DecompilerFi decompiler from Assembly-CSharp.dll class: CollisionDetector
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
	public LayerMask layerMask;

	public MonoBehaviour[] receivers = new MonoBehaviour[0];

	private void OnCollisionEnter(Collision collision)
	{
		for (int i = 0; i < receivers.Length; i++)
		{
			if (receivers[i] is ICollisionReceiver && collision.gameObject.IsInLayerMask(layerMask))
			{
				(receivers[i] as ICollisionReceiver).OnCollision(collision.transform);
			}
		}
	}
}
