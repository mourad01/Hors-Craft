// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DailyRewardsState
using Common.Audio;
using Common.Gameplay;
using Common.Managers;
using Common.Managers.Audio;
using Common.Managers.States;
using Gameplay;
using Gameplay.Audio;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class DailyRewardsState : XCraftUIState<DailyRewardsStateConnector>
	{
		private const string DAY_KEY = "daily.rewards.day";

		private const string DAY_DEFAULT = "DAY";

		public Action OnFinish;

		public Color rewardTodayBorder;

		public Color rewardTodayText;

		public Color rewardOtherBorder;

		public Color rewardOtherText;

		public GameObject giftPrefab;

		private List<Sprite> claimedRewardsSprites = new List<Sprite>();

		private GameObject giftSpawned;

		private DailyRewardsManager dailyRewardsManager;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			dailyRewardsManager = Manager.Get<DailyRewardsManager>();
			if (!(dailyRewardsManager != null) || !dailyRewardsManager.hasRewardToday)
			{
				ExitState();
			}
			else
			{
				InitConnectorDaily();
			}
		}

		public override void FinishState()
		{
			base.connector.Clear();
			base.FinishState();
			if (OnFinish != null)
			{
				OnFinish();
				OnFinish = null;
			}
		}

		private string GetKeyByRewardType(DailyRewards rewardType)
		{
			return "daily.rewards." + rewardType.ToString().ToLower();
		}

		private string GetRewardText(DailyRewardsModule.RewardConfig config)
		{
			string str = config.count.ToString() + "x ";
			DailyRewards id = (DailyRewards)config.id;
			return str + Manager.Get<TranslationsManager>().GetText(GetKeyByRewardType(id), id.ToString());
		}

		private string GetDayText(int day)
		{
			return Manager.Get<TranslationsManager>().GetText("daily.rewards.day", "DAY") + " " + day;
		}

		private void InitConnectorDaily()
		{
			base.connector.Init();
			SetCallbacks();
			FillDays();
			SelectDay(dailyRewardsManager.daysInRowWrapped);
		}

		private void SetCallbacks()
		{
			base.connector.onClaim = OnClaim;
			base.connector.onExit = ExitState;
		}

		private void FillDays()
		{
			DailyRewardsModule.RewardConfig[] array = dailyRewardsManager.FiltredRewardConfigs();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].id >= Enum.GetValues(typeof(DailyRewards)).Length)
				{
					array[i].id = Mathf.Clamp(array[i].id, 0, Enum.GetValues(typeof(DailyRewards)).Length - 1);
				}
				DailyRewards id = (DailyRewards)array[i].id;
				Reward rewardObject = Manager.Get<RewardsManager>().GetRewardObject(id.ToString().ToLower());
				base.connector.AddDay(GetDayText(i + 1), GetRewardText(array[i]), rewardOtherBorder, rewardOtherText, rewardObject.baseSprite, dailyRewardsManager.daysInRowWrapped - 1 > i);
			}
		}

		private void SelectDay(int day)
		{
			base.connector.SetActiveDay(day, rewardTodayBorder, rewardTodayText);
		}

		private void FillClaimedRewards()
		{
			Dictionary<Sprite, int> dictionary = new Dictionary<Sprite, int>();
			for (int i = 0; i < claimedRewardsSprites.Count; i++)
			{
				if (dictionary.TryGetValue(claimedRewardsSprites[i], out int value))
				{
					dictionary[claimedRewardsSprites[i]] = value + 1;
				}
				else
				{
					dictionary.Add(claimedRewardsSprites[i], 1);
				}
			}
			foreach (KeyValuePair<Sprite, int> item in dictionary)
			{
				base.connector.AddClaimed(rewardTodayBorder, item.Key, item.Value);
			}
		}

		private void OnClaim()
		{
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.DAILY_REWARDS_CLICKED);
			GetReward();
			dailyRewardsManager.Claim();
			FillClaimedRewards();
			ShowRewardAnimation();
			PlayAudio();
		}

		private void GetReward()
		{
			claimedRewardsSprites.Clear();
			DailyRewardsModule.RewardConfig todayRewardConfig = dailyRewardsManager.todayRewardConfig;
			DailyRewards id = (DailyRewards)todayRewardConfig.id;
			for (int i = 0; i < todayRewardConfig.count; i++)
			{
				Reward rewardObject = Manager.Get<RewardsManager>().GetRewardObject(id.ToString().ToLower());
				claimedRewardsSprites.AddRange(rewardObject.GetSprites());
				rewardObject.ClaimReward();
			}
		}

		private void ShowRewardAnimation()
		{
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			base.connector.ShowAnimation();
			giftSpawned = UnityEngine.Object.Instantiate(giftPrefab);
		}

		private void PlayAudio()
		{
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.DAILY_REWARD);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}

		private void ExitState()
		{
			UnityEngine.Object.Destroy(giftSpawned);
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
