// DecompilerFi decompiler from Assembly-CSharp.dll class: Bomb
using Common.Behaviours;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	[SerializeField]
	private GameObject explosionAnimation;

	[SerializeField]
	private float baseDamage = 1f;

	[SerializeField]
	private float explosionRange = 5f;

	private void OnCollisionEnter(Collision collision)
	{
		Collider[] gameObjectsInRange = GetGameObjectsInRange(base.transform.position);
		Collider[] array = gameObjectsInRange;
		foreach (Collider collider in array)
		{
			Health componentInParent = collider.gameObject.GetComponentInParent<Health>();
			if ((bool)componentInParent && !collider.GetComponent<CharacterController>())
			{
				componentInParent.OnHit(baseDamage, Vector3.up);
			}
		}
		CreateFakeBulletExplosion();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private Collider[] GetGameObjectsInRange(Vector3 center)
	{
		return Physics.OverlapSphere(center, explosionRange);
	}

	private void CreateFakeBulletExplosion()
	{
		GameObject gameObject = new GameObject("Explosion");
		gameObject.transform.position = base.transform.position;
		VoxelExplosion voxelExplosion = gameObject.AddComponent<VoxelExplosion>();
		voxelExplosion.radius = explosionRange;
		voxelExplosion.maxDamage = 0f;
		voxelExplosion.baseDamage = 0f;
		voxelExplosion.ExplodeArea(digDown: true, canDestroyAllBlocks: true);
		UnityEngine.Object.Destroy(gameObject);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(explosionAnimation);
		gameObject2.name = "Explosion animation";
		gameObject2.transform.position = base.transform.position;
		ParticleSystem componentInChildren = explosionAnimation.GetComponentInChildren<ParticleSystem>();
		componentInChildren.playOnAwake = true;
		componentInChildren.Play();
		DestroyAfter destroyAfter = gameObject2.AddComponent<DestroyAfter>();
		destroyAfter.delay = componentInChildren.main.duration;
	}
}
