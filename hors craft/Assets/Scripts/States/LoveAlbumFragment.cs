// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoveAlbumFragment
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class LoveAlbumFragment : Fragment
	{
		[Serializable]
		public class PhotoData
		{
			public GameObject enabledGO;

			public GameObject disabledGO;

			public Button button;

			public string cutscene;

			public PhotoRewardData rewardData;
		}

		[Serializable]
		public class PhotoRewardData
		{
			public Button rewardButton;

			public bool hasReward;

			public int rewardAmount;

			public TranslateText rewardText;
		}

		public List<PhotoData> photoSlots = new List<PhotoData>();

		[SerializeField]
		private PhotoRewardText photoRewardTextPrefab;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			UpdateSlots();
		}

		private void UpdateSlots()
		{
			int num = 1;
			foreach (PhotoData photoSlot in photoSlots)
			{
				bool flag = PlayerPrefs.GetString(photoSlot.cutscene + ".watched", "false") == "true";
				photoSlot.enabledGO.SetActive(flag);
				photoSlot.disabledGO.SetActive(!flag);
				photoSlot.button.onClick.RemoveAllListeners();
				PhotoData photoRef = photoSlot;
				photoSlot.button.onClick.AddListener(delegate
				{
					OnButtonPressed(photoRef);
				});
				InitReward(photoSlot, num);
				num++;
			}
		}

		private void InitReward(PhotoData photo, int index)
		{
			ModelManager modelManager = Manager.Get<ModelManager>();
			if (photo.rewardData.hasReward)
			{
				bool flag = PlayerPrefs.GetString(photo.cutscene + ".taken", "false") == "true";
				if (!(PlayerPrefs.GetString(photo.cutscene + ".watched", "false") == "true") || flag)
				{
					photo.rewardData.rewardButton.gameObject.SetActive(value: false);
					return;
				}
				photo.rewardData.rewardButton.gameObject.SetActive(value: true);
				int rewardAmount = modelManager.loveSettings.GetPhotoReward(index);
				photo.rewardData.rewardAmount = rewardAmount;
				photo.rewardData.rewardText.AddVisitor((string s) => s.Formatted(rewardAmount));
				photo.rewardData.rewardText.ForceRefresh();
				photo.rewardData.rewardButton.onClick.RemoveAllListeners();
				photo.rewardData.rewardButton.onClick.AddListener(delegate
				{
					TakeReward(photo);
				});
			}
		}

		private void TakeReward(PhotoData photoData)
		{
			PlayerPrefs.SetString(photoData.cutscene + ".taken", "true");
			Manager.Get<CraftSoftCurrencyManager>().OnCurrencyAmountChange(photoData.rewardData.rewardAmount);
			UpdateSlots();
			InitClaimAnimation(photoData);
		}

		private void InitClaimAnimation(PhotoData data)
		{
			if (!(photoRewardTextPrefab == null))
			{
				PhotoRewardText photoRewardText = UnityEngine.Object.Instantiate(photoRewardTextPrefab);
				Text component = photoRewardText.GetComponent<Text>();
				component.text = $"+{data.rewardData.rewardAmount.ToString()}";
				photoRewardText.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform);
				photoRewardText.transform.position = data.button.transform.position;
			}
		}

		private void OnButtonPressed(PhotoData photoData)
		{
			Manager.Get<SimpleCutsceneManager>().ShowCutscene(photoData.cutscene);
		}
	}
}
