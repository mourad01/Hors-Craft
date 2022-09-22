// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.TimeBaseMobSpawner
using Common.Utils;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class TimeBaseMobSpawner : AbstractMobSpawner
	{
		private DayTimeContext _context;

		public bool fixedSpawnTime;

		public bool spawnAtNight = true;

		public float startSpawnTime;

		public float endSpawnTime;

		public float spawnTimeFill = 1f;

		private readonly Queue<AbstractSpawnBatchBehaviour.MonstersBatch> toSpawnMonsters = new Queue<AbstractSpawnBatchBehaviour.MonstersBatch>();

		private readonly Queue<GameObject> spawnedMonsters = new Queue<GameObject>();

		protected DayTimeContext context => _context ?? (_context = SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>());

		public float spawnInterval
		{
			get;
			private set;
		}

		protected override Type[] contextTypes => new Type[1]
		{
			typeof(DayTimeContext)
		};

		public override void OnContextsUpdated()
		{
			if (context != null)
			{
				base.OnContextsUpdated();
			}
		}

		public override bool CanStartSpawning()
		{
			if (context == null || isSpawning)
			{
				return false;
			}
			if (fixedSpawnTime)
			{
				return FixedTimeReq();
			}
			return FlexTimeReq();
		}

		public override bool CanStopSpawning()
		{
			if (context == null || !isSpawning)
			{
				return false;
			}
			if (fixedSpawnTime)
			{
				return !FixedTimeReq();
			}
			return !FlexTimeReq();
		}

		public override void PreStartSpawning()
		{
			if (spawnedMonsters.Count > 0)
			{
				ClearSpawned();
			}
			toSpawnMonsters.Clear();
			int wave = GetWave();
			int loop = GetLoop();
			toSpawnMonsters.EnqueueRange(spawnBatchBehaviour.GetBatches(wave, loop, waves));
			SunController instance = SunController.instance;
			if (fixedSpawnTime)
			{
				if (startSpawnTime < endSpawnTime)
				{
					spawnInterval = (endSpawnTime - startSpawnTime) / (float)toSpawnMonsters.Count;
				}
				else
				{
					spawnInterval = (1f - startSpawnTime + endSpawnTime) / (float)toSpawnMonsters.Count;
				}
			}
			else if (spawnAtNight)
			{
				spawnInterval = (1f - instance.nightStartTime + instance.dayStartTime) / (float)toSpawnMonsters.Count;
			}
			else
			{
				spawnInterval = (instance.nightStartTime - instance.dayStartTime) / (float)toSpawnMonsters.Count;
			}
			spawnInterval *= instance.secondsInFullDay;
			spawnInterval *= spawnTimeFill;
		}

		public override void StartSpawning()
		{
			base.StartSpawning();
			survivalManager.TakeCombatSemaphore();
		}

		public override void PostStopSpawning()
		{
			ClearSpawned();
			survivalManager.ReleaseCombatSemaphore();
		}

		protected override IEnumerator SpawningCoroutine()
		{
			WaitForSeconds wait = new WaitForSeconds(spawnInterval);
			while (toSpawnMonsters.Count > 0 && isSpawning)
			{
				toSpawnMonsters.Dequeue().SpawnBatch(base.SpawnMonster);
				yield return wait;
			}
		}

		private bool FixedTimeReq()
		{
			if (startSpawnTime < endSpawnTime)
			{
				return context.time > startSpawnTime && context.time < endSpawnTime;
			}
			return context.time > startSpawnTime || context.time < endSpawnTime;
		}

		private bool FlexTimeReq()
		{
			if (spawnAtNight)
			{
				return context.dayTime == GlobalSettings.TimeOfDay.NIGHT;
			}
			return context.dayTime == GlobalSettings.TimeOfDay.MIDDAY;
		}

		private void ClearSpawned()
		{
			while (spawnedMonsters.Count > 0)
			{
				GameObject gameObject = spawnedMonsters.Dequeue();
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			spawnedMonsters.Clear();
		}

		public int GetWave()
		{
			return (context.day < waves.waves.Count) ? context.day : (startLoopingFrom + (context.day - startLoopingFrom) % (waves.waves.Count - startLoopingFrom));
		}

		public int GetLoop()
		{
			return ((context.day >= waves.waves.Count) ? ((context.day - (waves.waves.Count - 1)) / (waves.waves.Count - startLoopingFrom) + 1) : 0) * monsterCountProgression;
		}
	}
}
