// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingUpgradesStateConnector
using Common.Managers.States.UI;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingUpgradesStateConnector : UIConnector
	{
		[Serializable]
		public class DeviceUpgradesItem
		{
			public GameObject upgradeBarPrefab;

			public Transform upgradeBarsParent;

			public Text deviceName;

			public Image deviceImage;

			public GameObject[] unlockedGO;

			public GameObject[] lockedGO;
		}

		public class UpgradeConfig
		{
			public string name;

			public string value;

			public float progress;
		}

		public GameObject deviceItemPrefab;

		public Transform deviceItemParent;

		public DeviceUpgradesItem deviceUpgradeItem;

		public Button devicesButton;

		public Button foodButton;

		public Button furnitureButton;

		public Button upgradeButton;

		public Button returnButton;

		public Text upgradeCostText;

		public TranslateText unlocksOnPrestigeText;

		public Action<int> onItemClicked;

		public Action onDevicesButton;

		public Action onFoodButton;

		public Action onFurnitureButton;

		public Action onUpgradeButton;

		public Action onUnlockButton;

		public Action onReturnButton;

		private Dictionary<int, DeviceListItem> deviceListItems = new Dictionary<int, DeviceListItem>();

		private void Awake()
		{
			devicesButton.onClick.AddListener(delegate
			{
				onDevicesButton();
			});
			foodButton.onClick.AddListener(delegate
			{
				onFoodButton();
			});
			furnitureButton.onClick.AddListener(delegate
			{
				onFurnitureButton();
			});
			upgradeButton.onClick.AddListener(delegate
			{
				onUpgradeButton();
			});
			returnButton.onClick.AddListener(delegate
			{
				onReturnButton();
			});
		}

		public void ClearList()
		{
			while (deviceItemParent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(deviceItemParent.GetChild(0).gameObject);
			}
			deviceListItems = new Dictionary<int, DeviceListItem>();
		}

		public GameObject AddItem(int index, string name, string currentLevel, bool locked)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(deviceItemPrefab, deviceItemParent, worldPositionStays: false);
			DeviceListItem component = gameObject.GetComponent<DeviceListItem>();
			deviceListItems[index] = component;
			component.Init(name, currentLevel, locked);
			component.button.onClick.AddListener(delegate
			{
				onItemClicked(index);
			});
			return gameObject;
		}

		public void SetHighlight(int index)
		{
			foreach (DeviceListItem value in deviceListItems.Values)
			{
				value.SetHighlight(highlight: false);
			}
			deviceListItems[index].SetHighlight(highlight: true);
		}

		public void SetDeviceItem(string name, Sprite sprite, List<UpgradeConfig> upgrades, int price, bool enoughPrestige, bool locked, int requiredPrestige)
		{
			deviceUpgradeItem.deviceName.text = name;
			deviceUpgradeItem.deviceImage.sprite = sprite;
			upgradeCostText.text = price.ToString();
			while (deviceUpgradeItem.upgradeBarsParent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(deviceUpgradeItem.upgradeBarsParent.GetChild(0).gameObject);
			}
			foreach (UpgradeConfig upgrade in upgrades)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(deviceUpgradeItem.upgradeBarPrefab, deviceUpgradeItem.upgradeBarsParent, worldPositionStays: false);
				UpgradeBar component = gameObject.GetComponent<UpgradeBar>();
				component.Init(upgrade.name, upgrade.value, upgrade.progress);
			}
			TranslateText componentInChildren = upgradeButton.GetComponentInChildren<TranslateText>();
			if (locked)
			{
				componentInChildren.translationKey = "cooking.unlock";
				componentInChildren.defaultText = "buy";
			}
			else
			{
				componentInChildren.translationKey = "cooking.upgrade";
				componentInChildren.defaultText = "upgrade";
			}
			componentInChildren.ForceRefresh();
			if (enoughPrestige)
			{
				if (locked)
				{
					upgradeButton.onClick.RemoveAllListeners();
					upgradeButton.onClick.AddListener(delegate
					{
						onUnlockButton();
					});
				}
				else
				{
					upgradeButton.onClick.RemoveAllListeners();
					upgradeButton.onClick.AddListener(delegate
					{
						onUpgradeButton();
					});
				}
			}
			if (!enoughPrestige || locked)
			{
				unlocksOnPrestigeText.ClearVisitors();
				unlocksOnPrestigeText.AddVisitor((string t) => t.Replace("{0}", requiredPrestige.ToString()));
			}
			GameObject[] lockedGO = deviceUpgradeItem.lockedGO;
			foreach (GameObject gameObject2 in lockedGO)
			{
				gameObject2.SetActive(locked);
			}
			GameObject[] unlockedGO = deviceUpgradeItem.unlockedGO;
			foreach (GameObject gameObject3 in unlockedGO)
			{
				gameObject3.SetActive(!locked);
			}
		}

		public void SetUpgradeButtonInteractable(bool interactable)
		{
			if (interactable)
			{
				upgradeButton.interactable = true;
				upgradeButton.GetComponent<ColorController>().category = ColorManager.ColorCategory.HIGHLIGHT_COLOR;
				upgradeButton.GetComponent<ColorController>().UpdateColor();
			}
			else
			{
				upgradeButton.interactable = false;
				upgradeButton.GetComponent<ColorController>().category = ColorManager.ColorCategory.DISABLED_COLOR;
				upgradeButton.GetComponent<ColorController>().UpdateColor();
			}
		}
	}
}
