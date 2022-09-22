// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintItemCompleteWorker
using Common.Gameplay;
using Common.Managers;
using Gameplay;
using GameUI;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class BlueprintItemCompleteWorker : ScrollableItemWorker
{
	private BlueprintDataElement blueprint;

	private BlueprintItemConnector connector;

	public override void Work(ScrollableListElement element)
	{
		blueprint = (element as BlueprintDataElement);
		connector = (element.connector as BlueprintItemConnector);
		connector.blueprintIcon.sprite = blueprint.icon;
		connector.blueprintName.text = Manager.Get<TranslationsManager>().GetText(blueprint.blueprintConfigItem.translationsKey, blueprint.name).ToUpper();
		connector.requiredLevel.text = $"LVL {GetRequiredLevel()}";
		connector.buildingLimit.text = BuildingLimit();
		connector.currencyIcon.sprite = CurrencyIcon();
		connector.currencyIcon.enabled = (connector.currencyIcon.sprite != null);
		connector.currencyText.text = CurrencyText();
		connector.priceText.text = PriceText();
		connector.expText.text = ExpText();
		connector.lockedButtonGO.SetActive(value: false);
		bool flag = false;
		BlueprintDataElement blueprintDataElement = blueprint;
		BlueprintItemConnector blueprintItemConnector = connector;
		connector.bottomButton.onClick.RemoveAllListeners();
		if (IsMaxBuildingsReached())
		{
			SetMaxBuildingsReached(blueprintDataElement, blueprintItemConnector);
			flag = true;
		}
		else if (IsBought())
		{
			SetBoughtButtonLayout(blueprintDataElement, blueprintItemConnector);
			flag = true;
		}
		else if (IsLocked())
		{
			SetLockedButtonLayout(blueprintDataElement, blueprintItemConnector);
			flag = false;
		}
		else if (IsAffordable())
		{
			SetAffordableButtonLayout(blueprintDataElement, blueprintItemConnector);
			flag = true;
		}
		else
		{
			SetNotAffordableButtonLayout(blueprintDataElement, blueprintItemConnector);
			flag = true;
		}
		if (!flag)
		{
			connector.disabledGO.SetActive(value: true);
			return;
		}
		if (ExclamationMarkController.ExclamationMarkShown(blueprint.name) || !base.gameObject.activeInHierarchy)
		{
			connector.disabledGO.SetActive(value: false);
			return;
		}
		ExclamationMarkController.ShowExclamationMark(blueprint.name);
		connector.disabledGO.SetActive(value: true);
		connector.disabledGO.GetComponentInChildren<Animator>().SetTrigger("play");
	}

	private void SetMaxBuildingsReached(BlueprintDataElement blueprint, BlueprintItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: false);
		connector.buyButtonGO.SetActive(value: false);
		connector.unlockButtonGO.SetActive(value: false);
		connector.maxLimitReachedGO.SetActive(value: true);
		connector.bottomButton.interactable = false;
		connector.buttonColorController.category = ColorManager.ColorCategory.DISABLED_COLOR;
		connector.buttonColorController.UpdateColor();
	}

	private void SetBoughtButtonLayout(BlueprintDataElement blueprint, BlueprintItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: true);
		connector.buyButtonGO.SetActive(value: false);
		connector.unlockButtonGO.SetActive(value: false);
		connector.maxLimitReachedGO.SetActive(value: false);
		int id = blueprint.craftable.id;
		connector.bottomButton.onClick.AddListener(delegate
		{
			blueprint.craftable.customCraftableObject.GetComponent<BlueprintCraftableObject>().OnUseAction(id);
		});
		connector.buttonColorController.category = ColorManager.ColorCategory.HIGHLIGHT_COLOR;
		connector.buttonColorController.UpdateColor();
	}

	private void SetLockedButtonLayout(BlueprintDataElement blueprint, BlueprintItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: false);
		connector.buyButtonGO.SetActive(value: false);
		connector.unlockButtonGO.SetActive(value: true);
		connector.maxLimitReachedGO.SetActive(value: false);
		RequirementWrapper requirementWrapper = blueprint.blueprintConfigItem.requirements.FirstOrDefault((RequirementWrapper r) => r.requirement is AdRequirement && !r.CheckIfMet(blueprint.name));
		if (requirementWrapper != null)
		{
			connector.bottomButton.onClick.AddListener(delegate
			{
				Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_UNLOCK_CRAFTABLE, delegate(bool b)
				{
					if (b)
					{
						string key = $"{blueprint.name}.ads";
						if (!AutoRefreshingStock.HasItem(key))
						{
							AutoRefreshingStock.InitStockItem(key, float.NaN, int.MaxValue, 1);
						}
						else
						{
							AutoRefreshingStock.IncrementStockCount(key);
						}
						PlayerPrefs.Save();
						blueprint.isDirty = true;
						connector.bottomButton.onClick.RemoveAllListeners();
					}
				});
			});
			connector.buttonColorController.category = ColorManager.ColorCategory.MAIN_COLOR;
			connector.buttonColorController.UpdateColor();
		}
		else
		{
			connector.bottomButton.interactable = false;
			connector.buttonColorController.category = ColorManager.ColorCategory.DISABLED_COLOR;
			connector.buttonColorController.UpdateColor();
			connector.unlockButtonGO.SetActive(value: false);
			connector.lockedButtonGO.SetActive(value: true);
		}
	}

	private void SetAffordableButtonLayout(BlueprintDataElement blueprint, BlueprintItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: false);
		connector.buyButtonGO.SetActive(value: true);
		connector.unlockButtonGO.SetActive(value: false);
		connector.maxLimitReachedGO.SetActive(value: false);
		connector.bottomButton.onClick.AddListener(delegate
		{
			blueprint.blueprintConfigItem.prices.ForEach(delegate(PriceWrapper p)
			{
				p.currency.TryToBuySomething(blueprint.name, p.defaultAmount);
			});
			Singleton<PlayerData>.get.playerItems.AddCraftable(blueprint.craftable.id, 1);
			blueprint.isDirty = true;
			connector.bottomButton.onClick.RemoveAllListeners();
		});
		connector.buttonColorController.category = ColorManager.ColorCategory.HIGHLIGHT_COLOR;
		connector.buttonColorController.UpdateColor();
	}

	private void SetNotAffordableButtonLayout(BlueprintDataElement blueprint, BlueprintItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: false);
		connector.buyButtonGO.SetActive(value: true);
		connector.unlockButtonGO.SetActive(value: false);
		connector.maxLimitReachedGO.SetActive(value: false);
		connector.bottomButton.onClick.AddListener(delegate
		{
			blueprint.blueprintConfigItem.prices.FirstOrDefault((PriceWrapper p) => !p.currency.CanIBuyThis(blueprint.name, p.defaultAmount) && p.currency.CanGetMoreCurrency())?.currency.GetMoreCurrency();
		});
		connector.buttonColorController.category = ColorManager.ColorCategory.DISABLED_COLOR;
		connector.buttonColorController.UpdateColor();
	}

	private int GetRequiredLevel()
	{
		float? num = blueprint.blueprintConfigItem.requirements.FirstOrDefault((RequirementWrapper r) => r.requirement is LevelRequirement)?.value;
		return (int)((!num.HasValue) ? 0f : num.Value);
	}

	private string BuildingLimit()
	{
		RequirementWrapper requirementWrapper = blueprint.blueprintConfigItem.requirements.FirstOrDefault((RequirementWrapper r) => r.requirement is BuildingLimitPerLevelRequirement);
		if (requirementWrapper == null)
		{
			return string.Empty;
		}
		string key = $"{blueprint.name}.built";
		int stockCount = AutoRefreshingStock.GetStockCount(key, float.NaN, int.MaxValue, 0);
		int num = (requirementWrapper.requirement as BuildingLimitPerLevelRequirement).MaxAmount(requirementWrapper.data);
		if (num == int.MaxValue)
		{
			return string.Empty;
		}
		return string.Format($"{stockCount}/{num}");
	}

	private Sprite CurrencyIcon()
	{
		Reward obj = blueprint.blueprintConfigItem.rewards.FirstOrDefault((RewardWrapper r) => r.reward is CurrencyReward)?.reward;
		return Manager.Get<ShopManager>().GetCurrencyItem((obj as CurrencyReward)?.currency)?.sprite;
	}

	private string CurrencyText()
	{
		RewardWrapper rewardWrapper = blueprint.blueprintConfigItem.rewards.FirstOrDefault((RewardWrapper r) => r.reward is CurrencyReward);
		if (rewardWrapper != null)
		{
			return $"+{rewardWrapper.amount.ToString()}";
		}
		return string.Empty;
	}

	private string PriceText()
	{
		PriceWrapper priceWrapper = blueprint.blueprintConfigItem.prices.FirstOrDefault();
		return priceWrapper?.currency.GetPrice(blueprint.name, priceWrapper.defaultAmount).ToString() ?? string.Empty;
	}

	private string ExpText()
	{
		RewardWrapper rewardWrapper = blueprint.blueprintConfigItem.rewards.FirstOrDefault((RewardWrapper r) => r.reward is ExpReward);
		if (rewardWrapper != null)
		{
			return $"EXP +{rewardWrapper.amount.ToString()}";
		}
		return string.Empty;
	}

	private bool IsMaxBuildingsReached()
	{
		RequirementWrapper requirementWrapper = blueprint.blueprintConfigItem.requirements.FirstOrDefault((RequirementWrapper r) => r.requirement is BuildingLimitPerLevelRequirement);
		if (requirementWrapper == null)
		{
			return false;
		}
		string key = $"{blueprint.name}.built";
		int stockCount = AutoRefreshingStock.GetStockCount(key, float.NaN, int.MaxValue, 0);
		int num = (requirementWrapper.requirement as BuildingLimitPerLevelRequirement).MaxAmount(requirementWrapper.data);
		return stockCount >= num;
	}

	private bool IsBought()
	{
		return !IsLocked() && (Singleton<PlayerData>.get.playerItems.GetCraftableCount(blueprint.craftable.id) > 0 || blueprint.blueprintConfigItem.prices.Count == 0);
	}

	private bool IsLocked()
	{
		return blueprint.blueprintConfigItem.requirements.Any((RequirementWrapper r) => !r.CheckIfMet(blueprint.name));
	}

	private bool IsAffordable()
	{
		return blueprint.blueprintConfigItem.prices.All((PriceWrapper p) => p.currency.CanIBuyThis(blueprint.name, p.defaultAmount));
	}
}
