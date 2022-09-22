// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.AddHPDisplay
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class AddHPDisplay : AbstractSpawnSpawningBehaviour
	{
		public GameObject healthDisplayPrefab;

		public override void AfterSpawn(GameObject spawnedMonster, int wave, int iteration, int index)
		{
			Object.Instantiate(healthDisplayPrefab).transform.SetParent(spawnedMonster.transform);
		}
	}
}
