// DecompilerFi decompiler from Assembly-CSharp.dll class: ChestInteractiveObject
using Common.Managers;
using Gameplay;
using States;
using System.Collections;
using Uniblocks;
using UnityEngine;

public class ChestInteractiveObject : InteractiveObject
{
	public static SerializableSet<string> openedChest;

	private const string OPENED_CHESTS = "opened.chests";

	public GameObject coinToSpawn;

	public GameObject starIndicator;

	private Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		if (openedChest == null)
		{
			LoadOpenedChestLocation();
		}
	}

	public override void Init()
	{
		base.Init();
		if (voxelInfo != null)
		{
			if (!CanPutChestThere(voxelInfo.GetGlobalIndex()))
			{
				animator.SetTrigger("InstantOpen");
				isUsable = false;
			}
			else if (Singleton<BlocksController>.get.enableRarityBlocksNoAds)
			{
				starIndicator.SetActive(value: true);
			}
		}
	}

	public override void OnUse()
	{
		if (!CanPutChestThere(voxelInfo.GetGlobalIndex()))
		{
			return;
		}
		animator.SetTrigger("Open");
		openedChest.GetSet().Add(voxelInfo.GetGlobalIndex());
		if (Manager.Contains<AbstractAchievementManager>())
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(ClaimRewardsState)) && Singleton<BlocksController>.get.enableRarityBlocksNoAds)
			{
				Manager.Get<StateMachineManager>().GetStateInstance<ClaimRewardsState>().OnFinish = delegate
				{
					Manager.Get<AbstractAchievementManager>().RegisterEvent("chests.found");
				};
				starIndicator.SetActive(value: false);
			}
			else
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("chests.found");
			}
		}
		if (Singleton<BlocksController>.get.enableRarityBlocksNoAds)
		{
			LootChest chest = Manager.Get<LootChestManager>().GenerateChest(LootChestManager.Rarity.COMMON);
			Manager.Get<LootChestManager>().OpenChest(chest);
			animator.SetTrigger("CollectGold");
			AddOpening();
		}
		else
		{
			StartCoroutine(StartSpawining());
		}
	}

	public override void SetRotation(byte rotation)
	{
		if (Singleton<BlocksController>.get.enableRarityBlocksNoAds)
		{
			base.SetRotation(rotation);
		}
		else
		{
			base.transform.eulerAngles = Vector3.zero;
		}
	}

	private IEnumerator StartSpawining()
	{
		yield return new WaitForSeconds(0.3f);
		int currencyValue = Manager.Get<ModelManager>().chestSettings.GetChestCoins();
		Vector3 pos = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
		if (Manager.Get<ModelManager>().chestSettings.GetChestSpawnsResources() == 1f || !Manager.Contains<AbstractSoftCurrencyManager>())
		{
			SpawnLoot(pos, currencyValue);
		}
		else
		{
			Spawn(currencyValue, pos);
		}
		animator.SetTrigger("CollectGold");
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.FindTreasureXTimes);
		AddOpening();
	}

	public void Spawn(int value, Vector3 lootPosition)
	{
		int num = Random.Range(3, 6);
		int value2 = value / num;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(coinToSpawn);
			gameObject.GetComponent<CoinObject>().Init(value2, lootPosition, addRandomForce: true);
		}
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.FindTreasureXTimes);
		AddOpening();
	}

	private void SpawnLoot(Vector3 lootPosition, int currencyValue)
	{
		CraftingManager craftingManager = Manager.Get<CraftingManager>();
		if ((bool)craftingManager)
		{
			for (int i = 0; i < currencyValue; i++)
			{
				craftingManager.SpawnRandomResource(lootPosition, spawnWithRandomForce: true);
			}
		}
	}

	public static bool CanPutChestThere(string globalVoxelId)
	{
		if (openedChest == null)
		{
			LoadOpenedChestLocation();
		}
		return !openedChest.GetSet().Contains(globalVoxelId);
	}

	private void AddOpening()
	{
		if (openedChest == null)
		{
			LoadOpenedChestLocation();
		}
		openedChest.GetSet().Add(voxelInfo.GetGlobalIndex());
		SaveOpenedChest();
		UnityEngine.Object.Destroy(this);
	}

	private static void SaveOpenedChest()
	{
		PlayerPrefs.SetString("opened.chests", JsonUtility.ToJson(openedChest));
	}

	private static string GetDirPath()
	{
		return Application.persistentDataPath + "/" + Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId;
	}

	private static string GetFilePath()
	{
		return GetDirPath() + "/openedChests";
	}

	public static void LoadOpenedChestLocation()
	{
		if (openedChest == null)
		{
			string @string = PlayerPrefs.GetString("opened.chests", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				openedChest = new SerializableSet<string>();
			}
			else
			{
				openedChest = JsonUtility.FromJson<SerializableSet<string>>(@string);
			}
		}
	}
}
