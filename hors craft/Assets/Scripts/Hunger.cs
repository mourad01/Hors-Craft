// DecompilerFi decompiler from Assembly-CSharp.dll class: Hunger
using Common.Managers;
using Gameplay;
using System;
using UnityEngine;

public class Hunger : MonoBehaviour, IGameCallbacksListener
{
	[HideInInspector]
	public bool hungerEnabled;

	public float maxFeed = 100f;

	private const float HUNGER_LOSS_PER_SECOND = 0.2f;

	private const float DEFAULT_FOOD_VALUE = 15f;

	private const float HP_LOSS_PER_SECOND = 0.1f;

	private const float HP_LOSS_INTERVAL = 5f;

	private HungerContext hungerContext;

	private float nextHpLossTime;

	public Action OnFeedLevelIsExhausted;

	public bool saveLevelToWorldPrefs = true;

	private const string HUNGER_KEY = "hunger";

	public float feedLevel
	{
		get;
		private set;
	}

	private void Awake()
	{
		hungerEnabled = false;
	}

	public void Activate()
	{
		hungerEnabled = true;
		Manager.Get<GameCallbacksManager>().UnregisterObject(this);
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		OnGameplayStarted();
		if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.HUNGER))
		{
			hungerContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HungerContext>(Fact.HUNGER).GetLastItem();
			return;
		}
		hungerContext = new HungerContext
		{
			hunger = feedLevel / maxFeed
		};
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.HUNGER, hungerContext);
	}

	public void Feed(int quantity)
	{
		feedLevel = Mathf.Min(maxFeed, feedLevel + (float)quantity * 15f);
	}

	private void Update()
	{
		if (!hungerEnabled)
		{
			return;
		}
		if (feedLevel > 0f)
		{
			feedLevel -= 0.2f * Time.deltaTime;
			hungerContext.hunger = feedLevel / maxFeed;
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.HUNGER);
		}
		else if (Time.time > nextHpLossTime)
		{
			Health componentInParent = GetComponentInParent<Health>();
			if (componentInParent != null)
			{
				componentInParent.OnHit(0.5f, Vector3.zero);
			}
			nextHpLossTime = Time.time + 5f;
			if (OnFeedLevelIsExhausted != null)
			{
				OnFeedLevelIsExhausted();
				OnFeedLevelIsExhausted = null;
			}
		}
	}

	public void OnGameSavedFrequent()
	{
		if (saveLevelToWorldPrefs)
		{
			WorldPlayerPrefs.get.SetFloat("hunger", feedLevel);
		}
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayStarted()
	{
		if (saveLevelToWorldPrefs)
		{
			feedLevel = WorldPlayerPrefs.get.GetFloat("hunger", maxFeed);
		}
		else
		{
			feedLevel = maxFeed;
		}
	}

	public void OnGameplayRestarted()
	{
		if (saveLevelToWorldPrefs)
		{
			WorldPlayerPrefs.get.DeleteKey("hunger");
		}
	}

	private void OnDestroy()
	{
		if (Manager.Get<GameCallbacksManager>() != null)
		{
			Manager.Get<GameCallbacksManager>().UnregisterObject(this);
		}
	}
}
