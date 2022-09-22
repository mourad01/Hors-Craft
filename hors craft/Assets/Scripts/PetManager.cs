// DecompilerFi decompiler from Assembly-CSharp.dll class: PetManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class PetManager : Manager, IGameCallbacksListener
{
	public class PetsData
	{
		public int lastId;

		public Dictionary<string, Dictionary<string, PetIndividualData>> pets;
	}

	public class PetIndividualData
	{
		public string prefabName;

		public Vector3 position;

		public float tame;

		public bool isFollowing;

		public int outfitIndex;

		public int chatbotSeed;

		public double lastEggTime;

		public List<RememberedChatMessage> chatbotHistory;

		public PetIndividualData(Pettable pet)
		{
			prefabName = pet.prefabName;
			position = pet.transform.position;
			tame = pet.tameValue;
			isFollowing = pet.following;
			outfitIndex = ((!(pet is PettableHuman)) ? (-1) : (pet as PettableHuman).outfitIndex);
			chatbotSeed = pet.chatbotSeed;
			chatbotHistory = pet.chatbotHistory;
			lastEggTime = pet.lastEggTimeStamp;
		}

		public PetIndividualData()
		{
		}
	}

	public bool resourcesEnabled = true;

	public bool talkWithMobs;

	public GameObject petsListPrefab;

	public GameObject digIndicator;

	[HideInInspector]
	public PetsList petsList;

	[HideInInspector]
	public PettableFriend currentPet;

	[HideInInspector]
	public VoxelInfo resourceVoxelInfo;

	private const string PETS_KEY = "pets";

	private HashSet<int> spawnedIds = new HashSet<int>();

	private PetsData petsData;

	[SerializeField]
	protected bool useWhiteListing;

	[SerializeField]
	protected List<IPettable> whitelistedPets = new List<IPettable>();

	public bool UseWhiteListing => useWhiteListing;

	public void AddToAllowedPets(IPettable pet)
	{
		if (!whitelistedPets.Contains(pet))
		{
			whitelistedPets.Add(pet);
		}
	}

	public bool CanTamePet(IPettable pet)
	{
		if (!UseWhiteListing)
		{
			return true;
		}
		return whitelistedPets.Contains(pet);
	}

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		petsData = new PetsData
		{
			lastId = 0,
			pets = new Dictionary<string, Dictionary<string, PetIndividualData>>()
		};
		petsList = Object.Instantiate(petsListPrefab).GetComponent<PetsList>();
	}

	public void RegisterPet(Pettable pet)
	{
		if (!pet.isSpecialPet)
		{
			Index key = Engine.PositionToIndex(pet.transform.position);
			GameObject gameObject = Engine.PositionToChunk(pet.transform.position);
			ChunkManager.Chunks.TryGetValue(key, out Chunk value);
			string key2 = key.GetHashCode().ToString();
			if (!petsData.pets.ContainsKey(key2))
			{
				petsData.pets.Add(key2, new Dictionary<string, PetIndividualData>());
			}
			petsData.pets[key2].Add((++petsData.lastId).ToString(), new PetIndividualData(pet));
			pet.chunk = value;
			pet.id = petsData.lastId;
			spawnedIds.Add(pet.id);
		}
	}

	public void UnregisterPet(Pettable pet)
	{
		if (!pet.isSpecialPet && pet.chunk != null && petsData.pets.ContainsKey(pet.chunk.chunkData.ChunkIndex.GetHashCode().ToString()))
		{
			petsData.pets[pet.chunk.chunkData.ChunkIndex.GetHashCode().ToString()].Remove(pet.id.ToString());
			spawnedIds.Remove(pet.id);
		}
	}

	public void PetDespawned(Pettable pet)
	{
		spawnedIds.Remove(pet.id);
	}

	public void SavePetPosition(Pettable pet)
	{
		if (pet.isSpecialPet)
		{
			return;
		}
		Chunk chunk = Engine.PositionToChunkComponent(pet.transform.position);
		if (chunk == null)
		{
			return;
		}
		string key = chunk.chunkData.ChunkIndex.GetHashCode().ToString();
		Chunk chunk2 = pet.chunk;
		if (chunk2 != null)
		{
			string key2 = chunk2.chunkData.ChunkIndex.GetHashCode().ToString();
			if (chunk != chunk2 && petsData.pets.ContainsKey(key2) && petsData.pets[key2].ContainsKey(pet.id.ToString()))
			{
				petsData.pets[key2].Remove(pet.id.ToString());
			}
		}
		pet.chunk = chunk;
		if (!petsData.pets.ContainsKey(key))
		{
			petsData.pets.Add(key, new Dictionary<string, PetIndividualData>());
		}
		petsData.pets[key][pet.id.ToString()] = new PetIndividualData(pet);
	}

	public void ChunkSpawned(ChunkData chunk)
	{
		if (petsData != null)
		{
			string key = chunk.ChunkIndex.GetHashCode().ToString();
			if (petsData.pets.ContainsKey(key))
			{
				foreach (string key2 in petsData.pets[key].Keys)
				{
					if (!spawnedIds.Contains(int.Parse(key2)))
					{
						PlanSpawn(int.Parse(key2), petsData.pets[key][key2]);
					}
				}
			}
		}
	}

	private void PlanSpawn(int id, PetIndividualData pet)
	{
		MobsManager.MobSpawnConfig[] spawnConfigs = Manager.Get<MobsManager>().spawnConfigs;
		int i;
		for (i = 0; i < spawnConfigs.Length && spawnConfigs[i].prefab.name != pet.prefabName; i++)
		{
		}
		if (i < spawnConfigs.Length)
		{
			spawnedIds.Add(id);
			Manager.Get<MobsManager>().PlanSpawn(spawnConfigs[i], pet.position, id, pet);
			return;
		}
		for (int j = 0; j < petsList.petsList.Length; j++)
		{
			if (petsList.petsList[j].prefab.name == pet.prefabName)
			{
				spawnedIds.Add(id);
				Manager.Get<MobsManager>().PlanSpawn(new MobsManager.MobSpawnConfig
				{
					prefab = petsList.petsList[j].prefab,
					spawnCountFrom = 1,
					spawnCountTo = 1,
					weight = 1f
				}, pet.position, id, pet);
			}
		}
	}

	public void TryToUnlockTamedPet(Pettable pet)
	{
		for (int i = 0; i < petsList.petsList.Length; i++)
		{
			if (petsList.petsList[i].prefab.name == pet.prefabName && !petsList.petsList[i].unlocked)
			{
				GameObject prefab = petsList.petsList[i].prefab;
				Sprite mobSprite = prefab.GetComponent<AnimalMob>().mobSprite;
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.PET_UNLOCKED, new PetUnlockedContext
				{
					message = "Unlocked new pet: " + prefab.name + "!",
					icon = mobSprite
				});
				petsList.petsList[i].unlocked = true;
			}
		}
	}

	public void UnlockAll()
	{
		for (int i = 0; i < petsList.petsList.Length; i++)
		{
			petsList.petsList[i].unlocked = true;
		}
	}

	public void OnGameplayRestarted()
	{
		petsData = new PetsData
		{
			lastId = 0,
			pets = new Dictionary<string, Dictionary<string, PetIndividualData>>()
		};
	}

	public void OnGameplayStarted()
	{
		if (WorldPlayerPrefs.get.HasKey("pets"))
		{
			string @string = WorldPlayerPrefs.get.GetString("pets");
			petsData = JSONHelper.Deserialize<PetsData>(@string);
			if (petsData == null || petsData.pets == null)
			{
				petsData = new PetsData
				{
					lastId = 0,
					pets = new Dictionary<string, Dictionary<string, PetIndividualData>>()
				};
			}
		}
		else
		{
			petsData = new PetsData
			{
				lastId = 0,
				pets = new Dictionary<string, Dictionary<string, PetIndividualData>>()
			};
		}
	}

	public void OnGameSavedFrequent()
	{
		WorldPlayerPrefs.get.SetString("pets", JSONHelper.ToJSON(petsData));
	}

	public void OnGameSavedInfrequent()
	{
		WorldPlayerPrefs.get.SetString("pets", JSONHelper.ToJSON(petsData));
	}
}
