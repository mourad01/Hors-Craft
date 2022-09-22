// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.DropItemsSpawning
using ItemVInventory;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class DropItemsSpawning : AbstractSpawnSpawningBehaviour
	{
		public int inTierMonstersCount = 4;

		public override void AfterSpawn(GameObject spawnedMonster, int wave, int iteration, int index)
		{
			int tier = index / inTierMonstersCount;
			int monsterIndex = index % inTierMonstersCount;
			ItemDroperByDeath itemDroperByDeath = spawnedMonster.AddComponent<ItemDroperByDeath>();
			itemDroperByDeath.monsterIndex = monsterIndex;
			itemDroperByDeath.tier = tier;
		}
	}
}
