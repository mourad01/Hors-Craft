// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.CountSingleMonsterBatch
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class CountSingleMonsterBatch : AbstractSpawnBatchBehaviour
	{
		public override MonstersBatch[] GetBatches(int wave, int iteration, AbstractMobSpawner.WavesSettings waves)
		{
			List<MonstersBatch> list = new List<MonstersBatch>();
			for (int i = 0; i < waves.waves[wave].monstersCount.Count; i++)
			{
				for (int j = 0; j < waves.waves[wave].monstersCount[i]; j++)
				{
					list.Add(new MonstersBatch
					{
						monsters = new GameObject[1]
						{
							waves.monsters[waves.waves[wave].monstersIndexes[i]]
						},
						monsterCounts = new int[1]
						{
							1
						},
						monsterIndexes = new int[1]
						{
							waves.waves[wave].monstersIndexes[i]
						},
						wave = wave,
						iteration = iteration
					});
				}
			}
			for (int k = 0; k < iteration; k++)
			{
				int index = waves.waves[wave].monstersCount[Random.Range(0, waves.waves[wave].monstersCount.Count)];
				list.Add(new MonstersBatch
				{
					monsters = new GameObject[1]
					{
						waves.monsters[waves.waves[wave].monstersIndexes[index]]
					},
					monsterCounts = new int[1]
					{
						1
					},
					monsterIndexes = new int[1]
					{
						waves.waves[wave].monstersIndexes[index]
					},
					wave = wave,
					iteration = iteration
				});
			}
			return list.Shuffle().ToArray();
		}
	}
}
