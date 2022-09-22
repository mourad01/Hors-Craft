// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.DropAmmoSpawning
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class DropAmmoSpawning : AbstractSpawnSpawningBehaviour
	{
		public override void AfterSpawn(GameObject spawnedMonster, int wave, int iteration, int index)
		{
			if (SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>() != null)
			{
				spawnedMonster.AddComponent<DropAmmo>().ammoPrefabs = SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>().ammoPrefabs;
			}
		}
	}
}
