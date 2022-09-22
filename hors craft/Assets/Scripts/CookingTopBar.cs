// DecompilerFi decompiler from Assembly-CSharp.dll class: CookingTopBar
using Common.Managers;
using Cooking;
using Gameplay;
using States;
using UnityEngine;
using UnityEngine.UI;

public class CookingTopBar : MonoBehaviour
{
	public Text prestigeLevel;

	public Slider prestigeProgress;

	public Text gold;

	public Button watchAdButton;

	public Text watchAdText;

	private void Awake()
	{
		watchAdButton.onClick.AddListener(delegate
		{
			OnWatchAd();
		});
		watchAdButton.GetComponentInChildren<TranslateText>().AddVisitor((string t) => t.Replace("{0}", Manager.Get<ModelManager>().cookingSettings.MoneyPerAd().ToString()));
		InitVariables();
	}

	private void InitVariables()
	{
		WorkController workController = Manager.Get<CookingManager>().workController;
		prestigeLevel.text = workController.workData.prestigeLevel.ToString();
		prestigeProgress.value = workController.workData.prestige / (float)workController.workData.prestigeToLevelUp;
		gold.text = workController.workData.money.ToString();
		watchAdText.text = Manager.Get<ModelManager>().cookingSettings.MoneyPerAd().ToString();
	}

	public void Update()
	{
		InitVariables();
	}

	private void OnWatchAd()
	{
		string text = Manager.Get<TranslationsManager>().GetText("cooking.watch.ad", "Watch ad to get {1} coins").ToUpper();
		text = text.Replace("{1}", Manager.Get<ModelManager>().cookingSettings.MoneyPerAd().ToString());
		Manager.Get<CookingManager>().HideTopBar();
		Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
		{
			numberOfAdsNeeded = 1,
			translationKey = string.Empty,
			description = text,
			reason = StatsManager.AdReason.COOKING_CURRENCY,
			immediatelyAd = false,
			type = AdsCounters.Currency,
			onSuccess = delegate(bool b)
			{
				if (b)
				{
					Manager.Get<CookingManager>().workController.workData.money += Manager.Get<ModelManager>().cookingSettings.MoneyPerAd();
				}
			},
			configWatchButton = delegate(GameObject go)
			{
				TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
				componentInChildren.translationKey = "menu.watch";
				componentInChildren.defaultText = "watch";
				componentInChildren.ForceRefresh();
			}
		});
	}
}
