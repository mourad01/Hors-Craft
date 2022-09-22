// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GenerateMechBasedOnStatsSpawnBehaviour
using Mobs;
using SurvivalMobSpawning;
using UnityEngine;

namespace Gameplay
{
	public class GenerateMechBasedOnStatsSpawnBehaviour : AbstractSpawnSpawningBehaviour
	{
		public override void AfterSpawn(GameObject spawnedMonster, int wave, int iteration, int index)
		{
			float maxHp = spawnedMonster.GetComponent<Health>().maxHp;
			float num = spawnedMonster.GetComponent<MechEnemy>().dmgShoot;
			float wanderSpeed = spawnedMonster.GetComponent<MechEnemy>().wanderSpeed;
			int index2 = (int)(maxHp / 20f);
			int num2 = (int)(num / 12f);
			int index3 = (int)((wanderSpeed - 3f) / 1.2f);
			int index4 = num2 + Random.Range(-1, 2);
			FormChanger componentInChildren = spawnedMonster.GetComponentInChildren<FormChanger>();
			componentInChildren.SetPart("body", index2);
			componentInChildren.SetPart("weapon", num2);
			componentInChildren.SetPart("legs", index3);
			componentInChildren.SetPart("arms", index4);
		}
	}
}
