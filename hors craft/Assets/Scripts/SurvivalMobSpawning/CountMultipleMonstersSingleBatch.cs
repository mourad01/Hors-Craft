// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.CountMultipleMonstersSingleBatch
using System;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class CountMultipleMonstersSingleBatch : AbstractSpawnBatchBehaviour
	{
		public override MonstersBatch[] GetBatches(int wave, int iteration, AbstractMobSpawner.WavesSettings waves)
		{
			MonstersBatch monstersBatch = new MonstersBatch();
			monstersBatch.monsters = new GameObject[waves.waves[wave].monstersCount.Count];
			monstersBatch.monsterCounts = new int[waves.waves[wave].monstersCount.Count];
			monstersBatch.monsterIndexes = new int[waves.waves[wave].monstersCount.Count];
			monstersBatch.wave = wave;
			monstersBatch.iteration = iteration;
			MonstersBatch monstersBatch2 = monstersBatch;
			for (int i = 0; i < waves.waves[wave].monstersCount.Count; i++)
			{
				monstersBatch2.monsterCounts[i] = waves.waves[wave].monstersCount[i];
				monstersBatch2.monsters[i] = waves.monsters[waves.waves[wave].monstersIndexes[i]];
				monstersBatch2.monsterIndexes[i] = waves.waves[wave].monstersIndexes[i];
			}
			for (int j = 0; j < iteration; j++)
			{
				int index = waves.waves[wave].monstersCount[UnityEngine.Random.Range(0, waves.waves[wave].monstersCount.Count)];
				GameObject value = waves.monsters[waves.waves[wave].monstersIndexes[index]];
				int num = Array.IndexOf(monstersBatch2.monsters, value);
				monstersBatch2.monsterCounts[num]++;
			}
			return new MonstersBatch[1]
			{
				monstersBatch2
			};
		}
	}
}
