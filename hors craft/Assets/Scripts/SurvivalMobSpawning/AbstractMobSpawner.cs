// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.AbstractMobSpawner
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public abstract class AbstractMobSpawner : MonoBehaviour, ISurvivalContextListener
	{
		[Serializable]
		public class WaveSettings
		{
			public int allMonstersCount;

			public List<int> monstersIndexes = new List<int>();

			public List<int> monstersCount = new List<int>();

			public void RemoveMonster(int index)
			{
				int num = -1;
				for (int i = 0; i < monstersIndexes.Count; i++)
				{
					if (monstersIndexes[i] > index)
					{
						List<int> list;
						int index2;
						(list = monstersIndexes)[index2 = i] = list[index2] - 1;
					}
					else if (monstersIndexes[i] == index)
					{
						num = i;
					}
				}
				if (num != -1)
				{
					monstersIndexes.RemoveAt(num);
					monstersCount.RemoveAt(num);
				}
			}
		}

		[Serializable]
		public class WavesSettings
		{
			public List<GameObject> monsters = new List<GameObject>();

			public List<WaveSettings> waves = new List<WaveSettings>();

			public int GetMonsterIndex(GameObject prefab)
			{
				for (int i = 0; i < monsters.Count; i++)
				{
					if (monsters[i] == prefab)
					{
						return i;
					}
				}
				return -1;
			}

			public void AddNewWave()
			{
				waves.Add(new WaveSettings
				{
					monstersIndexes = new List<int>(),
					monstersCount = new List<int>()
				});
			}
		}

		public WavesSettings waves = new WavesSettings();

		[HideInInspector]
		public bool isSpawning;

		protected Coroutine spawnningCoroutine;

		protected SurvivalManager survivalManager;

		public float spawnRadiusMin = 15f;

		public float spawnRadiusMax = 25f;

		public float spawnForwardOfPlayerFactor = 0.25f;

		public int startLoopingFrom;

		public int monsterCountProgression;

		public bool spawnByWeight;

		public bool stopSpawnIfTooMany;

		public int stopTreshold;

		public bool spawnOnWater;

		public bool addAmmoDrop;

		public Action onStartSpawning;

		public Action onStopSpawning;

		private GameObject _player;

		protected AbstractSpawnBatchBehaviour spawnBatchBehaviour;

		protected AbstractSpawnPlacingBehaviour spawnPlacingBehaviour;

		protected AbstractSpawnSpawningBehaviour[] spawningBehaviours;

		protected GameObject player
		{
			get
			{
				if (_player == null && survivalManager != null)
				{
					_player = survivalManager.player;
				}
				return _player;
			}
		}

		protected abstract Type[] contextTypes
		{
			get;
		}

		public virtual void Init(SurvivalManager manager)
		{
			survivalManager = manager;
			spawnBatchBehaviour = GetComponent<AbstractSpawnBatchBehaviour>();
			spawnPlacingBehaviour = GetComponent<AbstractSpawnPlacingBehaviour>();
			spawnPlacingBehaviour.Init(spawnRadiusMin, spawnRadiusMax, spawnForwardOfPlayerFactor);
			spawningBehaviours = GetComponents<AbstractSpawnSpawningBehaviour>();
		}

		public virtual void OnContextsUpdated()
		{
			if (Manager.Get<StateMachineManager>().IsCurrentStateA<GameplayState>())
			{
				if (isSpawning && CanStopSpawning())
				{
					StopSpawning();
					PostStopSpawning();
				}
				else if (!isSpawning && CanStartSpawning())
				{
					PreStartSpawning();
					StartSpawning();
				}
			}
		}

		public abstract bool CanStartSpawning();

		public abstract bool CanStopSpawning();

		public virtual void PreStartSpawning()
		{
		}

		public virtual void StartSpawning()
		{
			isSpawning = true;
			spawnningCoroutine = StartCoroutine(SpawningCoroutine());
			if (onStartSpawning != null)
			{
				onStartSpawning();
			}
		}

		public virtual void PostStopSpawning()
		{
		}

		public virtual void StopSpawning()
		{
			isSpawning = false;
			if (spawnningCoroutine != null)
			{
				StopCoroutine(spawnningCoroutine);
			}
			if (onStopSpawning != null)
			{
				onStopSpawning();
			}
		}

		protected abstract IEnumerator SpawningCoroutine();

		protected void SpawnMonster(GameObject prefab, int wave, int iteration, int index)
		{
			SpawnMonster(prefab, spawnPlacingBehaviour.GetRandomPosition(prefab, player), wave, iteration, index);
		}

		protected void SpawnMonster(GameObject prefab, Vector3 position, int wave, int iteration, int index)
		{
			GameObject spawnedMonster = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
			for (int i = 0; i < spawningBehaviours.Length; i++)
			{
				spawningBehaviours[i].AfterSpawn(spawnedMonster, wave, iteration, index);
			}
		}

		public bool AddBySurvivalManager()
		{
			return true;
		}

		public Type[] ContextTypes()
		{
			return contextTypes;
		}

		Action[] ISurvivalContextListener.OnContextsUpdated()
		{
			return new Action[1]
			{
				OnContextsUpdated
			};
		}
	}
}
