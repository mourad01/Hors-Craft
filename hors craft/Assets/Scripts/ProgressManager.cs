// DecompilerFi decompiler from Assembly-CSharp.dll class: ProgressManager
using Common.Managers;
using Gameplay;
using States;
using System;
using UnityEngine;

public class ProgressManager : Manager
{
	[Space]
	public TopNotification levelUpNotificationPrefab;

	public string categoryToOpenOnNotificationClick;

	public string leaderboardName = "leaderboardGlamour";

	[HideInInspector]
	public Action onLevelUpCallbacks;

	private TopNotification _levelUpInstance;

	private float experienceMultiplier = 1.15f;

	private int experienceBase = 15;

	private bool useAdditive;

	private int experienceAdditive;

	private bool isDebugEnabled = true;

	public bool showRewards;

	private TopNotification levelUpInstance
	{
		get
		{
			if (_levelUpInstance == null && levelUpNotificationPrefab != null)
			{
				_levelUpInstance = UnityEngine.Object.Instantiate(levelUpNotificationPrefab.gameObject).GetComponentInChildren<TopNotification>();
				_levelUpInstance.GetComponentInChildren<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
				LevelUpNotification levelUpNotification = _levelUpInstance as LevelUpNotification;
				if (levelUpNotification != null)
				{
					levelUpNotification.categoryToOpen = categoryToOpenOnNotificationClick;
				}
			}
			return _levelUpInstance;
		}
	}

	public int level
	{
		get;
		private set;
	}

	public int experienceNeededToNextLevel
	{
		get;
		private set;
	}

	public int experience
	{
		get;
		private set;
	}

	public static event Action<int, int> onExpIncrease;

	public override void Init()
	{
		onLevelUpCallbacks = (Action)Delegate.Combine(onLevelUpCallbacks, new Action(SaveToLeaderboards));
		Load();
	}

	public void InitVariables()
	{
		ModelManager modelManager = Manager.Get<ModelManager>();
		experienceMultiplier = modelManager.progressSettings.GetExperienceMultiplier();
		useAdditive = modelManager.progressSettings.GetUseAdditive();
		experienceAdditive = modelManager.progressSettings.GetExperienceAdditive();
		experienceBase = modelManager.progressSettings.GetExperienceBase();
		UpdateNeededExperience();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
		{
			IncreaseExperience(2);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
		{
			IncreaseExperience(200);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
		{
			isDebugEnabled = !isDebugEnabled;
		}
	}

	public void IncreaseLevel()
	{
		level++;
		experience -= experienceNeededToNextLevel;
		UpdateNeededExperience();
		CheckForLevelUp();
		Manager.Get<QuestManager>().GetXLevel();
		HideLevelUpNotificationImmediately();
		Manager.Get<StatsManager>().LevelUp(level);
		Invoke("ShowLevelUpNotification", 0.2f);
		if (Manager.Contains<AbstractAchievementManager>())
		{
			Manager.Get<AbstractAchievementManager>().RegisterEvent("progress");
		}
		if (onLevelUpCallbacks != null)
		{
			onLevelUpCallbacks();
		}
		MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.LEVEL_UP);
	}

	public void IncreaseExperience(int amount)
	{
		experience += amount;
		if (ProgressManager.onExpIncrease != null)
		{
			ProgressManager.onExpIncrease(amount, experience);
		}
		CheckForLevelUp();
	}

	private void CheckForLevelUp()
	{
		if (experience >= experienceNeededToNextLevel)
		{
			IncreaseLevel();
		}
	}

	private void UpdateNeededExperience()
	{
		if (Manager.Get<ModelManager>().progressSettings.TryGetExperienceNeededForLevel(level, out int expNeeded))
		{
			experienceNeededToNextLevel = expNeeded;
		}
		else if (useAdditive)
		{
			experienceNeededToNextLevel = experienceBase + experienceAdditive * (level - 1);
		}
		else
		{
			experienceNeededToNextLevel = Mathf.FloorToInt((float)experienceBase * Mathf.Pow(experienceMultiplier, level - 1));
		}
	}

	public void ResetProgress()
	{
		level = 1;
		experience = 0;
		Save();
	}

	public void HideLevelUpNotificationImmediately()
	{
		if (levelUpInstance != null)
		{
			levelUpInstance.HideImmediately();
		}
	}

	private void ShowLevelUpNotification()
	{
		if (showRewards && Manager.Get<StateMachineManager>().ContainsState(typeof(LevelUpWithRewardsState)))
		{
			Manager.Get<StateMachineManager>().PushState<LevelUpWithRewardsState>();
		}
		else if (!(levelUpInstance == null))
		{
			TopNotification.ShowInformation showInformation = new TopNotification.ShowInformation();
			showInformation.setOnTop = true;
			showInformation.timeToHide = 1.5f;
			TopNotification.ShowInformation information = showInformation;
			levelUpInstance.Show(information);
			levelUpInstance.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
		}
	}

	private void SaveToLeaderboards()
	{
		Manager.Get<SocialPlatformManager>()?.social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(leaderboardName), Manager.Get<ProgressManager>().level);
	}

	private void Save()
	{
		PlayerPrefs.SetInt("player.level", level);
		PlayerPrefs.SetInt("player.experience", experience);
	}

	protected virtual void Load()
	{
		level = PlayerPrefs.GetInt("player.level", 1);
		experience = PlayerPrefs.GetInt("player.experience", 0);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Save();
		}
	}

	private void OnApplicationQuit()
	{
		Save();
	}
}
