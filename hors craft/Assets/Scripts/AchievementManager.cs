// DecompilerFi decompiler from Assembly-CSharp.dll class: AchievementManager
using Common.Gameplay;
using Common.Managers;
using Gameplay;
using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : AbstractAchievementManager
{
	public TopNotification achievementNotificationPrefab;

	private TopNotification _achievementNotificationInstance;

	private AchievementsFragment achievementsFragmant;

	private TopNotification achievementNotificationInstance
	{
		get
		{
			if (_achievementNotificationInstance == null && achievementNotificationPrefab != null)
			{
				_achievementNotificationInstance = Object.Instantiate(achievementNotificationPrefab.gameObject).GetComponent<TopNotification>();
			}
			return _achievementNotificationInstance;
		}
	}

	public override void Init()
	{
		configs.Clear();
	}

	protected override Reward GetReward(int rewardIndex)
	{
		RewardsManager rewardsManager = Manager.Get<RewardsManager>();
		DailyRewards dailyRewards = (DailyRewards)rewardIndex;
		return rewardsManager.GetRewardObject(dailyRewards.ToString().ToLower());
	}

	public void Claim(AchievementStep step)
	{
		step.reward.amount = step.rewardAmount;
		step.reward.ClaimReward();
	}

	public void IncreaseProgressBar()
	{
		CalculateAchievedPoints();
		if (achievementsFragmant == null)
		{
			achievementsFragmant = Object.FindObjectOfType<AchievementsFragment>();
		}
		if (!(achievementsFragmant == null))
		{
			achievementsFragmant.UpdateProgressBar(achievedPoints, maxPoints);
		}
	}

	public override void RaiseAchievement(List<AchievementStep> steps)
	{
		StartCoroutine(RaiseAchievementCO(steps));
	}

	public IEnumerator RaiseAchievementCO(List<AchievementStep> steps)
	{
		for (int i = steps.Count - 1; i >= 0; i--)
		{
			ShowAchievemntNotification(steps[i], achievementToastWaitTime);
			yield return new WaitForSecondsRealtime(achievementToastWaitTime + 1f);
		}
		SavePrefs();
	}

	public void ShowAchievemntNotification(AchievementStep step, float duration)
	{
		if (!(achievementNotificationInstance == null) && !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
		{
			AchievementToast.AchievementUnlockedInformation achievementUnlockedInformation = new AchievementToast.AchievementUnlockedInformation();
			achievementUnlockedInformation.information = step.achievement.titleTranslationKey;
			achievementUnlockedInformation.description = step.achievement.descriptionTranslationKey;
			achievementUnlockedInformation.informationDefault = step.achievement.titleTranslationDefaultText;
			achievementUnlockedInformation.descriptionDefault = step.achievement.descriptionTranslationDefaultText;
			achievementUnlockedInformation.setOnTop = true;
			achievementUnlockedInformation.timeToHide = duration;
			achievementUnlockedInformation.count = step.countToUnlock;
			achievementUnlockedInformation.icon = step.achievement.achivementIcon;
			AchievementToast.AchievementUnlockedInformation information = achievementUnlockedInformation;
			achievementNotificationInstance.Show(information);
			float y = (float)Screen.height * 0.05f;
			achievementNotificationInstance.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, y);
		}
	}
}
