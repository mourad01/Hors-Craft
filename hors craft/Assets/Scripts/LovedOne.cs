// DecompilerFi decompiler from Assembly-CSharp.dll class: LovedOne
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LovedOne : MonoBehaviour
{
	[Serializable]
	public class RelationshipInfo
	{
		public int relationshipStage;

		public float loveValue;

		public int favouriteGiftIndex;

		public string relationshipStartDate;

		public List<float> minutesInRelationshipPerStage = new List<float>();
	}

	private const float MAX_DISTANCE_FROM_PLAYER = 32f;

	private LoveManager loveManager;

	private LoveContext loveContext;

	private SpriteContainerContext spriteContainer;

	private float loveDeclineBlocked;

	public RelationshipInfo info
	{
		get;
		private set;
	}

	public float minutesSpentInRelationship => (float)(DateTime.Now - DateTimeFromString(info.relationshipStartDate)).TotalMinutes;

	public int daysSpentInRelationship => (int)(DateTime.Now - DateTimeFromString(info.relationshipStartDate)).TotalDays;

	public LoveManager.RelationshipStage relationshipStage => loveManager.relationshipStages[info.relationshipStage];

	public float maxLoveAtThisStage => Manager.Get<ModelManager>().loveSettings.GetLoveRequired(info.relationshipStage);

	private float lovePerTap => Manager.Get<ModelManager>().loveSettings.GetLovePerTap(info.relationshipStage);

	private void Awake()
	{
		loveManager = Manager.Get<LoveManager>();
		base.gameObject.AddComponentWithInit(delegate(SaveTransform f)
		{
			f.module = loveManager.loveModule;
			f.PrefabName = base.gameObject.name.Replace("(Clone)", string.Empty);
		});
	}

	private void Start()
	{
		AnimalMob componentInChildren = GetComponentInChildren<AnimalMob>();
		if (componentInChildren != null)
		{
			componentInChildren.logic = AnimalMob.AnimalLogic.FOLLOW_PLAYER;
			componentInChildren.ReconstructBehaviourTree();
		}
		HumanMobNavigator componentInChildren2 = GetComponentInChildren<HumanMobNavigator>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.enableAnimations = false;
		}
		AddLoveFact();
	}

	public void ConfigNewLover()
	{
		List<int> array = (from g in loveManager.gifts
			select loveManager.gifts.IndexOf(g)).ToList();
		if (GetComponentInParent<HumanMob>() != null)
		{
			Skin.Gender gender = GetComponentInParent<HumanMob>().currentGender;
			array = (from g in loveManager.gifts
				where g.gender == gender
				select loveManager.gifts.IndexOf(g)).ToList();
		}
		info = new RelationshipInfo
		{
			favouriteGiftIndex = array.Random(),
			relationshipStage = 0,
			loveValue = 1f,
			relationshipStartDate = DateTimeToString(DateTime.Now)
		};
		Manager.Get<StatsManager>().RelationshipLevelUp(info.relationshipStage);
		info.loveValue = 0.2f * maxLoveAtThisStage;
		PettableHuman componentInParent = GetComponentInParent<PettableHuman>();
		if (componentInParent != null)
		{
			componentInParent.GrabFlower();
		}
	}

	public void LoadData(RelationshipInfo info)
	{
		loveManager = Manager.Get<LoveManager>();
		this.info = info;
	}

	private void AddLoveFact()
	{
		loveContext = new LoveContext
		{
			loveValue = info.loveValue,
			maxLoveValue = maxLoveAtThisStage
		};
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_LOVE, loveContext);
		spriteContainer = new SpriteContainerContext
		{
			sprite = loveManager.relationshipStages[info.relationshipStage].tamePanelSprite
		};
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_LOVE, spriteContainer);
		MobStateIcon.SetIcon(base.gameObject, loveManager.relationshipStages[info.relationshipStage].tamePanelSprite, 99999f, 0.5f, 0.7f);
	}

	public void Date()
	{
		if (info.loveValue + lovePerTap < maxLoveAtThisStage || info.relationshipStage + 1 == loveManager.relationshipStages.Count)
		{
			PlayDateCutscene();
		}
		AddLoveValue(lovePerTap);
	}

	private void PlayDateCutscene()
	{
		int numberOfDates = Manager.Get<LoveManager>().numberOfDates;
		string name = "date " + UnityEngine.Random.Range(1, numberOfDates + 1).ToString();
		Manager.Get<SimpleCutsceneManager>().doAfterCutscene = delegate
		{
			Manager.Get<AchievementManager>().RegisterEvent("date");
		};
		Manager.Get<SimpleCutsceneManager>().ShowCutscene(name);
	}

	public void GiveGift(GiftCraftable gift)
	{
		bool favourite = loveManager.gifts.IndexOf(gift) == info.favouriteGiftIndex;
		Manager.Get<AchievementManager>().RegisterEvent("gift");
		float lovePerGift = Manager.Get<ModelManager>().loveSettings.GetLovePerGift(favourite);
		AddLoveValue(lovePerGift);
	}

	public void AddLoveValue(float value)
	{
		loveContext.loveValue = info.loveValue;
		loveContext.maxLoveValue = maxLoveAtThisStage;
		MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.IN_LOVE);
		info.loveValue += value;
		PlayHeartParticles(value);
		loveDeclineBlocked = 5f;
	}

	private void PlayHeartParticles(float value)
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_LOVE, new DatedHeartContext
		{
			heartStartPosition = base.transform.position,
			value = value,
			progress = info.loveValue / maxLoveAtThisStage
		});
	}

	private void Update()
	{
		CheckProgress();
		CheckBreakUp();
		UpdateLoveContext();
		UpdateMinutesSpentInRelationship();
		CheckIfNotTooFar();
	}

	private void UpdateLoveContext()
	{
		if (loveDeclineBlocked <= 0f)
		{
			info.loveValue -= loveReductionPerSecond() * Time.deltaTime;
			loveContext.loveValue = info.loveValue;
			loveContext.maxLoveValue = maxLoveAtThisStage;
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.IN_LOVE);
		}
		else
		{
			loveDeclineBlocked -= Time.deltaTime;
		}
	}

	private void CheckBreakUp()
	{
		if (info.loveValue < 0f)
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "love.left.you";
					t.defaultText = "Your partner has left you.";
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.gameObject.SetActive(value: false);
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "menu.ok";
					t.defaultText = "ok";
				}
			});
			loveManager.BreakUp();
		}
	}

	private void CheckProgress()
	{
		if (loveDeclineBlocked <= 0f && info.loveValue >= maxLoveAtThisStage && info.relationshipStage < loveManager.relationshipStages.Count - 1)
		{
			RelationshipStageUp();
		}
	}

	private void CheckIfNotTooFar()
	{
		float num = Vector3.Distance(base.transform.position, CameraController.instance.MainCamera.transform.position);
		if (num > 32f)
		{
			base.transform.position = CameraController.instance.MainCamera.transform.position + Vector3.up * 2f;
		}
	}

	private void RelationshipStageUp()
	{
		info.relationshipStage++;
		info.loveValue = 0.2f * maxLoveAtThisStage;
		Achievement achievement = (from v in Manager.Get<AchievementManager>().configs
			where v.modelSettingsKey == "relationship.stage"
			select v).FirstOrDefault();
		Manager.Get<StatsManager>().RelationshipLevelUp(info.relationshipStage);
		if (achievement != null && Manager.Get<AchievementManager>().achievementsKeeper[achievement] <= (double)info.relationshipStage)
		{
			Manager.Get<SimpleCutsceneManager>().doAfterCutscene = delegate
			{
				Manager.Get<AchievementManager>().RegisterEvent("relationship.stage");
			};
		}
		LoveManager.RelationshipStage relationshipStage = loveManager.relationshipStages[info.relationshipStage];
		relationshipStage.stageSetup.StartRelationshipStage();
		spriteContainer.sprite = relationshipStage.tamePanelSprite;
		MobStateIcon.SetIcon(base.gameObject, loveManager.relationshipStages[info.relationshipStage].tamePanelSprite, 99999f, 0.5f, 0.7f);
		MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.IN_LOVE);
	}

	private void UpdateMinutesSpentInRelationship()
	{
		if (info.minutesInRelationshipPerStage.Count == info.relationshipStage)
		{
			info.minutesInRelationshipPerStage.Add(0f);
		}
		float num = 0f;
		for (int i = 0; i < info.relationshipStage; i++)
		{
			num += info.minutesInRelationshipPerStage[i];
		}
		info.minutesInRelationshipPerStage[info.relationshipStage] = minutesSpentInRelationship - num;
	}

	private void OnDestroy()
	{
		if (MonoBehaviourSingleton<GameplayFacts>.get != null)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_LOVE, loveContext);
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_LOVE, spriteContainer);
		}
		if (GetComponent<SaveTransform>() != null)
		{
			UnityEngine.Object.Destroy(GetComponent<SaveTransform>());
		}
		MobStateIcon.RemoveIcon(base.gameObject);
	}

	private float loveReductionPerSecond()
	{
		if (loveManager.isTutorialOn)
		{
			return 0f;
		}
		return Manager.Get<ModelManager>().loveSettings.GetLoveDecrease(info.relationshipStage);
	}

	private DateTime DateTimeFromString(string dateTime)
	{
		return Convert.ToDateTime(dateTime, new CultureInfo("en-US"));
	}

	private string DateTimeToString(DateTime dateTime)
	{
		return dateTime.ToString(new CultureInfo("en-US"));
	}

	private IEnumerator DoAfterFrame(Action action)
	{
		yield return new WaitForEndOfFrame();
		action();
	}
}
