// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerPotionsDisplayer
using Common.Managers;
using ItemVInventory;
using States;
using UnityEngine;

public class PlayerPotionsDisplayer : MonoBehaviour
{
	public string potionId = "Potion";

	public ItemType potionType;

	public int startingPotions = 5;

	public Achievement achievement;

	private Backpack backpack;

	private PotionsContext potionsContext;

	private void Awake()
	{
		backpack = GetComponentInChildren<Backpack>();
		if (backpack == null)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			potionsContext = (MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<PotionsContext>(Fact.POTIONS) ?? new PotionsContext());
		}
	}

	private void Start()
	{
		if (!PlayerPrefs.HasKey("items.potions"))
		{
			AddPotions();
			PlayerPrefs.SetInt("items.potions", 1);
		}
		PotionsUpdate();
	}

	private void Update()
	{
		PotionsUpdate();
	}

	public void PotionsUpdate()
	{
		potionsContext.leftPotions = backpack.GetItemsCount(potionType);
		potionsContext.onButtonClicked = TakePotion;
		if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.POTIONS, potionsContext))
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.POTIONS);
		}
	}

	private void TakePotion()
	{
		Health componentInChildren = GetComponentInChildren<Health>();
		if ((componentInChildren == null || componentInChildren.hp == componentInChildren.maxHp) && backpack.GetItemsCount(potionType) > 0)
		{
			return;
		}
		if (backpack.TryTakeItem(potionType, -1, potionId))
		{
			componentInChildren.hp += componentInChildren.maxHp * 0.3f;
			if (achievement != null)
			{
				achievement.ReportProgress();
			}
		}
		else
		{
			string description = string.Format(Manager.Get<TranslationsManager>().GetText("get.potions.watch.ad", "Watch ad to get {0} potions"), startingPotions);
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				numberOfAdsNeeded = 1,
				translationKey = string.Empty,
				description = description,
				reason = StatsManager.AdReason.XSURVIVAL_GET_POTIONS,
				immediatelyAd = false,
				type = AdsCounters.None,
				onSuccess = delegate(bool b)
				{
					if (b)
					{
						AddPotions();
					}
				},
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren2 = go.GetComponentInChildren<TranslateText>();
					componentInChildren2.translationKey = "menu.watch";
					componentInChildren2.defaultText = "Watch";
					componentInChildren2.ForceRefresh();
				}
			});
		}
		PotionsUpdate();
	}

	private void AddPotions()
	{
		for (int i = 0; i < startingPotions; i++)
		{
			backpack.TryPlace(potionType, 0, potionId);
		}
	}
}
