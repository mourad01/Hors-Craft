// DecompilerFi decompiler from Assembly-CSharp.dll class: AchievementToast
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class AchievementToast : TopNotification
{
	public delegate void OnClick();

	public class AchievementUnlockedInformation : ShowInformation
	{
		public Sprite icon;

		public string description;

		public string descriptionDefault;

		public string informationDefault;

		public double count;
	}

	public Button claimButton;

	public Button closeButton;

	public TranslateText titleText;

	public TranslateText descriptionText;

	public Image iconImage;

	public OnClick onNotificationButtonClicked;

	private void Awake()
	{
		descriptionText.ClearVisitors();
		claimButton.onClick.AddListener(delegate
		{
			Claim();
		});
		closeButton.onClick.AddListener(HideImmediately);
	}

	private void Claim()
	{
		if (Manager.Get<StateMachineManager>().currentState is GameplayState)
		{
			GoToAchievementsFragment();
		}
		else
		{
			ShowPopup();
		}
		HideImmediately();
	}

	private void GoToAchievementsFragment()
	{
		if (Manager.Get<StateMachineManager>().currentState.GetType() == typeof(PauseState))
		{
			Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Achievements");
		}
		else
		{
			Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
			{
				canSave = true,
				allowTimeChange = true,
				categoryToOpen = "Achievements"
			});
		}
	}

	private void ShowPopup()
	{
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureMessage = delegate(TranslateText t)
			{
				t.translationKey = "achievements.go.to.pause";
				t.defaultText = "Go to the menu to claim your reward!";
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
	}

	public override void SetElement(ShowInformation information)
	{
		AchievementUnlockedInformation achievementUnlockedInfo = information as AchievementUnlockedInformation;
		descriptionText.ClearVisitors();
		descriptionText.AddVisitor((string s) => s.Formatted(achievementUnlockedInfo.count));
		UpdateTranslations(achievementUnlockedInfo);
		if (!(achievementUnlockedInfo.icon == null))
		{
		}
	}

	public void ShowAchievementUnlocked(AchievementStep step, float waitTime)
	{
		ShowInformation showInformation = new ShowInformation();
		showInformation.timeToHide = 5f;
		Show(showInformation);
	}

	private void UpdateTranslations(AchievementUnlockedInformation info)
	{
		descriptionText.translationKey = info.description;
		descriptionText.defaultText = info.descriptionDefault;
		descriptionText.ForceRefresh();
		titleText.translationKey = info.information;
		titleText.defaultText = info.informationDefault;
		titleText.ForceRefresh();
	}
}
