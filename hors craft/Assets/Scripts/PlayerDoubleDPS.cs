// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerDoubleDPS
using Common.Managers;
using Gameplay;
using States;
using UnityEngine;

public class PlayerDoubleDPS : MonoBehaviour
{
	public const string prefsKey = "double.dps";

	public PlayerStats statsToDouble;

	public float maxTime = 5f;

	public Achievement achievement;

	private DoubleDPSContext context;

	private bool active;

	private float leftTime;

	private PlayerStats.Modifier _modifier;

	private PlayerStats.Modifier modifier => _modifier ?? (_modifier = new PlayerStats.Modifier
	{
		priority = 200,
		value = 2f,
		Action = ((float value, float old) => old * value)
	});

	private void Start()
	{
		InitData();
		context = (MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<DoubleDPSContext>(Fact.DOUBLE_DPS) ?? new DoubleDPSContext());
		maxTime = Manager.Get<ModelManager>().itemsUpgradeStatsSettings.GetDoubleDPSTime((int)maxTime);
		UpdateContext();
	}

	private void Update()
	{
		if (active)
		{
			leftTime -= Time.deltaTime;
			if (leftTime < 0f)
			{
				active = false;
				statsToDouble.Remove(modifier);
			}
			UpdateContext();
		}
	}

	private void InitData()
	{
		if (!PlayerPrefs.HasKey("double.dps"))
		{
			active = false;
			leftTime = 0f;
		}
	}

	private void DoubleDPSClicked()
	{
		if (!active)
		{
			string text = Manager.Get<TranslationsManager>().GetText("double.dps.watch.ad", "Watch ad to double dps");
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				numberOfAdsNeeded = 1,
				translationKey = string.Empty,
				description = text,
				reason = StatsManager.AdReason.XSURVIVAL_DOUBLE_DPS,
				immediatelyAd = false,
				type = AdsCounters.None,
				onSuccess = delegate(bool b)
				{
					if (b)
					{
						Active();
					}
				},
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.translationKey = "menu.watch";
					componentInChildren.defaultText = "Watch";
					componentInChildren.ForceRefresh();
				}
			});
		}
	}

	private void Active()
	{
		leftTime = maxTime;
		active = true;
		UpdateContext();
		statsToDouble.Add(modifier);
		if (achievement != null)
		{
			achievement.ReportProgress();
		}
	}

	private void UpdateContext()
	{
		context.active = active;
		context.leftTime = leftTime;
		context.proggress = leftTime / maxTime;
		context.onButtonClicked = DoubleDPSClicked;
		if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.DOUBLE_DPS, context))
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.DOUBLE_DPS);
		}
	}
}
