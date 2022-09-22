// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingUpgradesState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Cooking;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingUpgradesState : XCraftUIState<CookingUpgradesStateConnector>
	{
		public Sprite productsIcon;

		public Sprite devicesIcon;

		public Sprite furnitureIcon;

		private WorkController workController;

		private List<Device> devices;

		private Device.Category currentCategory;

		private int currentChosen;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			workController = Manager.Get<CookingManager>().workController;
			base.connector.onDevicesButton = OnDeviceCategory;
			base.connector.onFoodButton = OnFoodCategory;
			base.connector.onFurnitureButton = OnFurnitureCategory;
			base.connector.onUpgradeButton = OnUpgrade;
			base.connector.onItemClicked = OnDeviceItem;
			base.connector.onReturnButton = OnReturn;
			base.connector.onUnlockButton = OnUnlock;
			Manager.Get<CookingManager>().ShowTopBar();
			currentCategory = Device.Category.DEVICE;
			InitDevicesList();
			InitCategoriesIcons();
			OnDeviceItem(0);
			ShowCategoryExclamationMarksIfSomethingNew();
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			Manager.Get<CookingManager>().ShowTopBar();
		}

		private void InitCategoriesIcons()
		{
			if (productsIcon != null)
			{
				base.connector.foodButton.transform.GetChild(0).GetComponent<Image>().sprite = productsIcon;
			}
			if (devicesIcon != null)
			{
				base.connector.devicesButton.transform.GetChild(0).GetComponent<Image>().sprite = devicesIcon;
			}
			if (furnitureIcon != null)
			{
				base.connector.furnitureButton.transform.GetChild(0).GetComponent<Image>().sprite = furnitureIcon;
			}
		}

		private void InitDevicesList()
		{
			base.connector.ClearList();
			UpdateButtonsSortingOrder();
			devices = Manager.Get<CookingManager>().workController.workPlace.devices;
			devices = (from d in devices
				where d.category == currentCategory
				select d).ToList();
			devices.Sort((Device linda, Device harold) => linda.GetPrestigeRequired() - harold.GetPrestigeRequired());
			for (int i = 0; i < devices.Count; i++)
			{
				int index = i;
				Device device = devices[i];
				int num = device.GetUpgradeLevel() + 1;
				string currentLevel = (num > device.MaxLevel) ? Manager.Get<TranslationsManager>().GetText("cooking.max", "max").ToUpper() : num.ToString();
				GameObject item = base.connector.AddItem(index, device.TranslatedName, currentLevel, !device.Unlocked());
				if (!device.Unlocked() && workController.workData.prestigeLevel >= device.GetPrestigeRequired())
				{
					ShowExclamationMark(device, item);
				}
			}
		}

		private void ShowExclamationMark(Device device, GameObject item)
		{
			GameObject x = ExclamationMarkController.ShowExclamationMark("cooking.upgrade.item." + device.GetKey(), item);
			if (x != null)
			{
				ShowExclamationMarkOnCategoryButton(device.category, "cooking.upgrade.category." + device.GetKey());
			}
		}

		private void ShowCategoryExclamationMarksIfSomethingNew()
		{
			List<Device> list = (from d in workController.workPlace.devices
				where d.category != currentCategory
				select d).ToList();
			foreach (Device item in list)
			{
				if (!item.Unlocked() && workController.workData.prestigeLevel >= item.GetPrestigeRequired())
				{
					ShowExclamationMarkOnCategoryButton(item.category, "cooking.upgrade.category." + item.GetKey());
				}
			}
		}

		private void ShowExclamationMarkOnCategoryButton(Device.Category category, string key)
		{
			GameObject gameObject = GetCategoryButton(category).gameObject;
			GameObject gameObject2 = ExclamationMarkController.ShowExclamationMark(key, gameObject);
			if (!(gameObject2 == null))
			{
				Canvas canvas = gameObject2.GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = gameObject2.AddComponent<Canvas>();
				}
				canvas.overrideSorting = true;
				canvas.sortingOrder = 5;
			}
		}

		private void UpdateButtonsSortingOrder()
		{
			base.connector.devicesButton.GetComponent<Canvas>().sortingOrder = 1;
			base.connector.foodButton.GetComponent<Canvas>().sortingOrder = 1;
			base.connector.furnitureButton.GetComponent<Canvas>().sortingOrder = 1;
			Button categoryButton = GetCategoryButton(currentCategory);
			categoryButton.GetComponent<Canvas>().sortingOrder = 3;
		}

		private Button GetCategoryButton(Device.Category category)
		{
			switch (category)
			{
			case Device.Category.DEVICE:
				return base.connector.devicesButton;
			case Device.Category.FOOD:
				return base.connector.foodButton;
			case Device.Category.FURNITURE:
				return base.connector.furnitureButton;
			default:
				return null;
			}
		}

		private void OnDeviceItem(int i)
		{
			Device device = devices[i];
			currentChosen = i;
			List<CookingUpgradesStateConnector.UpgradeConfig> list = new List<CookingUpgradesStateConnector.UpgradeConfig>();
			Device.UpgradeEffect[] upgradeEffects = device.upgradeEffects;
			for (int j = 0; j < upgradeEffects.Length; j++)
			{
				Device.UpgradeEffect upgradeEffect = upgradeEffects[j];
				CookingUpgradesStateConnector.UpgradeConfig upgradeConfig = new CookingUpgradesStateConnector.UpgradeConfig();
				string text = Misc.CustomNameToKey(upgradeEffect.ToString());
				string key = "cooking.upgrade." + text;
				upgradeConfig.name = Manager.Get<TranslationsManager>().GetText(key, upgradeEffect.ToString()).ToUpper();
				float upgradeValue = Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(device.Key, text, device.GetUpgradeLevel());
				float upgradeValue2 = Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(device.Key, text, device.GetUpgradeLevel() + 1);
				float num = upgradeValue2 - upgradeValue;
				string str = (!(num > 0f)) ? string.Empty : "+";
				upgradeConfig.value = upgradeValue.ToString("f2") + " " + str + num.ToString("f2");
				float upgradeValue3 = Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(device.Key, text, 0);
				float upgradeValue4 = Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(device.Key, text, device.MaxLevel);
				float num2 = upgradeConfig.progress = (upgradeValue - upgradeValue3) / (upgradeValue4 - upgradeValue3);
				list.Add(upgradeConfig);
			}
			bool flag = workController.workData.prestigeLevel >= device.GetPrestigeRequired();
			int price = GetPrice(device);
			base.connector.SetHighlight(i);
			base.connector.SetDeviceItem(device.TranslatedName, device.sprite, list, price, flag, !device.Unlocked(), device.GetPrestigeRequired());
			base.connector.SetUpgradeButtonInteractable(flag && workController.workData.money >= price);
			base.connector.upgradeButton.gameObject.SetActive(device.GetUpgradeLevel() <= device.MaxLevel);
			ExclamationMarkController.ClearMarksWithKey("cooking.upgrade.item." + device.GetKey());
			ExclamationMarkController.ClearMarksWithKey("cooking.upgrade.category." + device.GetKey());
		}

		private int GetPrice(Device device)
		{
			if (device.Unlocked())
			{
				return device.GetUpgradePrice();
			}
			if (device.GetUnlockPrice() > 0)
			{
				return device.GetUnlockPrice();
			}
			return Manager.Get<ModelManager>().cookingSettings.GetUpgradePrice(device.GetKey(), device.GetPrestigeRequired());
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (Manager.Get<CookingManager>().IsProgressDirty)
			{
				OnDeviceItem(currentChosen);
			}
		}

		private void OnUpgrade()
		{
			Device device = devices[currentChosen];
			workController.workData.money -= device.GetUpgradePrice();
			device.IncrementUpgradeLevel();
			InitDevicesList();
			OnDeviceItem(currentChosen);
			Manager.Get<StatsManager>().CookingUpgradeBought();
		}

		private void OnUnlock()
		{
			Device device = devices[currentChosen];
			workController.workData.money -= device.GetUnlockPrice();
			device.Unlock();
			workController.workPlace.InitDevices();
			InitDevicesList();
			OnDeviceItem(currentChosen);
		}

		private void OnDeviceCategory()
		{
			OnCategory(Device.Category.DEVICE);
		}

		private void OnFurnitureCategory()
		{
			OnCategory(Device.Category.FURNITURE);
		}

		private void OnFoodCategory()
		{
			OnCategory(Device.Category.FOOD);
		}

		private void OnCategory(Device.Category category)
		{
			if (currentCategory != category)
			{
				currentCategory = category;
				InitDevicesList();
				OnDeviceItem(0);
				ClearExclamationMarks(category);
			}
		}

		private void ClearExclamationMarks(Device.Category category)
		{
			List<Device> list = (from d in workController.workPlace.devices
				where d.category == category
				select d).ToList();
			foreach (Device item in list)
			{
				ExclamationMarkController.ClearMarksWithKey("cooking.upgrade.category." + item.GetKey());
			}
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
