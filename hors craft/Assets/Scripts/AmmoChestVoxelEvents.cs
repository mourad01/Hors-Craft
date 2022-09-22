// DecompilerFi decompiler from Assembly-CSharp.dll class: AmmoChestVoxelEvents
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class AmmoChestVoxelEvents : DefaultVoxelEvents
{
	public static HashSet<string> openedChest = new HashSet<string>();

	public GameObject[] ammoPrefabs;

	public override void OnBlockDestroy(VoxelInfo voxelInfo)
	{
		Vector3 lootPosition = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
		SpawnLoot(lootPosition);
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.FindTreasureXTimes);
		openedChest.Add(voxelInfo.index.ToString());
		base.OnBlockDestroy(voxelInfo);
	}

	private void SpawnLoot(Vector3 lootPosition)
	{
		int chestCoins = Manager.Get<ModelManager>().chestSettings.GetChestCoins();
		int num = Random.Range(1, 6);
		int lootValue = chestCoins / num;
		for (int i = 0; i < num; i++)
		{
			if (Random.value <= 0.2f)
			{
				SpawnAmmo(lootPosition, lootValue);
			}
			else
			{
				SpawnResource(lootPosition);
			}
		}
	}

	private void SpawnAmmo(Vector3 lootPosition, int lootValue)
	{
		GameObject gameObject = Object.Instantiate(ammoPrefabs[Random.Range(0, ammoPrefabs.Length)]);
		gameObject.GetComponent<AmmoObject>().Init(Mathf.Clamp(lootValue, 1, 4), lootPosition, addRandomForce: true);
	}

	private void SpawnResource(Vector3 lootPosition)
	{
		CraftingManager craftingManager = Manager.Get<CraftingManager>();
		if ((bool)craftingManager)
		{
			craftingManager.SpawnRandomResource(lootPosition, spawnWithRandomForce: true);
		}
	}
}
