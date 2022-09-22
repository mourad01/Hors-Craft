// DecompilerFi decompiler from Assembly-CSharp.dll class: MobsManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MobsManager : Manager, IGameCallbacksListener
{
	[Serializable]
	public class MobSpawnConfig
	{
		public GameObject prefab;

		public float weight = 1f;

		public int spawnCountFrom = 1;

		public int spawnCountTo = 10;

		public SpawnTime spawnTime = SpawnTime.BOTH;

		[HideInInspector]
		public MobType mobType;

		public float desiredYRotation;

		public bool canSpawnOnGround => mobType == MobType.GROUND || mobType == MobType.FLYING;

		public bool canSpawnInWater => mobType == MobType.SWIMMING || mobType == MobType.FLYING;

		public MobSpawnConfig()
		{
		}

		public MobSpawnConfig(GameObject prefab)
		{
			this.prefab = prefab;
		}
	}

	public enum MobType
	{
		GROUND,
		FLYING,
		SWIMMING,
		VEHICLE
	}

	public enum SpawnTime
	{
		DAY,
		NIGHT,
		BOTH
	}

	public class PlannedSpawn
	{
		public MobSpawnConfig config;

		public Vector3 pos;

		public float spawnTime;

		public int petId;

		public PetManager.PetIndividualData pet;

		public float randomNumber;

		public float yRotation;
	}

	[Header("Use this value when you do not have PetsManager")]
	public bool talkWithMobs;

	[Space(10f)]
	public MobSpawnConfig[] spawnConfigs;

	[Header("Additional spawn modifiers")]
	public float scaleEveryMobBy = 1f;

	public float spawnProbability = 1f;

	private float sumOfWeights;

	private float waterSumOfWeights;

	private float vehicleSumOfWeights;

	private List<ChunkData> alreadyRegisteredChunks;

	private LayerMask mobsLayerMask;

	private bool isSurvival;

	private MobsContainer _mobsContainer;

	public Dictionary<int, int> mobIndexToEngineIndex;

	public Dictionary<string, int> mobNameToEngineIndex;

	[HideInInspector]
	public List<GameObject> _spawnedMobs = new List<GameObject>();

	private static GameObject mobParent;

	private AbstractMobConfig[] mobConfigs;

	private const float RADIUS_CHANGE_PER_SPAWN = 2f;

	private List<PlannedSpawn> plannedSpawns = new List<PlannedSpawn>();

	public MobsContainer mobsContainer
	{
		get
		{
			if (_mobsContainer == null)
			{
				_mobsContainer = UnityEngine.Object.FindObjectOfType<MobsContainer>();
			}
			return _mobsContainer;
		}
	}

	public List<GameObject> spawnedMobs
	{
		get
		{
			RevalidateMobsList();
			return _spawnedMobs;
		}
	}

	public void RevalidateMobsList()
	{
		_spawnedMobs.RemoveAll((GameObject mob) => mob == null);
	}

	public override void Init()
	{
		mobConfigs = GetComponentsInChildren<AbstractMobConfig>();
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		isSurvival = Manager.Contains<SurvivalManager>();
		alreadyRegisteredChunks = new List<ChunkData>();
		sumOfWeights = 0f;
		waterSumOfWeights = 0f;
		AssignMobTypes();
		MobSpawnConfig[] array = spawnConfigs;
		foreach (MobSpawnConfig mobSpawnConfig in array)
		{
			if (mobSpawnConfig.canSpawnInWater)
			{
				waterSumOfWeights += mobSpawnConfig.weight;
			}
			if (mobSpawnConfig.mobType == MobType.VEHICLE)
			{
				vehicleSumOfWeights += mobSpawnConfig.weight;
			}
			if (mobSpawnConfig.canSpawnOnGround)
			{
				sumOfWeights += mobSpawnConfig.weight;
			}
		}
		mobsLayerMask = LayerMask.GetMask("Mobs");
		OnConfigInitialized();
	}

	private void OnConfigInitialized()
	{
		MobsContainer mobsContainer = UnityEngine.Object.FindObjectOfType<MobsContainer>();
		if (mobsContainer != null)
		{
			mobsContainer.OnManagerInitialized(spawnConfigs);
		}
	}

	public int GetEngineIndexByName(string name)
	{
		if (mobNameToEngineIndex != null && mobNameToEngineIndex.ContainsKey(name))
		{
			return mobNameToEngineIndex[name];
		}
		return -1;
	}

	public void AddSpawnersToBlocks()
	{
		if (mobParent == null)
		{
			mobIndexToEngineIndex = new Dictionary<int, int>();
			mobNameToEngineIndex = new Dictionary<string, int>();
			int num = 0;
			int baseLenght = Engine.Blocks.Length;
			mobParent = new GameObject("MobParent");
			SceneManager.MoveGameObjectToScene(mobParent, SceneManager.GetSceneByName("gameplay"));
			Dictionary<string, GameObject> dictionary = PrepareMobsToAdd();
			Array.Resize(ref Engine.Blocks, Engine.Blocks.Length + dictionary.Count);
			foreach (KeyValuePair<string, GameObject> item in dictionary)
			{
				AddVoxelToEngine(mobParent.transform, item.Value, item.Key, num, baseLenght);
				num++;
			}
		}
	}

	private Dictionary<string, GameObject> PrepareMobsToAdd()
	{
		MobsContainer mobsContainer = UnityEngine.Object.FindObjectOfType<MobsContainer>();
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		if (mobsContainer != null)
		{
			foreach (GameObject mob in mobsContainer.mobList)
			{
				dictionary[mob.name] = mob;
			}
			return dictionary;
		}
		return dictionary;
	}

	private void AddVoxelToEngine(Transform mobParent, GameObject prefab, string name, int index, int baseLenght)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.SetParent(mobParent);
		Voxel voxel = gameObject.AddComponent<Voxel>();
		voxel.blockCategory = Voxel.Category.none;
		if (prefab.GetComponent<Mob>() != null && prefab.GetComponent<Mob>().mobSprite != null)
		{
			voxel.voxelSprite = prefab.GetComponent<Mob>().mobSprite;
		}
		voxel.VColliderType = ColliderType.none;
		voxel.SetUniqueID((ushort)(baseLenght + index));
		voxel.isMob = true;
		voxel.priority = 300;
		gameObject.AddComponent<OneTimeMobSpawnerEvents>().prefab = prefab;
		Engine.Blocks[baseLenght + index] = voxel;
		Engine.EngineInstance.voxelToPriority[voxel] = 300;
		mobIndexToEngineIndex.Add(index, baseLenght + index);
		mobNameToEngineIndex[gameObject.name] = baseLenght + index;
	}

	private void AssignMobTypes()
	{
		MobSpawnConfig[] array = spawnConfigs;
		foreach (MobSpawnConfig config in array)
		{
			AssignMobType(config);
		}
	}

	private void AssignMobType(MobSpawnConfig config)
	{
		MobNavigator component = config.prefab.GetComponent<MobNavigator>();
		if (component is FlyingMobNavigator)
		{
			config.mobType = MobType.FLYING;
		}
		else if (component is SwimmingMobNavigator)
		{
			config.mobType = MobType.SWIMMING;
		}
		else if (component is CarNavigator)
		{
			config.mobType = MobType.VEHICLE;
		}
		else
		{
			config.mobType = MobType.GROUND;
		}
	}

	public bool IsThisMobHuman(int index)
	{
		if (index < spawnConfigs.Length)
		{
			return spawnConfigs[index].prefab.GetComponent<HumanMob>() != null;
		}
		return false;
	}

	public bool IsThisMobVehicle(int index)
	{
		if (index < spawnConfigs.Length)
		{
			return spawnConfigs[index].prefab.GetComponent<CarMob>() != null;
		}
		return false;
	}

	public void RegisterSpawnerOn(ChunkData chunk, Vector3 worldPos, byte spawnerRotation)
	{
		if (alreadyRegisteredChunks.Contains(chunk))
		{
			return;
		}
		DeterministicRNG deterministicRNG = new DeterministicRNG((int)(worldPos.x + worldPos.z));
		if (deterministicRNG.NextFloat() > spawnProbability)
		{
			return;
		}
		MobSpawnConfig mobSpawnConfig = null;
		if (IsItWater(chunk, worldPos))
		{
			mobSpawnConfig = PickConfig(deterministicRNG, (MobSpawnConfig item) => item.canSpawnInWater, waterSumOfWeights);
		}
		else if (IsOnRoad(chunk, worldPos))
		{
			mobSpawnConfig = PickConfig(deterministicRNG, (MobSpawnConfig item) => item.mobType == MobType.VEHICLE, vehicleSumOfWeights);
			if (mobSpawnConfig == null)
			{
				return;
			}
			mobSpawnConfig.desiredYRotation = getYAngleFromVoxelRotation(spawnerRotation);
		}
		else if (spawnConfigs.Length > 0)
		{
			mobSpawnConfig = PickConfig(deterministicRNG, (MobSpawnConfig item) => IsConfigStandardMob(item) && CanBeSpawnedAtCurrentDayTime(item), sumOfWeights, spawnConfigs[0]);
		}
		if (mobSpawnConfig != null)
		{
			float num = 1f;
			int num2 = deterministicRNG.Next(mobSpawnConfig.spawnCountFrom, mobSpawnConfig.spawnCountTo + 1);
			for (int i = 0; i < num2; i++)
			{
				Vector3 normalized = new Vector3(deterministicRNG.NextFloat() * 2f - 1f, 0f, deterministicRNG.NextFloat() * 2f - 1f).normalized;
				normalized = worldPos + normalized * num;
				DeterministicRNG deterministicRNG2 = new DeterministicRNG(normalized.GetHashCode() % 10000);
				PlanSpawn(mobSpawnConfig, normalized, -1, null, deterministicRNG2.NextFloat());
				num += 2f;
			}
			alreadyRegisteredChunks.Add(chunk);
		}
	}

	private float getYAngleFromVoxelRotation(byte rotation)
	{
		switch (rotation)
		{
		case 0:
			return 0f;
		case 1:
			return 180f;
		case 2:
			return 270f;
		case 3:
			return 90f;
		default:
			return 0f;
		}
	}

	public void ChunkDespawned(ChunkData chunk)
	{
		Vector3 vector = Vector3.one * (ChunkData.SideLength / 2);
		Vector3 center = ChunkData.IndexToPosition(chunk.ChunkIndex) + vector;
		Collider[] array = Physics.OverlapBox(center, vector, Quaternion.identity, mobsLayerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Mob componentInParent = collider.GetComponentInParent<Mob>();
			if (componentInParent != null)
			{
				componentInParent.Despawn();
			}
		}
		alreadyRegisteredChunks.Remove(chunk);
	}

	public void DespawnAll()
	{
		while (base.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
		}
	}

	public void RegisterCustomMobSpawner(ChunkData chunk, GameObject prefab, Vector3 worldPos, int countFrom, int countTo, float yRotation = -1f)
	{
		if (!alreadyRegisteredChunks.Contains(chunk))
		{
			DeterministicRNG deterministicRNG = new DeterministicRNG((int)(worldPos.x + worldPos.z));
			MobSpawnConfig mobSpawnConfig = new MobSpawnConfig();
			mobSpawnConfig.prefab = prefab;
			mobSpawnConfig.spawnCountFrom = countFrom;
			mobSpawnConfig.spawnCountTo = countTo;
			mobSpawnConfig.weight = 1f;
			MobSpawnConfig mobSpawnConfig2 = mobSpawnConfig;
			AssignMobType(mobSpawnConfig2);
			int num = deterministicRNG.Next(mobSpawnConfig2.spawnCountFrom, mobSpawnConfig2.spawnCountTo + 1);
			float num2 = (num > 1) ? 1 : 0;
			for (int i = 0; i < num; i++)
			{
				Vector3 normalized = new Vector3(deterministicRNG.NextFloat() * 2f - 1f, 0f, deterministicRNG.NextFloat() * 2f - 1f).normalized;
				normalized = worldPos + normalized * num2;
				DeterministicRNG deterministicRNG2 = new DeterministicRNG(normalized.GetHashCode() % 10000);
				PlanSpawn(mobSpawnConfig2, normalized, -1, null, deterministicRNG2.NextFloat(), yRotation);
				num2 += 2f;
			}
			alreadyRegisteredChunks.Add(chunk);
		}
	}

	private bool IsConfigStandardMob(MobSpawnConfig config)
	{
		return config.mobType != MobType.SWIMMING && config.mobType != MobType.VEHICLE;
	}

	private bool CanBeSpawnedAtCurrentDayTime(MobSpawnConfig config)
	{
		if (config.spawnTime == SpawnTime.BOTH)
		{
			return true;
		}
		bool flag = (!isSurvival) ? SunController.instance.IsNight() : Manager.Get<SurvivalManager>().IsCombatTime();
		return (config.spawnTime == SpawnTime.NIGHT && flag) || (config.spawnTime == SpawnTime.DAY && !flag);
	}

	private MobSpawnConfig PickConfig(DeterministicRNG generator, Func<MobSpawnConfig, bool> check, float sum, MobSpawnConfig defaultToReturn = null)
	{
		float num = generator.NextFloat() * sum;
		float num2 = 0f;
		MobSpawnConfig[] array = spawnConfigs;
		foreach (MobSpawnConfig mobSpawnConfig in array)
		{
			if (check(mobSpawnConfig))
			{
				num2 += mobSpawnConfig.weight;
				if (num < num2)
				{
					return mobSpawnConfig;
				}
			}
		}
		return defaultToReturn;
	}

	private bool IsItWater(ChunkData chunk, Vector3 position)
	{
		if (chunk == null || chunk.NeighborChunks == null)
		{
			return false;
		}
		Index index = chunk.PositionToVoxelIndex(position);
		ushort voxel = chunk.GetVoxel(index.x, index.y + 1, index.z);
		ushort voxel2 = chunk.GetVoxel(index.x, index.y - 1, index.z);
		ushort voxel3 = chunk.GetVoxel(index.x, index.y - 3, index.z);
		ushort voxel4 = chunk.GetVoxel(index.x, index.y - 5, index.z);
		return IsWaterVoxel(voxel) || IsWaterVoxel(voxel2) || IsWaterVoxel(voxel3) || IsWaterVoxel(voxel4);
	}

	private bool IsOnRoad(ChunkData chunk, Vector3 position)
	{
		if (chunk == null || chunk.NeighborChunks == null)
		{
			return false;
		}
		Index index = chunk.PositionToVoxelIndex(position);
		ushort voxel = chunk.GetVoxel(index.x, index.y - 1, index.z);
		ushort voxel2 = chunk.GetVoxel(index.x - 1, index.y - 1, index.z);
		ushort voxel3 = chunk.GetVoxel(index.x + 1, index.y - 1, index.z);
		ushort voxel4 = chunk.GetVoxel(index.x, index.y - 1, index.z - 1);
		ushort voxel5 = chunk.GetVoxel(index.x, index.y - 1, index.z + 1);
		return IsRoadVoxel(voxel) || IsRoadVoxel(voxel2) || IsRoadVoxel(voxel3) || IsRoadVoxel(voxel4) || IsRoadVoxel(voxel5);
	}

	private bool IsWaterVoxel(ushort id)
	{
		return id == Engine.usefulIDs.waterBlockID;
	}

	private bool IsRoadVoxel(ushort id)
	{
		return id == Engine.usefulIDs.roadBlockID || id == Engine.usefulIDs.roadLineBlockID;
	}

	public void PlanSpawn(MobSpawnConfig config, Vector3 pos, int id = -1, PetManager.PetIndividualData pet = null, float rand = 0f, float yRotation = -1f)
	{
		plannedSpawns.Add(new PlannedSpawn
		{
			config = config,
			pos = pos,
			spawnTime = Time.time + 1f,
			petId = id,
			pet = pet,
			randomNumber = rand,
			yRotation = yRotation
		});
	}

	private void Update()
	{
		int num = 0;
		while (num < plannedSpawns.Count)
		{
			if (Time.time > plannedSpawns[num].spawnTime)
			{
				Spawn(plannedSpawns[num].config, plannedSpawns[num].pos, plannedSpawns[num].petId, plannedSpawns[num].pet, plannedSpawns[num].randomNumber, plannedSpawns[num].yRotation);
				plannedSpawns.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public Sprite GetMobSprite(int mobIndex)
	{
		if (mobIndex < 0 || spawnConfigs.Length <= mobIndex)
		{
			return null;
		}
		return spawnConfigs[mobIndex].prefab.GetComponent<Mob>().mobSprite;
	}

	public Sprite GetMobSpriteFromContainer(string name)
	{
		if (mobsContainer == null)
		{
			return null;
		}
		return mobsContainer.GetMobImage(name);
	}

	private void Spawn(MobSpawnConfig config, Vector3 worldPos, int id = -1, PetManager.PetIndividualData pet = null, float rand = 0f, float randomYRotation = -1f)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(config.prefab.gameObject);
		SaveTransform component = gameObject.GetComponent<SaveTransform>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		gameObject.transform.position = worldPos + Vector3.up;
		gameObject.transform.Rotate(Vector3.up, (randomYRotation != -1f) ? randomYRotation : ((float)UnityEngine.Random.Range(0, 360)), Space.Self);
		gameObject.transform.SetParent(base.transform, worldPositionStays: true);
		if (!Mathf.Approximately(0f, config.desiredYRotation))
		{
			gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.WithY(config.desiredYRotation));
		}
		HumanMob componentInChildren = gameObject.GetComponentInChildren<HumanMob>();
		if (componentInChildren == null)
		{
			gameObject.transform.localScale *= scaleEveryMobBy;
		}
		Mob component2 = gameObject.GetComponent<Mob>();
		if (component2 != null)
		{
			gameObject.GetComponent<Mob>().name = config.prefab.gameObject.name;
			ConfigSpawnedMob(gameObject, config, id, pet, rand);
			CheckForQuest(gameObject);
			spawnedMobs.Add(gameObject);
			gameObject.GetComponent<Mob>().petData = pet;
		}
		Pettable component3 = gameObject.GetComponent<Pettable>();
		if (!object.ReferenceEquals(component3, null))
		{
			component3.TryBePatient();
		}
		SaveTransform component4 = gameObject.GetComponent<SaveTransform>();
		if (component4 != null)
		{
			component4.enabled = false;
		}
		for (int i = 0; i < mobConfigs.Length; i++)
		{
			mobConfigs[i].Config(gameObject);
		}
	}

	private void CheckForQuest(GameObject spawnedmob)
	{
		Manager.Get<QuestManager>().HandleMobIndicator(spawnedmob);
	}

	private void ConfigSpawnedMob(GameObject spawned, MobSpawnConfig config, int id, PetManager.PetIndividualData pet, float rand)
	{
		AnimalMob componentInChildren = spawned.GetComponentInChildren<AnimalMob>();
		if (componentInChildren != null)
		{
			ConfigAnimal(componentInChildren, config, id, pet);
		}
		HumanMob componentInChildren2 = spawned.GetComponentInChildren<HumanMob>();
		if (componentInChildren2 != null)
		{
			ConfigHuman(componentInChildren2, pet, rand);
		}
		CarMob componentInChildren3 = spawned.GetComponentInChildren<CarMob>();
		if (componentInChildren3 != null)
		{
			ConfigVehicle(componentInChildren3);
		}
	}

	private void ConfigAnimal(AnimalMob animalMob, MobSpawnConfig config, int id, PetManager.PetIndividualData pet)
	{
		animalMob.runSpeed *= scaleEveryMobBy;
		animalMob.wanderDistanceFrom *= scaleEveryMobBy;
		animalMob.wanderDistanceTo *= scaleEveryMobBy;
		animalMob.wanderSpeed *= scaleEveryMobBy;
		Pettable component = animalMob.GetComponent<Pettable>();
		if (component != null)
		{
			component.prefabName = config.prefab.name;
			if (pet != null)
			{
				component.LoadTamedPetData(id, pet);
			}
		}
	}

	private void ConfigHuman(HumanMob humanMob, PetManager.PetIndividualData pet, float rand)
	{
		humanMob.Init(new HumanMob.HumanParameters(rand));
		if (pet != null && humanMob.GetComponentInChildren<PlayerGraphic>() != null && humanMob.hasToSetGraphic)
		{
			humanMob.SetSkin(pet.outfitIndex);
		}
		Pettable component = humanMob.GetComponent<Pettable>();
		if (component != null)
		{
			component.TrySetVampireSkin();
		}
	}

	private void ConfigVehicle(CarMob carMob)
	{
		SaveTransform component = carMob.GetComponent<SaveTransform>();
		if ((bool)component)
		{
			component.enabled = false;
		}
	}

	public List<GameObject> GetMobList()
	{
		return mobsContainer.mobList;
	}

	public void OnGameplayStarted()
	{
		AddSpawnersToBlocks();
	}

	public void OnGameplayRestarted()
	{
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}
}
