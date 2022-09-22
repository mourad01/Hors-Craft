// DecompilerFi decompiler from Assembly-CSharp.dll class: Loot
using Uniblocks;
using UnityEngine;

public class Loot : MonoBehaviour
{
	public LootInfo[] lootInfo;

	private const float DROP_RANGE = 1.5f;

	public void DropLoot()
	{
		LootInfo[] array = this.lootInfo;
		foreach (LootInfo lootInfo in array)
		{
			Vector3 position = base.transform.position;
			for (int j = 0; j < lootInfo.quantity; j++)
			{
				float x = position.x + UnityEngine.Random.Range(-1f, 1f) * 1.5f;
				float z = position.z + UnityEngine.Random.Range(-1f, 1f) * 1.5f;
				SpawnLoot(lootPosition: new Vector3(x, position.y + 0.45f, z), lootId: lootInfo.id);
			}
		}
	}

	private void SpawnLoot(ushort lootId, Vector3 lootPosition)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Engine.GetVoxelGameObject(lootId), lootPosition, Quaternion.identity);
		gameObject.transform.SetParent(null);
		CollectibleVoxel component = gameObject.GetComponent<CollectibleVoxel>();
		if (component != null)
		{
			component.id = lootId;
		}
	}
}
