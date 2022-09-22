// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.CountMultipleMonstersBatch
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class CountMultipleMonstersBatch : AbstractSpawnBatchBehaviour
	{
		public override MonstersBatch[] GetBatches(int wave, int iteration, AbstractMobSpawner.WavesSettings waves)
		{
			List<MonstersBatch> list = new List<MonstersBatch>();
			for (int i = 0; i < waves.waves[wave].monstersCount.Count; i++)
			{
				list.Add(new MonstersBatch
				{
					monsters = new GameObject[1]
					{
						waves.monsters[waves.waves[wave].monstersIndexes[i]]
					},
					monsterCounts = new int[1]
					{
						waves.waves[wave].monstersCount[i]
					},
					monsterIndexes = new int[1]
					{
						waves.waves[wave].monstersIndexes[i]
					},
					wave = wave,
					iteration = iteration
				});
			}
			for (int j = 0; j < iteration; j++)
			{
				int index = waves.waves[wave].monstersCount[UnityEngine.Random.Range(0, waves.waves[wave].monstersCount.Count)];
				GameObject monster = waves.monsters[waves.waves[wave].monstersIndexes[index]];
				MonstersBatch monstersBatch = list.FirstOrDefault((MonstersBatch batch) => batch.monsters.Contains(monster));
				int num = Array.IndexOf(monstersBatch.monsters, monster);
				monstersBatch.monsterCounts[num]++;
			}
			return list.ToArray();
		}
	}
}
