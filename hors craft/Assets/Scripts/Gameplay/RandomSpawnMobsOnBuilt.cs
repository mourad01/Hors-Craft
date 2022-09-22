// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RandomSpawnMobsOnBuilt
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "SpawnMobs", menuName = "ScriptableObjects/On Built/Spawn Mobs", order = 1)]
	public class RandomSpawnMobsOnBuilt : OnBuiltAction
	{
		public GameObject mob;

		public override void OnBuilt(PlacedBlueprintData data)
		{
		}
	}
}
