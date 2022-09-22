// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.AbstractSpawnBatchBehaviour
using System;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public abstract class AbstractSpawnBatchBehaviour : MonoBehaviour
	{
		[Serializable]
		public class MonstersBatch
		{
			public GameObject[] monsters;

			public int[] monsterCounts;

			public int[] monsterIndexes;

			public int wave;

			public int iteration;

			public void SpawnBatch(Action<GameObject, int, int, int> spawnMethod)
			{
				for (int i = 0; i < monsters.Length; i++)
				{
					for (int j = 0; j < monsterCounts[i]; j++)
					{
						spawnMethod(monsters[i], wave, iteration, monsterIndexes[i]);
					}
				}
			}
		}

		public abstract MonstersBatch[] GetBatches(int wave, int iteration, AbstractMobSpawner.WavesSettings waves);
	}
}
