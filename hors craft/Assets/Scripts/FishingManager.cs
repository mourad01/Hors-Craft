// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingManager
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay;
using Gameplay.Audio;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;

public class FishingManager : Manager
{
	public enum FishingStatus
	{
		IDLE,
		WAIT,
		FIGHT,
		SUCCESS
	}

	public enum FishingMiniGameDifficulty
	{
		EASY,
		MEDIUM,
		HARD,
		VERYHARD
	}

	public enum FishingRodQuality
	{
		Normal = 1,
		Silver,
		Golden
	}

	[Serializable]
	public struct FishesRarity
	{
		public Sprite symbolIcon;

		public string symbolName;
	}

	[Serializable]
	public struct FishesBatch
	{
		public List<int> fishesId;
	}

	private string leaderboardName = "leaderboardCollectionPoints";

	private int tutorialFinished;

	private int tutorialPopup;

	public bool tutorialPause;

	[Header("Upgrades")]
	public bool lineUpgrade;

	private float lineUpgradeValue;

	public bool floatUpgrade;

	private float floatUpgradeValue;

	public bool reelUpgrade;

	public float reelUpgradeValue;

	public bool hookUpgrade;

	public float hookUpgradeValue;

	public bool baitsUpgrade;

	private float baitsUpgradeValue;

	[Space]
	[Reorderable]
	public Fish[] fishesConfig;

	public FishesBatch[] fishesBatch;

	public FishesRarity[] fishesRarity;

	public Rod[] rodsConfig;

	public Fish fishConfigOnHook;

	public Rod rodConfigCurrent;

	public FishingStateConnector fishingConnector;

	public PlayerFishingController playerFishingController;

	public FishingStatus currentFishingStatus;

	public float waitingTimeLeft;

	[Range(0f, 100f)]
	public float catchingFishProgress;

	public bool fishFighting;

	public bool fishOnHook;

	public bool fishCatched;

	public bool fishBreaks;

	public bool canFish;

	public bool debugMode;

	public int[] fishCount = new int[5];

	public int[] fishUnlockedCount = new int[5];

	public float distanceWanted;

	protected float distanceToOtherSide = 1f;

	protected GameObject spawnedFish;

	protected Animator spawnedFishAnimator;

	protected Transform rodLineStartTransform;

	protected Transform hookTransform;

	protected Transform fishCatchedTransform;

	public int randomFish;

	protected bool waitingForFishToCatch;

	protected static Vector3 fishCatchedRotation = new Vector3(-90f, 0f, -75f);

	protected static Vector3 fishFightingRotation = new Vector3(-180f, 0f, -180f);

	protected Vector3 rodLineStartOffset = new Vector3(0f, -1.5f, 0f);

	protected Vector3 fishSpawnPosition;

	private QuestManager questManager;

	protected ModelManager modelManager;

	private LakeNavigator lakeNavigator;

	public bool isFishingEnabled;

	private bool isFishingInitialized;

	private int batch;

	private const int BRONZE_SCALE = 11;

	private const int SILVER_SCALE = 12;

	private const int GOLDEN_SCALE = 13;

	private const int WOOD = 8;

	private const int DIAMOND = 5;

	private List<float> chanceList = new List<float>();

	public virtual bool isTutorialFinished
	{
		get
		{
			return tutorialFinished > 0;
		}
		set
		{
			if (value)
			{
				tutorialFinished = 1;
			}
		}
	}

	public bool isTutorialPopupFinished
	{
		get
		{
			return tutorialPopup > 0;
		}
		set
		{
			if (value)
			{
				tutorialPopup = 1;
			}
		}
	}

	public PlayerFishingController GetPlayerFishingController
	{
		get
		{
			if (playerFishingController == null)
			{
				playerFishingController = CameraController.instance.Anchor.GetComponentInChildren<PlayerFishingController>();
			}
			return playerFishingController;
		}
	}

	public override void Init()
	{
		modelManager = Manager.Get<ModelManager>();
		isFishingEnabled = false;
	}

	public void ModelDownloaded()
	{
		isFishingEnabled = modelManager.fishingSettings.IsFishingEnabled();
		if (isFishingEnabled)
		{
			StartCoroutine(DoActionAfterTime(0.1f, InitFishing));
		}
	}

	private IEnumerator DoActionAfterTime(float seconds, Action action)
	{
		yield return new WaitForSeconds(seconds);
		action?.Invoke();
	}

	public virtual void InitFishing()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.FISHING_ENABLED);
		SetBaseSettingsFromModel();
		LoadCollection();
		LoadRod();
		LoadTutorialStatus();
		LoadUpgrades();
		Fish[] array = fishesConfig;
		for (int i = 0; i < array.Length; i++)
		{
			Fish fish = array[i];
			fishCount[fish.rarity - 1]++;
			if (fish.catched)
			{
				fishUnlockedCount[fish.rarity - 1]++;
			}
		}
		questManager = Manager.Get<QuestManager>();
		if (isTutorialFinished)
		{
			tutorialPause = false;
		}
		isFishingInitialized = true;
	}

	protected virtual void Update()
	{
		if (!isFishingEnabled)
		{
			return;
		}
		if (playerFishingController == null)
		{
			try
			{
				playerFishingController = CameraController.instance.Anchor.GetComponentInChildren<PlayerFishingController>();
				rodLineStartTransform = playerFishingController.rodLinePivot;
				hookTransform = playerFishingController.fishHookPivot;
				fishCatchedTransform = playerFishingController.fishCatchedPivot;
				lakeNavigator = playerFishingController.GetComponent<LakeNavigator>();
				LoadRod();
			}
			catch
			{
			}
		}
		if (tutorialPause)
		{
			return;
		}
		if (currentFishingStatus == FishingStatus.WAIT)
		{
			if (!waitingForFishToCatch)
			{
				waitingTimeLeft = UnityEngine.Random.Range(3f, 4f);
				if (floatUpgrade)
				{
					waitingTimeLeft *= floatUpgradeValue;
				}
				waitingForFishToCatch = true;
			}
			if (waitingTimeLeft > 0f)
			{
				waitingTimeLeft -= Time.deltaTime;
			}
			if (waitingTimeLeft <= 0f)
			{
				waitingTimeLeft = 0f;
				fishOnHook = true;
				catchingFishProgress = 50f;
			}
		}
		if (currentFishingStatus == FishingStatus.FIGHT)
		{
			FishFight();
			if (catchingFishProgress >= 100f)
			{
				catchingFishProgress = 100f;
				FishCatched();
			}
			else if (catchingFishProgress <= 0f)
			{
				if (!isTutorialFinished)
				{
					catchingFishProgress = 0f;
				}
				else
				{
					catchingFishProgress = 0f;
					FishBreaks();
				}
			}
		}
		if (currentFishingStatus == FishingStatus.SUCCESS)
		{
			Vector3 position = fishCatchedTransform.position;
			Vector3 vector = CameraController.instance.MainCamera.ScreenToWorldPoint(Vector3.zero);
			float y = vector.y;
			Vector3 size = hookTransform.GetChild(0).GetComponentInChildren<Renderer>().bounds.size;
			position.y = y + size.y / 2f - 0.2f;
			hookTransform.localRotation = Quaternion.Lerp(hookTransform.localRotation, Quaternion.Euler(fishCatchedRotation), Time.deltaTime * 3f);
			hookTransform.position = Vector3.Lerp(hookTransform.position, position, Time.deltaTime * 3f);
			spawnedFishAnimator.SetTrigger("catched");
		}
	}

	protected virtual void LateUpdate()
	{
		if (isFishingEnabled && playerFishingController != null)
		{
			if (currentFishingStatus == FishingStatus.WAIT || currentFishingStatus == FishingStatus.FIGHT)
			{
				playerFishingController.fishingLineRenderer.SetPosition(0, rodLineStartTransform.position);
				playerFishingController.fishingLineRenderer.SetPosition(1, hookTransform.position);
			}
			else if (currentFishingStatus == FishingStatus.SUCCESS)
			{
				playerFishingController.fishingLineRenderer.SetPosition(0, hookTransform.position);
				playerFishingController.fishingLineRenderer.SetPosition(1, hookTransform.position + Vector3.up);
			}
			else
			{
				playerFishingController.fishingLineRenderer.SetPosition(0, rodLineStartTransform.position);
				playerFishingController.fishingLineRenderer.SetPosition(1, rodLineStartTransform.position + Vector3.down * 2f);
			}
		}
	}

	public void FishingButtonClicked()
	{
		switch (currentFishingStatus)
		{
		case FishingStatus.IDLE:
			ThrowRod();
			break;
		case FishingStatus.WAIT:
			if (fishOnHook)
			{
				StartFighting();
			}
			break;
		case FishingStatus.SUCCESS:
			EndShowing();
			break;
		}
	}

	public int GetCollectionPoints()
	{
		int num = 0;
		Fish[] array = fishesConfig;
		for (int i = 0; i < array.Length; i++)
		{
			Fish fish = array[i];
			num += fish.catchedPoints;
		}
		return num;
	}

	protected virtual void ThrowRod()
	{
		fishBreaks = false;
		waitingForFishToCatch = false;
		playerFishingController.ThrowRod();
		currentFishingStatus = FishingStatus.WAIT;
		fishSpawnPosition = new Vector3(0f, 0f, distanceToOtherSide);
		LoadRod();
		if (hookTransform == null)
		{
			hookTransform = playerFishingController.fishHookPivot;
		}
		if (rodLineStartTransform == null)
		{
			rodLineStartTransform = playerFishingController.rodLinePivot;
		}
		hookTransform.localPosition = fishSpawnPosition / 2f + rodLineStartTransform.localPosition + rodLineStartOffset;
		Vector3 position = CameraController.instance.MainCamera.transform.position;
		position.y -= 2f;
		hookTransform.LookAt(position);
	}

	protected virtual void StartFighting()
	{
		batch = lakeNavigator.GetCurrentLakeBatch();
		if (!debugMode)
		{
			if (!fishesConfig[7].catched)
			{
				randomFish = 7;
			}
			else
			{
				randomFish = GetRandomFishId(batch);
			}
		}
		fishConfigOnHook = fishesConfig[randomFish];
		int num = UnityEngine.Random.Range(fishConfigOnHook.weightMin, fishConfigOnHook.weightMax);
		if (lineUpgrade)
		{
			num = Mathf.FloorToInt((float)num * lineUpgradeValue);
		}
		fishConfigOnHook.catchedWeight = num;
		int catchedPoints = num * fishConfigOnHook.rarity / 100;
		fishConfigOnHook.catchedPoints = catchedPoints;
		fishingConnector.fishCatchedStats.gameObject.SetActive(value: true);
		fishingConnector.fishCatchedStats.SetUp(fishConfigOnHook);
		SpawnFish();
		playerFishingController.FishFight();
		currentFishingStatus = FishingStatus.FIGHT;
		fishFighting = true;
	}

	protected void SpawnFish()
	{
		spawnedFish = UnityEngine.Object.Instantiate(fishConfigOnHook.prefab, hookTransform);
		spawnedFish.transform.localPosition = Vector3.zero;
		spawnedFishAnimator = spawnedFish.GetComponent<Animator>();
		spawnedFishAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
	}

	protected void FishFight()
	{
		if (spawnedFish != null)
		{
			distanceWanted = (100f - catchingFishProgress) / 100f;
			Vector3 a = rodLineStartTransform.localPosition + rodLineStartOffset;
			hookTransform.localPosition = a + fishSpawnPosition * distanceWanted;
		}
	}

	protected virtual void EndShowing()
	{
		fishingConnector.fishCatchedStats.HideFishStats();
		playerFishingController.EndShowing();
		waitingForFishToCatch = false;
		fishBreaks = false;
		fishCatched = false;
		currentFishingStatus = FishingStatus.WAIT;
		UnityEngine.Object.Destroy(spawnedFish.gameObject);
		hookTransform.localPosition = fishSpawnPosition / 2f + rodLineStartTransform.localPosition + rodLineStartOffset;
		hookTransform.localRotation = Quaternion.Euler(fishFightingRotation);
	}

	protected void FishBreaks()
	{
		fishingConnector.fightFishingButton.gameObject.SetActive(value: false);
		fishOnHook = false;
		fishCatched = false;
		fishFighting = false;
		fishBreaks = true;
		playerFishingController.FishBreak();
		currentFishingStatus = FishingStatus.IDLE;
		UnityEngine.Object.Destroy(spawnedFish.gameObject);
		hookTransform.localPosition = fishSpawnPosition / 2f + rodLineStartTransform.localPosition + rodLineStartOffset;
		hookTransform.localRotation = Quaternion.Euler(fishFightingRotation);
		questManager.FishXTimes();
		Manager.Get<StatsManager>().MinigameFinished(StatsManager.MinigameType.FISH, success: false);
		Manager.Get<StatsManager>().FishingFailure();
	}

	protected virtual void FishCatched()
	{
		fishingConnector.fishCatchedStats.ShowFishStats();
		fishingConnector.fightFishingButton.gameObject.SetActive(value: false);
		StartCoroutine(EnableFishingButton());
		fishOnHook = false;
		fishFighting = false;
		fishBreaks = false;
		fishCatched = true;
		playerFishingController.FishCatched();
		currentFishingStatus = FishingStatus.SUCCESS;
		if (!fishesConfig[randomFish].catched)
		{
			FishingState fishingState = Manager.Get<StateMachineManager>().currentState as FishingState;
			fishingState.ShowFishNotification(fishesConfig[randomFish].name + " Unlocked!", fishesConfig[randomFish].icon);
			fishesConfig[randomFish].catched = true;
			fishUnlockedCount[fishesConfig[randomFish].rarity - 1]++;
		}
		int catchedWeight = fishConfigOnHook.catchedWeight;
		if (catchedWeight > fishesConfig[randomFish].catchedWeight)
		{
			fishesConfig[randomFish].catchedWeight = catchedWeight;
			FishingState fishingState2 = Manager.Get<StateMachineManager>().currentState as FishingState;
			fishingState2.ShowFishNotification(fishesConfig[randomFish].name + " New Record!", fishesConfig[randomFish].icon);
		}
		int catchedPoints = fishConfigOnHook.catchedPoints;
		if (catchedPoints > fishesConfig[randomFish].catchedPoints)
		{
			fishesConfig[randomFish].catchedPoints = catchedPoints;
		}
		UpdateQuestsAndGetResources();
		Manager.Get<StatsManager>().MinigameFinished(StatsManager.MinigameType.FISH, success: true);
		Manager.Get<StatsManager>().FishingSuccess();
		Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(leaderboardName), Manager.Get<FishingManager>().GetCollectionPoints());
	}

	public virtual void EndFishing()
	{
		playerFishingController.DisableFishing();
		waitingForFishToCatch = false;
		fishBreaks = false;
		fishOnHook = false;
		fishFighting = false;
		fishCatched = false;
		if (fishingConnector != null)
		{
			fishingConnector.fightFishingButton.gameObject.SetActive(value: false);
		}
		currentFishingStatus = FishingStatus.IDLE;
		if (spawnedFish != null)
		{
			UnityEngine.Object.Destroy(spawnedFish.gameObject);
		}
	}

	private void UpdateQuestsAndGetResources()
	{
		if (fishConfigOnHook.rarity > 1)
		{
			float num = 0f;
			for (int i = 0; i < fishConfigOnHook.bronzeScalesDrop; i++)
			{
				questManager.OnResourcePickedUp(11);
			}
			GatherResource(11);
			PlaySound(GameSound.RESOURCE_PICKUP);
			num = ((fishConfigOnHook.rarity != 2) ? 1f : UnityEngine.Random.value);
			if (num >= 0.7f)
			{
				for (int j = 0; j < fishConfigOnHook.silverScalesDrop; j++)
				{
					questManager.OnResourcePickedUp(12);
				}
				GatherResource(12);
				PlaySound(GameSound.RESOURCE_PICKUP);
			}
			num = ((fishConfigOnHook.rarity != 3) ? 1f : UnityEngine.Random.value);
			if (num >= 0.7f)
			{
				for (int k = 0; k < fishConfigOnHook.goldenScalesDrop; k++)
				{
					questManager.OnResourcePickedUp(13);
				}
				GatherResource(13);
				PlaySound(GameSound.RESOURCE_PICKUP);
			}
		}
		else
		{
			if (fishConfigOnHook.codeName == "Trash_1")
			{
				for (int l = 0; l < 3; l++)
				{
					questManager.OnResourcePickedUp(5);
				}
				GatherResource(5);
			}
			else if (fishConfigOnHook.codeName == "Trash_2")
			{
				for (int m = 0; m < 3; m++)
				{
					questManager.OnResourcePickedUp(8);
				}
				GatherResource(8);
			}
			PlaySound(GameSound.RESOURCE_PICKUP);
		}
		if (fishConfigOnHook.codeName == "LegendaryFish_1")
		{
			questManager.OnStingrayCatched();
		}
		if (fishConfigOnHook.codeName == "LegendaryFish_2")
		{
			questManager.OnOctopusCatched();
		}
		if (fishConfigOnHook.codeName == "LegendaryFish_3")
		{
			questManager.OnMarlinCatched();
		}
		CheckAchievementsStatus();
		if (fishConfigOnHook.rarity == 3)
		{
			questManager.OnRareCatched();
		}
		else if (fishConfigOnHook.rarity == 4)
		{
			questManager.OnEpicCatched();
		}
		else if (fishConfigOnHook.rarity == 5)
		{
			questManager.OnLegendaryCatched();
		}
		questManager.OnXCollectionPoints(GetCollectionPoints());
		questManager.FishXTimes();
		if (playerFishingController.isInBoat || playerFishingController.isOnBoat)
		{
			if (playerFishingController.currentBoat.boatData.boatType == BoatType.Wood)
			{
				questManager.CatchedFishBeingOnBoat();
			}
			else if (playerFishingController.currentBoat.boatData.boatType == BoatType.Motor)
			{
				questManager.CatchedFishBeingOnMotorboat();
			}
			else if (playerFishingController.currentBoat.boatData.boatType == BoatType.Yacht)
			{
				questManager.CatchedFishBeingOnYachtBoat();
			}
			else if (playerFishingController.currentBoat.boatData.boatType == BoatType.Fishing)
			{
				questManager.CatchedFishBeingOnFishingBoat();
			}
			else if (playerFishingController.currentBoat.boatData.boatType == BoatType.Tourist)
			{
				questManager.CatchedFishBeingOnTouristBoat();
			}
		}
	}

	private void CheckAchievementsStatus()
	{
		int collectionPoints = GetCollectionPoints();
		if (collectionPoints >= 1000)
		{
			AchievementReporter.Report("points1000");
		}
		if (collectionPoints >= 1500)
		{
			AchievementReporter.Report("points1500");
		}
		if (collectionPoints >= 2000)
		{
			AchievementReporter.Report("points2000");
		}
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		for (int i = 0; i < fishesConfig.Length; i++)
		{
			if (!fishesConfig[i].catched)
			{
				flag3 = false;
			}
			if (fishesConfig[i].rarity == 2)
			{
				if (!fishesConfig[i].catched)
				{
					flag = false;
				}
			}
			else if (fishesConfig[i].rarity == 3 && !fishesConfig[i].catched)
			{
				flag2 = false;
			}
		}
		if (flag)
		{
			AchievementReporter.Report("catchedAllRareFishes");
		}
		if (flag2)
		{
			AchievementReporter.Report("catchedAllEpicFishes");
		}
		if (flag3)
		{
			AchievementReporter.Report("catchedAllFishes");
		}
		if (fishConfigOnHook.catchedWeight >= 100 && fishConfigOnHook.catchedWeight <= 130)
		{
			AchievementReporter.Report("catchedLightestFish");
		}
		if (fishConfigOnHook.catchedWeight >= 7900)
		{
			AchievementReporter.Report("catchedHeaviestFish");
		}
		if (fishesConfig[0].catched && fishesConfig[1].catched)
		{
			AchievementReporter.Report("catchedCanBoot");
		}
		if (fishConfigOnHook.rarity == 3)
		{
			AchievementReporter.Report("catchedRareFish");
		}
		else if (fishConfigOnHook.rarity == 4)
		{
			AchievementReporter.Report("catchedEpicFish");
		}
		else if (fishConfigOnHook.rarity == 5)
		{
			AchievementReporter.Report("catchedLegendaryFish");
		}
	}

	private void GatherResource(int resourceId)
	{
		(Manager.Get<StateMachineManager>().currentState as FishingState).OnResourceGathered(resourceId);
		Singleton<PlayerData>.get.playerItems.AddToResources(resourceId, 1);
	}

	private int GetRandomFishId(int index)
	{
		List<int> fishesId = fishesBatch[index].fishesId;
		float num = 300f / (float)fishesBatch.Length;
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		chanceList.Clear();
		for (int i = 0; i < fishesId.Count; i++)
		{
			num3 += fishesConfig[fishesId[i]].precentageToSuccess;
			chanceList.Add(num3);
		}
		chanceList[fishesId.Count - 1] = num;
		if (baitsUpgrade)
		{
			num2 *= baitsUpgradeValue;
		}
		for (int j = 0; j < chanceList.Count; j++)
		{
			if (chanceList[j] > num2)
			{
				return fishesId[j];
			}
		}
		return 0;
	}

	public void SetWaterMaxDistance(float distance)
	{
		distanceToOtherSide = distance;
	}

	public virtual void SetRodConfig(int rodId)
	{
		UnityEngine.Debug.Log("[FISHING MANAGER]: Trying set rod: " + rodId);
		if (rodId < 0 || rodId >= rodsConfig.Length)
		{
			rodId = 0;
		}
		rodConfigCurrent = rodsConfig[rodId];
		SaveRod();
		try
		{
			playerFishingController.SetRodRenderer(rodId);
		}
		catch
		{
			UnityEngine.Debug.Log("[FISHING MANAGER]: Didn't set rod renderers.");
		}
	}

	private void PlaySound(GameSound clip)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(clip);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		Sound sound2 = sound;
		sound2.Play();
	}

	protected IEnumerator EnableFishingButton()
	{
		fishingConnector.actionFishingButton.interactable = false;
		yield return new WaitForSecondsRealtime(1f);
		fishingConnector.actionFishingButton.interactable = true;
	}

	public void SetFishId(int id)
	{
		randomFish = id;
	}

	public Fish GetFishById(int id)
	{
		if (id < 0)
		{
			id = 0;
		}
		else if (id > fishesConfig.Length - 1)
		{
			id = fishesConfig.Length - 1;
		}
		return fishesConfig[id];
	}

	public void CheckUpgradeablesStatus()
	{
		AchievementReporter.Report("unlockedAllUpgrades");
	}

	private void SaveCollection()
	{
		for (int i = 0; i < fishesConfig.Length; i++)
		{
			string str = "fishing.collection." + fishesConfig[i].codeName;
			PlayerPrefs.SetInt(str + ".weight", fishesConfig[i].catchedWeight);
			PlayerPrefs.SetInt(str + ".points", fishesConfig[i].catchedPoints);
		}
	}

	private void SaveRod()
	{
		int quality = (int)rodConfigCurrent.quality;
		UnityEngine.Debug.Log("[FISHING MANAGER]: Save rod: " + (quality - 1));
		if (quality < 1)
		{
			UnityEngine.Debug.Log("[FISHING MANAGER]: Not saved");
			return;
		}
		PlayerPrefs.SetInt("fishing.rod.current", quality);
		PlayerPrefs.Save();
	}

	private void SaveTutorialStatus()
	{
		PlayerPrefs.SetInt("fishing.tutorial.finished", tutorialFinished);
		PlayerPrefs.SetInt("fishing.tutorial.popup.finished", tutorialPopup);
	}

	private void SaveUpgrades()
	{
		if (lineUpgrade)
		{
			PlayerPrefs.SetInt("fishing.upgrade.line", 1);
		}
		else
		{
			PlayerPrefs.SetInt("fishing.upgrade.line", 0);
		}
		if (floatUpgrade)
		{
			PlayerPrefs.SetInt("fishing.upgrade.float", 1);
		}
		else
		{
			PlayerPrefs.SetInt("fishing.upgrade.float", 0);
		}
		if (reelUpgrade)
		{
			PlayerPrefs.SetInt("fishing.upgrade.reel", 1);
		}
		else
		{
			PlayerPrefs.SetInt("fishing.upgrade.reel", 0);
		}
		if (hookUpgrade)
		{
			PlayerPrefs.SetInt("fishing.upgrade.hook", 1);
		}
		else
		{
			PlayerPrefs.SetInt("fishing.upgrade.hook", 0);
		}
		if (baitsUpgrade)
		{
			PlayerPrefs.SetInt("fishing.upgrade.baits", 1);
		}
		else
		{
			PlayerPrefs.SetInt("fishing.upgrade.baits", 0);
		}
	}

	private void LoadTutorialStatus()
	{
		tutorialFinished = PlayerPrefs.GetInt("fishing.tutorial.finished", 0);
		tutorialPopup = PlayerPrefs.GetInt("fishing.tutorial.popup.finished", 0);
	}

	private void LoadRod()
	{
		int num = PlayerPrefs.GetInt("fishing.rod.current", 1) - 1;
		bool flag = Singleton<PlayerData>.get.playerItems.GetCraftableCount(0) > 0;
		bool flag2 = Singleton<PlayerData>.get.playerItems.GetCraftableCount(1) > 0;
		if (flag)
		{
			num = 1;
		}
		if (flag2)
		{
			num = 2;
		}
		UnityEngine.Debug.Log("[FISHING MANAGER]: Load rod: " + num);
		SetRodConfig(num);
	}

	private void LoadUpgrades()
	{
		lineUpgrade = (PlayerPrefs.GetInt("fishing.upgrade.line", 0) == 1);
		floatUpgrade = (PlayerPrefs.GetInt("fishing.upgrade.float", 0) == 1);
		reelUpgrade = (PlayerPrefs.GetInt("fishing.upgrade.reel", 0) == 1);
		hookUpgrade = (PlayerPrefs.GetInt("fishing.upgrade.hook", 0) == 1);
		baitsUpgrade = (PlayerPrefs.GetInt("fishing.upgrade.baits", 0) == 1);
	}

	protected void SetBaseSettingsFromModel()
	{
		for (int i = 0; i < fishesConfig.Length; i++)
		{
			switch (fishesConfig[i].rarity)
			{
			case 1:
				fishesConfig[i].precentageToSuccess = modelManager.fishingSettings.TrashChance();
				break;
			case 2:
				fishesConfig[i].precentageToSuccess = modelManager.fishingSettings.CommonChance();
				fishesConfig[i].bronzeScalesDrop = modelManager.fishingSettings.CommonBronzeScaleDrop();
				fishesConfig[i].silverScalesDrop = modelManager.fishingSettings.CommonSilverScaleDrop();
				break;
			case 3:
				fishesConfig[i].precentageToSuccess = modelManager.fishingSettings.RareChance();
				fishesConfig[i].bronzeScalesDrop = modelManager.fishingSettings.RareBronzeScaleDrop();
				fishesConfig[i].silverScalesDrop = modelManager.fishingSettings.RareSilverScaleDrop();
				fishesConfig[i].goldenScalesDrop = modelManager.fishingSettings.RareSilverScaleDrop();
				break;
			case 4:
				fishesConfig[i].precentageToSuccess = modelManager.fishingSettings.EpicChance();
				fishesConfig[i].bronzeScalesDrop = modelManager.fishingSettings.EpicBronzeScaleDrop();
				fishesConfig[i].silverScalesDrop = modelManager.fishingSettings.EpicSilverScaleDrop();
				fishesConfig[i].goldenScalesDrop = modelManager.fishingSettings.EpicGoldenScaleDrop();
				break;
			case 5:
				fishesConfig[i].precentageToSuccess = modelManager.fishingSettings.LegendaryChance();
				fishesConfig[i].bronzeScalesDrop = modelManager.fishingSettings.LegendaryBronzeScaleDrop();
				fishesConfig[i].silverScalesDrop = modelManager.fishingSettings.LegendarySilverScaleDrop();
				fishesConfig[i].goldenScalesDrop = modelManager.fishingSettings.LegendaryGoldenScaleDrop();
				break;
			}
		}
		for (int j = 0; j < rodsConfig.Length; j++)
		{
			switch (rodsConfig[j].quality)
			{
			case FishingRodQuality.Normal:
				rodsConfig[j].baseWidth = modelManager.fishingSettings.RodWoodenBaseWidth();
				break;
			case FishingRodQuality.Silver:
				rodsConfig[j].baseWidth = modelManager.fishingSettings.RodSilverBaseWidth();
				break;
			case FishingRodQuality.Golden:
				rodsConfig[j].baseWidth = modelManager.fishingSettings.RodGoldenBaseWidth();
				break;
			}
		}
		lineUpgradeValue = modelManager.fishingSettings.LineUpgrade();
		if (lineUpgradeValue == 0f)
		{
			lineUpgradeValue = 1.25f;
		}
		floatUpgradeValue = modelManager.fishingSettings.FloatUpgrade();
		if (floatUpgradeValue == 0f)
		{
			floatUpgradeValue = 0.8f;
		}
		reelUpgradeValue = modelManager.fishingSettings.ReelUpgrade();
		if (reelUpgradeValue == 0f)
		{
			reelUpgradeValue = 0.85f;
		}
		hookUpgradeValue = modelManager.fishingSettings.HookUpgrade();
		if (hookUpgradeValue == 0f)
		{
			hookUpgradeValue = 1.1f;
		}
		baitsUpgradeValue = modelManager.fishingSettings.BaitsUpgrade();
		if (baitsUpgradeValue == 0f)
		{
			baitsUpgradeValue = 1.25f;
		}
	}

	private void LoadCollection()
	{
		for (int i = 0; i < fishesConfig.Length; i++)
		{
			string str = "fishing.collection." + fishesConfig[i].codeName;
			fishesConfig[i].catchedWeight = PlayerPrefs.GetInt(str + ".weight", 0);
			fishesConfig[i].catchedPoints = PlayerPrefs.GetInt(str + ".points", 0);
			if (fishesConfig[i].catchedWeight > 0)
			{
				fishesConfig[i].catched = true;
			}
		}
	}

	private void SaveFishing()
	{
		if (isFishingEnabled && isFishingInitialized)
		{
			SaveCollection();
			SaveRod();
			SaveTutorialStatus();
			SaveUpgrades();
			PlayerPrefs.Save();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		SaveFishing();
	}

	private void OnApplicationQuit()
	{
		SaveFishing();
	}
}
