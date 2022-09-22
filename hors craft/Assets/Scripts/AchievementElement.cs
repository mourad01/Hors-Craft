// DecompilerFi decompiler from Assembly-CSharp.dll class: AchievementElement
using Common.Managers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class AchievementElement : MonoBehaviour
{
	private AchievementStep currentStep;

	private Achievement config;

	[SerializeField]
	private GameObject StarPrefab;

	[SerializeField]
	private GameObject fullStarPrefab;

	[SerializeField]
	private Transform starsHolder;

	[SerializeField]
	private Sprite FullStarSprite;

	[SerializeField]
	private Text rewardText;

	[SerializeField]
	private Image rewardImage;

	[SerializeField]
	private TranslateText titleText;

	[SerializeField]
	private TranslateText descriptionText;

	[SerializeField]
	private Text currentCountText;

	[SerializeField]
	private ClaimAchievementButton claimButton;

	private int starIndex;

	public void Init(Achievement config)
	{
		this.config = config;
		currentStep = config.GetCurrentAchievementStep();
		InitUI();
	}

	private void ClaimEffects()
	{
		UpdateTexts();
		UpdateButton();
		AssignStar();
		UpdateImages();
	}

	private void InitUI()
	{
		descriptionText.AddVisitor((string s) => s.Formatted(currentStep.countToUnlock));
		InitStars();
		UpdateTexts();
		UpdateButton();
		UpdateImages();
		claimButton.Init(this);
	}

	private void InitStars()
	{
		int stepsCount = config.GetStepsCount();
		int num = starIndex = config.GetCurrentStepIndex();
		for (int i = 0; i < stepsCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(StarPrefab, starsHolder, worldPositionStays: false);
			if ((i < num) ? true : false)
			{
				if (fullStarPrefab != null)
				{
					Object.Instantiate(fullStarPrefab, gameObject.transform, worldPositionStays: false);
				}
				else
				{
					gameObject.GetComponent<Image>().sprite = FullStarSprite;
				}
			}
		}
	}

	private void UpdateImages()
	{
		if (currentStep.reward.baseSprite != null)
		{
			rewardImage.sprite = currentStep.reward.baseSprite;
		}
		else if (Manager.Contains<ShopManager>())
		{
			rewardImage.sprite = Manager.Get<ShopManager>().GetCurrencyItem("soft").sprite;
		}
	}

	private void AssignStar()
	{
		if (fullStarPrefab != null)
		{
			Object.Instantiate(fullStarPrefab, starsHolder.GetChild(starIndex), worldPositionStays: false);
		}
		else
		{
			starsHolder.GetChild(starIndex).GetComponent<Image>().sprite = FullStarSprite;
		}
		starIndex++;
	}

	private void UpdateTexts()
	{
		titleText.translationKey = currentStep.achievement.titleTranslationKey;
		titleText.defaultText = currentStep.achievement.titleTranslationDefaultText;
		titleText.ForceRefresh();
		descriptionText.translationKey = currentStep.achievement.descriptionTranslationKey;
		descriptionText.defaultText = currentStep.achievement.descriptionTranslationDefaultText;
		descriptionText.ForceRefresh();
		double num = Manager.Get<AchievementManager>().achievementsKeeper[config];
		double countToUnlock = currentStep.countToUnlock;
		currentCountText.text = $"{num:0}/{countToUnlock}";
		if (currentStep.rewardAmount > 1)
		{
			rewardText.gameObject.SetActive(value: true);
			rewardText.text = currentStep.rewardAmount.ToString();
		}
		else
		{
			rewardText.gameObject.SetActive(value: false);
		}
	}

	private void UpdateButton()
	{
		bool isUnlocked = currentStep.isUnlocked;
		bool isDone = config.IsAllClaimed();
		claimButton.UpdateButton(isUnlocked, isDone);
	}

	public void ClaimAchievement()
	{
		Manager.Get<AchievementManager>().Claim(currentStep);
		config.ClaimAchievementStep(currentStep);
		currentStep = config.GetCurrentAchievementStep();
		ClaimEffects();
		Manager.Get<AchievementManager>().IncreaseProgressBar();
	}

	private void AddCurrency(double amount)
	{
		if (Manager.Contains<AbstractSoftCurrencyManager>())
		{
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange((int)amount);
		}
	}
}
