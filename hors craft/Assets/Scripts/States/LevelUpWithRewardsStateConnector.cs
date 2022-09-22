// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LevelUpWithRewardsStateConnector
using Common.Managers.States.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class LevelUpWithRewardsStateConnector : UIConnector
	{
		public GameObject contentItemPrefab;

		public Transform contentTransform;

		public Button okButton;

		public Action onOk;

		private WaitForEndOfFrame wait = new WaitForEndOfFrame();

		public void Init(List<LevelUpRewardItemData> rewards)
		{
			InitButtons();
			if (rewards != null)
			{
				StartCoroutine(SpawnRewards(rewards));
			}
		}

		private IEnumerator SpawnRewards(List<LevelUpRewardItemData> rewards)
		{
			foreach (LevelUpRewardItemData rewardData in rewards)
			{
				yield return wait;
				GameObject instance = UnityEngine.Object.Instantiate(contentItemPrefab, contentTransform);
				LevelupRewardItem item = instance.GetComponent<LevelupRewardItem>();
				if (item == null)
				{
					UnityEngine.Object.Destroy(instance);
				}
				else
				{
					item.SetStuff(rewardData);
				}
			}
		}

		private void InitButtons()
		{
			okButton.onClick.AddListener(delegate
			{
				onOk();
			});
		}
	}
}
