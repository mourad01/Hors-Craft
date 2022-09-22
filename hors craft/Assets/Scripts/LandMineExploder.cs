// DecompilerFi decompiler from Assembly-CSharp.dll class: LandMineExploder
using UnityEngine;

public class LandMineExploder : VoxelExplosion
{
	public int baseDmg = 1;

	private void OnTriggerEnter(Collider collider)
	{
		IFighting componentInParent = collider.gameObject.GetComponentInParent<IFighting>();
		if (componentInParent != null && componentInParent.IsEnemy())
		{
			baseDamage = baseDmg;
			ExplodeAtCollider(collider);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
