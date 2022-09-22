// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DailyRewardsStateConnector
using Common.Managers.States.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DailyRewardsStateConnector : UIConnector
	{
		[SerializeField]
		private GameObject rewardTabItemPrefab;

		[SerializeField]
		private GameObject claimedTabItemPrefab;

		[SerializeField]
		private Button claimButton;

		[SerializeField]
		private Button exitButton;

		[SerializeField]
		private GameObject allRewardsTab;

		[SerializeField]
		private GameObject claimedRewardsTab;

		[SerializeField]
		private Transform allRewardsRewardList;

		[SerializeField]
		private Transform claimedRewardsRewardList;

		[SerializeField]
		private Scrollbar rewardsScrollbar;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private Animator animator;

		public Action onClaim;

		public Action onExit;

		private List<DailyRewardsItem> dayTabItems = new List<DailyRewardsItem>();

		private List<DailyRewardsItem> claimedTabItems = new List<DailyRewardsItem>();

		public void Init()
		{
			InitButtons();
			allRewardsTab.SetActive(value: true);
			claimedRewardsTab.SetActive(value: false);
		}

		public void Clear()
		{
			ClearAllRewardsTab();
			ClearClaimedRewardTab();
			allRewardsTab.SetActive(value: false);
			claimedRewardsTab.SetActive(value: false);
		}

		public void AddDay(string day, string rewardsText, Color border, Color text, Sprite rewardSprite, bool wasClaimed)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(rewardTabItemPrefab);
			DailyRewardsItem component = gameObject.GetComponent<DailyRewardsItem>();
			if (component == null)
			{
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			gameObject.transform.parent = allRewardsRewardList;
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = 0f;
			gameObject.transform.localPosition = localPosition;
			dayTabItems.Add(component);
			component.Init(day, rewardsText, border, text, rewardSprite, wasClaimed);
		}

		public void AddClaimed(Color border, Sprite rewardSprite, int count)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(claimedTabItemPrefab);
			DailyRewardsItem component = gameObject.GetComponent<DailyRewardsItem>();
			if (component == null)
			{
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			gameObject.transform.parent = claimedRewardsRewardList;
			claimedTabItems.Add(component);
			component.Init(string.Empty, string.Empty, border, Color.black, rewardSprite, wasClaimed: false);
			component.SetCounter(count);
		}

		public void SetActiveDay(int day, Color border, Color text)
		{
			int num = day - 1;
			if (num >= 0 && num < dayTabItems.Count)
			{
				dayTabItems[num].SetActive(border, text);
				rewardsScrollbar.value = num / dayTabItems.Count;
				scrollRect.verticalNormalizedPosition = num / dayTabItems.Count;
			}
		}

		public void ShowClaimed()
		{
			allRewardsTab.SetActive(value: false);
			claimedRewardsTab.SetActive(value: true);
		}

		public void ShowAnimation()
		{
			animator.SetTrigger("Claim");
		}

		private void InitButtons()
		{
			claimButton.onClick.RemoveAllListeners();
			claimButton.onClick.AddListener(delegate
			{
				onClaim();
			});
			exitButton.onClick.RemoveAllListeners();
			exitButton.onClick.AddListener(delegate
			{
				onExit();
			});
		}

		private void ClearAllRewardsTab()
		{
			for (int i = 0; i < dayTabItems.Count; i++)
			{
				UnityEngine.Object.Destroy(dayTabItems[i]);
			}
			allRewardsTab.SetActive(value: false);
		}

		private void ClearClaimedRewardTab()
		{
			for (int i = 0; i < claimedTabItems.Count; i++)
			{
				UnityEngine.Object.Destroy(claimedTabItems[i]);
			}
			claimedRewardsTab.SetActive(value: false);
		}
	}
}
