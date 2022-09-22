// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ClaimRewardsState
using Common.Gameplay;
using Common.Managers;
using Common.Managers.States;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class ClaimRewardsState : XCraftUIState<ClaimRewardsStateConnector>
	{
		public List<RewardTypeConfig> rewardsConfig = new List<RewardTypeConfig>();

		public Action OnFinish;

		private ClaimRewardsStateStartParameter startParameter;

		public GameObject spawnedChest;

		private Dictionary<ChestRewardType, List<Reward>> chestRewardTypeToRewards;

		private int counter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as ClaimRewardsStateStartParameter);
			spawnedChest = startParameter.spawnedChest;
			counter = 0;
			chestRewardTypeToRewards = new Dictionary<ChestRewardType, List<Reward>>();
			foreach (Reward reward in startParameter.rewards)
			{
				if (reward.amount != 0)
				{
					ChestRewardType key = CheckRewardType(reward);
					if (!chestRewardTypeToRewards.ContainsKey(key))
					{
						chestRewardTypeToRewards.Add(key, new List<Reward>());
					}
					chestRewardTypeToRewards[key].Add(reward);
				}
			}
			InitNewCategory();
		}

		private void InitNewCategory()
		{
			UnityEngine.Object.Destroy(base.connector.CurrentPanel);
			ChestRewardType rewardType = rewardsConfig[counter].rewardType;
			while (!chestRewardTypeToRewards.ContainsKey(rewardType))
			{
				counter++;
				rewardType = rewardsConfig[counter].rewardType;
			}
			GameObject panelPrefab = rewardsConfig[counter].panelPrefab;
			if (panelPrefab == null)
			{
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(rewardsConfig[counter].panelPrefab, base.connector.transform, worldPositionStays: false);
			base.connector.CurrentPanel = gameObject;
			ClaimRewardsPanel component = gameObject.GetComponent<ClaimRewardsPanel>();
			component.rewardName.defaultText = rewardsConfig[counter].defaultName;
			component.rewardName.translationKey = rewardsConfig[counter].key;
			component.rewardName.ForceRefresh();
			List<Sprite> list = new List<Sprite>();
			foreach (Reward item in chestRewardTypeToRewards[rewardType])
			{
				for (int i = 0; i < item.amount; i++)
				{
					if (item.baseSprite != null)
					{
						list.Add(item.baseSprite);
					}
					else
					{
						list.AddRange(item.GetSprites());
					}
				}
			}
			Dictionary<Sprite, int> dictionary = new Dictionary<Sprite, int>();
			for (int j = 0; j < list.Count; j++)
			{
				if (dictionary.TryGetValue(list[j], out int value))
				{
					dictionary[list[j]] = value + 1;
				}
				else
				{
					dictionary.Add(list[j], 1);
				}
			}
			foreach (KeyValuePair<Sprite, int> item2 in dictionary)
			{
				AddReward(item2.Key, item2.Value, component.itemParent, rewardsConfig[counter].itemPrefab);
			}
			component.okButton.onClick.AddListener(OnClaim);
			counter++;
			chestRewardTypeToRewards.Remove(rewardType);
		}

		private void AddReward(Sprite sprite, int count, Transform itemParent, GameObject itemPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab, itemParent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			DailyRewardsItem component = gameObject.GetComponent<DailyRewardsItem>();
			Color colorForCategory = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.FONT_COLOR);
			component.Init(string.Empty, string.Empty, Color.white, colorForCategory, sprite, wasClaimed: true);
			component.SetCounter(count);
		}

		private void OnClaim()
		{
			if (chestRewardTypeToRewards.IsNullOrEmpty())
			{
				if (spawnedChest != null)
				{
					UnityEngine.Object.Destroy(spawnedChest);
				}
				Manager.Get<StateMachineManager>().PopState();
			}
			else
			{
				InitNewCategory();
			}
		}

		public override void FinishState()
		{
			base.FinishState();
			if (OnFinish != null)
			{
				OnFinish();
				OnFinish = null;
			}
		}

		private ChestRewardType CheckRewardType(Reward reward)
		{
			foreach (RewardTypeConfig item in rewardsConfig)
			{
				if (item.rewards.Contains(reward))
				{
					return item.rewardType;
				}
			}
			return ChestRewardType.DEFAULT;
		}
	}
}
