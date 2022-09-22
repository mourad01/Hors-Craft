// DecompilerFi decompiler from Assembly-CSharp.dll class: ResourceSpawnDestoyBehaviour
using Common.Managers;
using UnityEngine;

public class ResourceSpawnDestoyBehaviour : AbstractInteractiveDestroyBehaviour
{
	public int minResources = 1;

	public int maxResources = 2;

	public ushort[] resourcesId;

	public override void Destroy(GameObject toDestroy)
	{
		int num = Random.Range(minResources, maxResources + 1);
		for (int i = 0; i < num; i++)
		{
			Vector3 lootPosition = toDestroy.transform.position + Random.insideUnitSphere;
			lootPosition.y += 4f;
			Manager.Get<CraftingManager>().SpawnResource(lootPosition, GetRandomResource());
		}
	}

	private ushort GetRandomResource()
	{
		return resourcesId[Random.Range(0, resourcesId.Length)];
	}
}
