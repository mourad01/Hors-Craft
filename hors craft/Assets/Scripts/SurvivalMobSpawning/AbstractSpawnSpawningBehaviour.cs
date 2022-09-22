// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.AbstractSpawnSpawningBehaviour
using UnityEngine;

namespace SurvivalMobSpawning
{
	public abstract class AbstractSpawnSpawningBehaviour : MonoBehaviour
	{
		public abstract void AfterSpawn(GameObject spawnedMonster, int wave, int iteration, int index);
	}
}
