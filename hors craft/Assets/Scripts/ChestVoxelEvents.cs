// DecompilerFi decompiler from Assembly-CSharp.dll class: ChestVoxelEvents
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class ChestVoxelEvents : DefaultVoxelEvents, ISpawnableVoxelEvent
{
	public static HashSet<string> openedChest = new HashSet<string>();

	public GameObject coinToSpawn;

	public GameObject[] GetPrefabs()
	{
		return new GameObject[1]
		{
			coinToSpawn
		};
	}

	public override void OnBlockDestroy(VoxelInfo voxelInfo)
	{
		int chestCoins = Manager.Get<ModelManager>().chestSettings.GetChestCoins();
		Vector3 lootPosition = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
		if (Manager.Get<ModelManager>().chestSettings.GetChestSpawnsResources() == 1f)
		{
			SpawnLoot(lootPosition, chestCoins);
		}
		else
		{
			Spawn(chestCoins, lootPosition);
		}
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.FindTreasureXTimes);
		openedChest.Add(voxelInfo.index.ToString());
		base.OnBlockDestroy(voxelInfo);
	}

	private void Spawn(int value, Vector3 lootPosition)
	{
		int num = Random.Range(3, 6);
		int value2 = value / num;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(coinToSpawn);
			gameObject.GetComponent<CoinObject>().Init(value2, lootPosition, addRandomForce: true);
		}
	}

	private void SpawnLoot(Vector3 lootPosition, int currencyValue)
	{
		CraftingManager craftingManager = Manager.Get<CraftingManager>();
		if ((bool)craftingManager)
		{
			for (int i = 0; i < currencyValue; i++)
			{
				craftingManager.SpawnRandomResource(lootPosition, spawnWithRandomForce: true);
			}
		}
	}
}
