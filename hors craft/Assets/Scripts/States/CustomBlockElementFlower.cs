// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomBlockElementFlower
using Common.Managers;
using Gameplay;
using System;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CustomBlockElementFlower : CustomBlockElement
	{
		public Text flowerText;

		public Button buyButton;

		public Text priceText;

		public Image flowerIcon;

		public Text levelReqText;

		public GameObject[] activeWhenEnabledObjects;

		public GameObject[] activeWhenDisabledObjects;

		public override void Init(Voxel voxel)
		{
			flowerIcon.sprite = voxel.voxelSprite;
			buyButton.onClick.AddListener(delegate
			{
				OnChoose(voxel);
			});
			GrowableInfoVoxel growableInfoVoxel = Manager.Get<FarmingManager>().growablesData.FirstOrDefault((GrowableInfoVoxel d) => d.initialBlockIndex == voxel.GetUniqueID());
			string settingsKey = growableInfoVoxel.settingsKey;
			InitFlowerName(settingsKey, voxel.VName);
			InitLock(settingsKey);
			InitPrice(settingsKey);
		}

		private void InitFlowerName(string key, string defaultName)
		{
			string text = Manager.Get<TranslationsManager>().GetText(key, defaultName);
			flowerText.text = text;
		}

		private void InitLock(string key)
		{
			int requiredLevel = Manager.Get<ModelManager>().progressSettings.GetRequiredLevel(key);
			levelReqText.GetComponent<TranslateText>().ClearVisitors();
			levelReqText.GetComponent<TranslateText>().AddVisitor((string t) => t.Replace("{0}", requiredLevel.ToString()));
			bool locked = Manager.Get<ProgressManager>().level < requiredLevel;
			Array.ForEach(activeWhenEnabledObjects, delegate(GameObject a)
			{
				a.SetActive(!locked);
			});
			Array.ForEach(activeWhenDisabledObjects, delegate(GameObject d)
			{
				d.SetActive(locked);
			});
		}

		private void InitPrice(string key)
		{
			int softPriceFor = Manager.Get<ModelManager>().currencySettings.GetSoftPriceFor(key);
			priceText.text = softPriceFor.ToString();
		}

		private void OnChoose(Voxel voxel)
		{
			ExampleInventory.HeldBlock = voxel.GetUniqueID();
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
