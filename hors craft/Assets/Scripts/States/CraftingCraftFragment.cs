// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingCraftFragment
using Common.Managers;
using Gameplay;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CraftingCraftFragment : Fragment
	{
		public CraftItem gigantBlock;

		public GameObject resourceElementPrefab;

		public GameObject resourcesParent;

		private GameObject[] requierments;

		public Button craftButton;

		public Button returnButton;

		public Button useButton;

		public Button upgradeButton;

		public GameObject fullyUpgradedButton;

		public Button getResourcesButton;

		public List<CraftItem> resourcesesNeeded;

		public List<CraftItem> craftableNeeded;

		private CraftingFragment.CrafttStartParameter startParam;

		public CraftableStatus currentItemStatus;

		public int currentcraftableCount;

		private Craftable craftable;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			destroyCurrentReources();
			CreateNeededResources(Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId));
			InitBlock();
			InitButtons();
		}

		private void InitBlock()
		{
			int sourceBlockId = startParam.sourceBlockId;
			currentItemStatus = Manager.Get<CraftingManager>().GetCraftableStatus(sourceBlockId);
			currentcraftableCount = Singleton<PlayerData>.get.playerItems.GetCraftableCount(sourceBlockId);
			gigantBlock.Init(sourceBlockId, currentcraftableCount, currentItemStatus, Manager.Get<CraftingManager>().GetCraftable(sourceBlockId).GetGraphic());
			craftable = Manager.Get<CraftingManager>().GetCraftable(sourceBlockId);
		}

		private void InitButtons()
		{
			craftButton.onClick.RemoveAllListeners();
			returnButton.onClick.RemoveAllListeners();
			useButton.onClick.RemoveAllListeners();
			upgradeButton.onClick.RemoveAllListeners();
			getResourcesButton.onClick.RemoveAllListeners();
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId);
			if (craftable.craftableType == Craftable.type.Upgradeable)
			{
				useButton.gameObject.SetActive(value: false);
				craftButton.gameObject.SetActive(value: false);
				if (craftable.status != CraftableStatus.FullyUpgraded)
				{
					upgradeButton.gameObject.SetActive(value: true);
					fullyUpgradedButton.SetActive(value: false);
				}
				else
				{
					upgradeButton.gameObject.SetActive(value: false);
					fullyUpgradedButton.SetActive(value: true);
				}
			}
			else
			{
				useButton.gameObject.SetActive(value: true);
				craftButton.gameObject.SetActive(value: true);
				upgradeButton.gameObject.SetActive(value: false);
				fullyUpgradedButton.SetActive(value: false);
			}
			if (startParam.parentFragment.enableResourcesForAd)
			{
				getResourcesButton.gameObject.SetActive(value: true);
			}
			else
			{
				getResourcesButton.gameObject.SetActive(value: false);
			}
			SetButtons();
			CheckButtonsState();
		}

		private void SetButtons()
		{
			useButton.onClick.AddListener(delegate
			{
				OnUseButton();
			});
			upgradeButton.onClick.AddListener(delegate
			{
				CraftingFragment parentFragment2 = startParam.parentFragment;
				OnUpgradeButton(parentFragment2.onCraft);
			});
			craftButton.onClick.AddListener(delegate
			{
				CraftingFragment parentFragment = startParam.parentFragment;
				OnCraftButton(parentFragment.onCraft);
			});
			returnButton.onClick.AddListener(delegate
			{
				OnReturn();
			});
			getResourcesButton.onClick.AddListener(delegate
			{
				OnGetResourcesButton();
			});
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			RevalidateResources();
		}

		private void OnCraftButton(Action<int> onCraftButton)
		{
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId);
			if (craftable.craftableType != Craftable.type.Weapon || CanCraftMoreWeapons(craftable))
			{
				if (craftable.craftableType == Craftable.type.Custom)
				{
					OnCraftCustomItem();
				}
				onCraftButton?.Invoke(startParam.sourceBlockId);
				InitBlock();
				CheckButtonsState();
				RevalidateResources();
				base.gameObject.SetActive(value: false);
				base.gameObject.SetActive(value: true);
			}
		}

		private void OnCraftCustomItem()
		{
			craftable.customCraftableObject.GetComponent<ICustomCraftingItem>()?.OnCraftAction();
		}

		private bool CanCraftMoreWeapons(Craftable craftable)
		{
			return CanOwnMoreThanOneWeapon(craftable) || Singleton<PlayerData>.get.playerItems.GetCraftableCount(startParam.sourceBlockId) < 1;
		}

		private bool CanOwnMoreThanOneWeapon(Craftable craftable)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			return gameObject.GetComponent<ArmedPlayer>().GetWeapon(craftable.weaponId).canOwnMoreThanOne;
		}

		private void OnUseButton()
		{
			startParam.parentFragment.OnBlockSelected(craftable);
		}

		private void OnUpgradeButton(Action<int> onUpgradeButton)
		{
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId);
			if (craftable.craftableType == Craftable.type.Upgradeable && Singleton<PlayerData>.get.playerItems.GetCraftableCount(startParam.sourceBlockId) >= 1)
			{
				return;
			}
			onUpgradeButton?.Invoke(startParam.sourceBlockId);
			Singleton<PlayerData>.get.playerItems.Craft(craftable.id);
			if (craftable.craftableType == Craftable.type.Upgradeable && craftable.status == CraftableStatus.Craftable && Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftable.id) >= 0)
			{
				Craftable craftable2 = Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId + 1);
				if (craftable2.craftableType == Craftable.type.Upgradeable && craftable2.upgradeId != craftable.upgradeId)
				{
					craftable.showInCraftableList = false;
					craftable2.showInCraftableList = true;
					startParam.parentFragment.UpdateRecipesFragment();
					if (craftable.id == 0)
					{
						Manager.Get<QuestManager>().UnlockSilverRod();
						Manager.Get<FishingManager>().SetRodConfig(1);
					}
					startParam.parentFragment.SetState(CraftingFragment.State.Requirements, craftable.upgradeId);
				}
				fullyUpgradedButton.SetActive(value: false);
				upgradeButton.gameObject.SetActive(value: true);
			}
			else if (craftable.craftableType == Craftable.type.Upgradeable && craftable.status == CraftableStatus.FullyUpgraded && Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftable.id) == 1)
			{
				Manager.Get<FishingManager>().SetRodConfig(2);
				startParam.parentFragment.SetState(CraftingFragment.State.FullyUpgraded, craftable.id);
			}
			InitBlock();
			InitButtons();
			CheckButtonsState();
			RevalidateResources();
		}

		private void OnGetResourcesButton()
		{
			startParam.parentFragment.OnGetResourcesForAds("Watch an ad for extra resources", "menu.watch.ad.for.resources", _useCraftableId: true);
		}

		private void CheckButtonsState()
		{
			craftButton.GetComponent<ColorController>().category = ((currentItemStatus != CraftableStatus.Craftable) ? ColorManager.ColorCategory.THIRD_COLOR : ColorManager.ColorCategory.MAIN_COLOR);
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId);
			if (craftable.craftableType == Craftable.type.Weapon && !CanOwnMoreThanOneWeapon(craftable) && currentcraftableCount > 0)
			{
				craftButton.GetComponent<ColorController>().category = ColorManager.ColorCategory.THIRD_COLOR;
			}
			useButton.GetComponent<ColorController>().category = ((currentcraftableCount <= 0) ? ColorManager.ColorCategory.THIRD_COLOR : ColorManager.ColorCategory.MAIN_COLOR);
			if (craftable.craftableType == Craftable.type.Weapon && !CanOwnMoreThanOneWeapon(craftable) && currentcraftableCount > 0 && !Manager.Get<SurvivalManager>().IsCombatTime())
			{
				useButton.GetComponent<ColorController>().category = ColorManager.ColorCategory.THIRD_COLOR;
			}
			craftButton.GetComponent<ColorController>().UpdateColor();
			useButton.GetComponent<ColorController>().UpdateColor();
		}

		private void CreateNeededResources(Craftable craftable)
		{
			destroyCurrentReources();
			resourcesesNeeded = new List<CraftItem>();
			craftableNeeded = new List<CraftItem>();
			for (int i = 0; i < craftable.requiredResourcesToCraft.Count; i++)
			{
				CreateElement(craftable.requiredResourcesToCraft[i].count, craftable.requiredResourcesToCraft[i].id, Manager.Get<CraftingManager>().GetResourceImage(craftable.requiredResourcesToCraft[i].id));
			}
			for (int j = 0; j < craftable.requiredCraftableToCraft.Count; j++)
			{
				CreateElementCraftable(craftable.requiredCraftableToCraft[j].count, craftable.requiredCraftableToCraft[j].id, Manager.Get<CraftingManager>().GetCraftable(craftable.requiredCraftableToCraft[j].id).GetGraphic());
			}
		}

		private void RevalidateResources()
		{
			UnityEngine.Debug.LogWarning(resourcesesNeeded.Count + " -- " + craftableNeeded.Count);
			foreach (CraftItem item in resourcesesNeeded)
			{
				int resourcesCount = Singleton<PlayerData>.get.playerItems.GetResourcesCount(item.id);
				RevalidateElement(item, resourcesCount);
			}
			foreach (CraftItem item2 in craftableNeeded)
			{
				int craftableCount = Singleton<PlayerData>.get.playerItems.GetCraftableCount(item2.id);
				RevalidateElement(item2, craftableCount);
			}
		}

		private void RevalidateElement(CraftItem item, int currentRes)
		{
			UnityEngine.Debug.LogWarning("REVALIDADED!");
			CraftableStatus status = (item.counter <= currentRes) ? CraftableStatus.Craftable : CraftableStatus.NoResources;
			string arg = ColorUtility.ToHtmlStringRGB(item.GetComponent<CraftItem>().GetColorForStatus(status));
			item.GetComponentInChildren<Text>().text = $" <color=#{arg}>{currentRes}</color>/{item.counter}";
		}

		private GameObject CreateElement(int counter, int id, Sprite sprite)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(resourceElementPrefab);
			int resourcesCount = Singleton<PlayerData>.get.playerItems.GetResourcesCount(id);
			gameObject.transform.parent = resourcesParent.transform;
			gameObject.transform.localScale = Vector3.one;
			CraftItem component = gameObject.GetComponent<CraftItem>();
			CraftableStatus status = (counter <= resourcesCount) ? CraftableStatus.Craftable : CraftableStatus.NoResources;
			component.Init(id, counter, Manager.Get<CraftingManager>().GetCraftableStatus(id), Manager.Get<CraftingManager>().GetResourceImage(id));
			string arg = ColorUtility.ToHtmlStringRGB(component.GetColorForStatus(status));
			gameObject.GetComponentInChildren<Text>().text = $" <color=#{arg}>{resourcesCount}</color>/{counter}";
			gameObject.SetActive(value: true);
			resourcesesNeeded.Add(component);
			component.EnableNotification(Manager.Get<QuestManager>().IsThisResourceNotified(id));
			return gameObject;
		}

		private GameObject CreateElementCraftable(int counter, int id, Sprite sprite)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(resourceElementPrefab);
			gameObject.transform.parent = resourcesParent.transform;
			gameObject.GetComponentInChildren<Text>().text = $"x{counter}";
			gameObject.transform.localScale = Vector3.one;
			int num = Mathf.Max(0, Singleton<PlayerData>.get.playerItems.GetCraftableCount(id));
			CraftableStatus status = (counter <= num) ? CraftableStatus.Craftable : CraftableStatus.NoResources;
			gameObject.GetComponent<CraftItem>().Init(id, counter, Manager.Get<CraftingManager>().GetCraftableStatus(id), Manager.Get<CraftingManager>().GetCraftable(id).GetGraphic());
			string arg = ColorUtility.ToHtmlStringRGB(gameObject.GetComponent<CraftItem>().GetColorForStatus(status));
			gameObject.GetComponentInChildren<Text>().text = $" <color=#{arg}>{num}</color>/{counter}";
			gameObject.SetActive(value: true);
			craftableNeeded.Add(gameObject.GetComponent<CraftItem>());
			return gameObject;
		}

		private void destroyCurrentReources()
		{
			IEnumerator enumerator = resourcesParent.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private void OnReturn()
		{
			try
			{
				startParam.parentFragment.UpdateRecipesFragment();
			}
			catch
			{
			}
			startParam.parentFragment.SetState(CraftingFragment.State.Recipes, startParam);
		}
	}
}
