// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.WeightSingleMonsterBatch
using System.Linq;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class WeightSingleMonsterBatch : AbstractSpawnBatchBehaviour
	{
		protected struct MonsterInfo
		{
			public GameObject prefab;

			public int index;
		}

		public override MonstersBatch[] GetBatches(int wave, int iteration, AbstractMobSpawner.WavesSettings waves)
		{
			int num = waves.waves[wave].allMonstersCount + iteration;
			MonstersBatch[] array = new MonstersBatch[num];
			for (int i = 0; i < num; i++)
			{
				MonsterInfo randomMonsterFromWave = GetRandomMonsterFromWave(wave, waves);
				array[i] = new MonstersBatch
				{
					monsters = new GameObject[1]
					{
						randomMonsterFromWave.prefab
					},
					monsterCounts = new int[1]
					{
						1
					},
					monsterIndexes = new int[1]
					{
						randomMonsterFromWave.index
					},
					wave = wave,
					iteration = iteration
				};
			}
			return array;
		}

		private static MonsterInfo GetRandomMonsterFromWave(int wave, AbstractMobSpawner.WavesSettings waves)
		{
			int num = Random.Range(0, waves.waves[wave].monstersCount.Sum());
			int i = 0;
			for (float num2 = 0f; i < waves.waves[wave].monstersCount.Count && (float)num > (float)waves.waves[wave].monstersCount[i] + num2; i++)
			{
				num2 += (float)waves.waves[wave].monstersCount[i];
			}
			MonsterInfo result = default(MonsterInfo);
			result.prefab = waves.monsters[waves.waves[wave].monstersIndexes[i]];
			result.index = waves.waves[wave].monstersIndexes[i];
			return result;
		}
	}
}
