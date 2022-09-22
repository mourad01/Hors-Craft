// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlocksFragment
using Common.Managers;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class BlocksFragment : Fragment
	{
		[Serializable]
		public class BlocksCategoryTabController
		{
			public Voxel.Category category;

			public Image tabLine;

			public Button button;

			[HideInInspector]
			public int blocksCounter;
		}

		[Serializable]
		public class BlocksRarityCategoryTabController
		{
			public Voxel.RarityCategory category;

			public GameObject activeTab;

			public Button button;

			public Text notificationText;
		}

		private BlocksRarityCategoryTabController[] tabRarityControllers;

		private BlocksCategoryTabController[] _tabControllers;

		public GameObject scrollListContentView;

		public ScrollRect scrollRect;

		public Button unlockBlocksButton;

		public Button unlockRarityBlocksButton;

		public Button unlockCategoryButton;

		public GameObject blockButtonPrefab;

		public GameObject videoBlockButtonPrefab;

		public GameObject block3dButtonPrefab;

		public GameObject bottomBarStandard;

		public GameObject bottomBarCurrency;

		public GameObject bottomBarRarity;

		public RectTransform rectTransform;

		public GridLayoutGroup gridLayout;

		[HideInInspector]
		public bool scrollPosDirty;

		[HideInInspector]
		public float nextScrollPosition;

		[HideInInspector]
		public int currentTabIndex = 1;

		private bool categoryBasedAds;

		private BlocksController blocksController;

		private float timeToSpawn = 0.05f;

		private int countToSpawn = 5;

		public BlocksCategoryTabController[] tabControllers
		{
			get
			{
				if (_tabControllers == null)
				{
					if (Manager.Get<ModelManager>().blocksUnlocking.IsUnlockedByCurrency())
					{
						_tabControllers = bottomBarCurrency.GetComponent<TabContainer>().tabs;
					}
					else
					{
						_tabControllers = bottomBarStandard.GetComponent<TabContainer>().tabs;
					}
				}
				return _tabControllers;
			}
		}

		private Voxel.Category currentCategory => (Voxel.Category)currentTabIndex;

		private Voxel.RarityCategory currentRarityCategory => (Voxel.RarityCategory)currentTabIndex;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			blocksController = Singleton<BlocksController>.get;
			if (blocksController.enableRarityBlocks)
			{
				blocksController.InitStartingBlocks();
				tabRarityControllers = bottomBarRarity.GetComponent<TabRarityContainer>().tabs;
			}
			InitBottomTabs();
			InitBlockCategories();
			if (Manager.Get<ModelManager>().modulesContext.isAdsFree)
			{
				unlockBlocksButton.gameObject.SetActive(value: false);
				if (blocksController.enableRarityBlocks)
				{
					unlockRarityBlocksButton.gameObject.SetActive(value: false);
				}
			}
			unlockBlocksButton.onClick.AddListener(OnUnlockButton);
			if (blocksController.enableRarityBlocks)
			{
				unlockRarityBlocksButton.onClick.AddListener(OnUnlockButton);
			}
			unlockCategoryButton.onClick.AddListener(OnUnlockButton);
		}

		private void InitBottomTabs()
		{
			if (blocksController.enableRarityBlocks)
			{
				bottomBarRarity.SetActive(value: true);
				bottomBarCurrency.SetActive(value: false);
				bottomBarStandard.SetActive(value: false);
			}
			else
			{
				bool flag = Manager.Get<ModelManager>().blocksUnlocking.IsUnlockedByCurrency();
				bottomBarCurrency.SetActive(flag);
				bottomBarStandard.SetActive(!flag);
			}
		}

		private void OnUnlockButton()
		{
			if (Manager.Get<ModelManager>().blocksUnlocking.GetUnlockType() == ItemsUnlockModel.Ads)
			{
				OnUnlock(0);
			}
			else
			{
				OnCategoryUnlock(currentCategory);
			}
		}

		private void OnCategoryUnlock(Voxel.Category category)
		{
			int cost = blocksController.CalculateCostOfCategoryUnlock(category, currentCategory);
			Manager.Get<AbstractSoftCurrencyManager>().TryToBuySomething(cost, delegate
			{
				List<Voxel> voxelsForCategory = blocksController.GetVoxelsForCategory(category);
				foreach (Voxel item in voxelsForCategory)
				{
					Singleton<PlayerData>.get.playerItems.OnBlockUnlock(item.GetUniqueID(), save: false);
				}
				Singleton<PlayerData>.get.playerItems.SaveToPrefs();
				EnableUnlockBlock(enabled: true);
				UpdateBlocksList();
				Manager.Get<StatsManager>().BlockCategoryUnlock(VoxelCategoryToUnlockProduct(category), Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount(), cost);
			});
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			if (Manager.Get<ModelManager>().modulesContext.isAdsFree)
			{
				unlockBlocksButton.gameObject.SetActive(value: false);
				if (blocksController.enableRarityBlocks)
				{
					unlockRarityBlocksButton.gameObject.SetActive(value: false);
				}
			}
			UpdateBlocksList();
			CheckUnlockAllButton();
			if (blocksController.enableRarityBlocks)
			{
				UpdateCellSize();
				CountUnseenBlocks();
			}
		}

		private void OnGUI()
		{
			if (scrollPosDirty)
			{
				scrollPosDirty = false;
				scrollRect.verticalNormalizedPosition = nextScrollPosition;
			}
		}

		private void InitBlockCategories()
		{
			if (blocksController.enableRarityBlocks)
			{
				for (int i = 0; i < tabRarityControllers.Length; i++)
				{
					tabRarityControllers[i].button.gameObject.SetActive(value: true);
					InitCategoryButtonListener(i);
				}
				currentTabIndex = PlayerPrefs.GetInt("lastOpenedRarityBlocksCategory", 0);
				CountUnseenBlocks();
				UpdateCellSize();
			}
			else
			{
				int num = -1;
				for (int j = 0; j < tabControllers.Length; j++)
				{
					if (IsValidCategory(tabControllers[j].category))
					{
						if (tabControllers[j].button.transform != null)
						{
							tabControllers[j].button.gameObject.SetActive(value: false);
						}
						continue;
					}
					tabControllers[j].button.gameObject.SetActive(value: true);
					InitCategoryButtonListener(j);
					if (num < 0)
					{
						num = j;
					}
				}
				currentTabIndex = PlayerPrefs.GetInt("lastOpenedBlocksCategory", 1);
			}
			DisableTabs();
			TurnOnTab(currentTabIndex);
			FillBlockList();
			UpdateBlocksList();
			UpdateUnlockBlocksButton();
			if (blocksController.enableRarityBlocks)
			{
				LoadScrollPosForBlocks(currentRarityCategory);
			}
			else
			{
				LoadScrollPosForBlocks(currentCategory);
			}
		}

		private bool IsValidCategory(Voxel.Category category)
		{
			return !startParameter.pauseState.allowedCategoriesFiltered.Contains(category) || Singleton<BlocksController>.get.GetVoxelsForCategory(category).Count == 0;
		}

		private void InitCategoryButtonListener(int i)
		{
			if (blocksController.enableRarityBlocks)
			{
				tabRarityControllers[i].button.onClick.AddListener(delegate
				{
					if (currentTabIndex != i)
					{
						ChangeTab(currentTabIndex, i);
						currentTabIndex = i;
						UpdateBlocksList();
						LoadScrollPosForBlocks(tabRarityControllers[i].category);
						PlayerPrefs.SetInt("lastOpenedRarityBlocksCategory", i);
						CheckUnlockAllButton();
						UpdateCellSize();
					}
				});
			}
			else
			{
				tabControllers[i].button.onClick.AddListener(delegate
				{
					if (currentTabIndex != i)
					{
						ChangeTab(currentTabIndex, i);
						currentTabIndex = i;
						UpdateBlocksList();
						LoadScrollPosForBlocks(tabControllers[i].category);
						PlayerPrefs.SetInt("lastOpenedBlocksCategory", i);
						CheckUnlockAllButton();
					}
				});
			}
		}

		private void CheckUnlockAllButton()
		{
			if (blocksController.enableRarityBlocksNoAds)
			{
				EnableUnlockBlock(enabled: false);
				return;
			}
			List<Voxel> list = (!blocksController.enableRarityBlocks) ? blocksController.GetVoxelsForCategory(currentCategory) : blocksController.GetVoxelsForRarityCategory(currentRarityCategory);
			Voxel x = list.Find((Voxel voxel) => !Singleton<PlayerData>.get.playerItems.IsBlockUnlocked(voxel.GetUniqueID()));
			bool enabled = x != null;
			EnableUnlockBlock(enabled);
		}

		private void DisableTabs()
		{
			if (blocksController.enableRarityBlocks)
			{
				for (int i = 0; i < tabRarityControllers.Length; i++)
				{
					TurnOffTab(i);
				}
			}
			else
			{
				foreach (Voxel.Category item in startParameter.pauseState.allowedCategoriesFiltered)
				{
					TurnOffTab((int)item);
				}
			}
		}

		private void TurnOffTab(int index)
		{
			if (blocksController.enableRarityBlocks)
			{
				tabRarityControllers[index].activeTab.SetActive(value: false);
			}
			else
			{
				tabControllers[index].tabLine.gameObject.SetActive(value: true);
			}
		}

		private void TurnOnTab(int index)
		{
			if (blocksController.enableRarityBlocks)
			{
				tabRarityControllers[index].activeTab.SetActive(value: true);
			}
			else
			{
				tabControllers[index].tabLine.gameObject.SetActive(value: false);
			}
		}

		private void ChangeTab(int lastIndex, int index)
		{
			TurnOffTab(lastIndex);
			TurnOnTab(index);
		}

		private void UpdateBlocksList()
		{
			blocksController.CheckRemoveAds();
			IEnumerator enumerator = scrollListContentView.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform voxelChild = (Transform)enumerator.Current;
					CheckVoxelChild(voxelChild);
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

		private void CheckVoxelChild(Transform voxelChild)
		{
			ushort blockID = voxelChild.GetComponent<ChooseBlockButtonController>().blockID;
			CraftItem component = voxelChild.GetComponent<CraftItem>();
			Voxel voxel = Engine.Blocks[blockID];
			if (blocksController.enableRarityBlocks)
			{
				CheckVoxelCountAndNewStatus(voxelChild, voxel);
			}
			if (blocksController.enableRarityBlocksNoAds)
			{
				if (blocksController.IsBlocked(voxel))
				{
					component.LockObject.SetActive(value: true);
					Color color = component.mainSprite.color;
					color.a = 0.3f;
					component.mainSprite.color = color;
				}
				else
				{
					component.LockObject.SetActive(value: false);
					Color color2 = component.mainSprite.color;
					color2.a = 1f;
					component.mainSprite.color = color2;
				}
			}
			else if (Manager.Get<ModelManager>().blocksUnlocking.GetUnlockType() == ItemsUnlockModel.Ads)
			{
				CheckVoxelForAddUnlock(voxelChild, voxel);
			}
			else
			{
				CheckVoxelForCurrency(voxelChild, voxel);
			}
			voxelChild.GetComponent<CraftItem>().EnableNotification(Manager.Get<QuestManager>().IsThisBlockNotified(voxel.GetUniqueID()));
		}

		private void CheckVoxelCountAndNewStatus(Transform voxelChild, Voxel voxel)
		{
			bool blocked = false;
			UpdateBlockLockState(voxelChild.gameObject, voxel, out blocked);
			if (IsInCurrentTab(voxel, voxelChild))
			{
				voxelChild.gameObject.SetActive(value: true);
				if (!blocked)
				{
					blocksController.UnlockBlock(voxel);
					CraftItem component = voxelChild.GetComponent<CraftItem>();
					if (blocksController.enableLimitedBlocks && PlayerPrefs.GetInt("Max.blocks.Unlimited." + voxel.rarityCategory.ToString(), 0) == 0)
					{
						component.borderObject.SetActive(value: true);
						component.borderObject.GetComponentInChildren<Text>().text = blocksController.GetBlockCount(voxel).ToString();
					}
					else
					{
						component.borderObject.SetActive(value: false);
					}
					if (!blocksController.WasBlockShown(voxel))
					{
						component.newNotificationObject.GetComponent<Image>().enabled = true;
						component.newNotificationObject.GetComponent<Animator>().enabled = true;
					}
					else
					{
						component.newNotificationObject.GetComponent<Image>().enabled = false;
						component.newNotificationObject.GetComponent<Animator>().enabled = false;
					}
				}
			}
			else
			{
				voxelChild.gameObject.SetActive(value: false);
			}
			SetBlockForCrafting(voxelChild, voxel);
		}

		private void CheckVoxelForCurrency(Transform voxelChild, Voxel voxel)
		{
			bool flag = !Singleton<PlayerData>.get.playerItems.IsBlockUnlocked(voxel.GetUniqueID()) && voxel.blockCategory != Voxel.Category.craftable;
			voxelChild.gameObject.SetActive(IsInCurrentTab(voxel, voxelChild));
			int num = Manager.Get<AbstractSoftCurrencyManager>().BlockCost(voxel.GetUniqueID());
			voxelChild.GetComponent<CraftItem>().Lock(flag);
			if (flag)
			{
				voxelChild.GetComponent<CraftItem>().SetTextOnLock(num.ToString());
			}
			SetBlockForCrafting(voxelChild, voxel);
		}

		private void CheckVoxelForAddUnlock(Transform voxelChild, Voxel voxel)
		{
			if (blocksController.enableRarityBlocksNoAds)
			{
				return;
			}
			bool blocked = false;
			UpdateBlockLockState(voxelChild.gameObject, voxel, out blocked);
			if (IsInCurrentTab(voxel, voxelChild))
			{
				voxelChild.gameObject.SetActive(value: true);
				if (blocked)
				{
					CraftItem component = voxelChild.GetComponent<CraftItem>();
					component.adsObject.gameObject.SetActive(value: true);
					component.adsObject.GetComponentInChildren<Text>().text = blocksController.AdsToUnlock(voxel).ToString();
				}
			}
			else
			{
				voxelChild.gameObject.SetActive(value: false);
			}
			SetBlockForCrafting(voxelChild, voxel);
		}

		private void SetBlockForCrafting(Transform voxelChild, Voxel voxel)
		{
			if (voxel.blockCategory == Voxel.Category.craftable)
			{
				Craftable craftableFromBlock = Manager.Get<CraftingManager>().GetCraftableFromBlock(voxel.GetUniqueID());
				if (craftableFromBlock == null || craftableFromBlock.ShouldBeHidden() || currentCategory != Voxel.Category.craftable)
				{
					voxelChild.gameObject.SetActive(value: false);
					return;
				}
				voxelChild.gameObject.SetActive(value: true);
				voxelChild.GetComponent<CraftItem>().Init(craftableFromBlock.id, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableFromBlock.id), Manager.Get<CraftingManager>().GetCraftableStatus(craftableFromBlock.id), null);
			}
		}

		private void UpdateBlockLockState(GameObject blockButton, Voxel voxel, out bool blocked)
		{
			blocked = blocksController.IsBlocked(voxel);
			if (!blocked)
			{
				CraftItem component = blockButton.GetComponent<CraftItem>();
				component.adsObject.gameObject.SetActive(value: false);
			}
		}

		private bool IsInCurrentTab(Voxel voxel, Transform voxelChild)
		{
			if (blocksController.enableRarityBlocks)
			{
				return voxel.rarityCategory == (Voxel.RarityCategory)currentTabIndex;
			}
			return voxel.blockCategory == (Voxel.Category)currentTabIndex && voxelChild.GetComponent<ChooseBlockButtonController>().IsActive();
		}

		private void LoadScrollPosForBlocks(Voxel.Category category)
		{
			nextScrollPosition = PlayerPrefs.GetFloat("scrollRectY" + category.ToString().ToLower(), 1f);
			scrollPosDirty = true;
		}

		private void LoadScrollPosForBlocks(Voxel.RarityCategory category)
		{
			nextScrollPosition = PlayerPrefs.GetFloat("scrollRectY" + category.ToString().ToLower(), 1f);
			scrollPosDirty = true;
		}

		private void UpdateUnlockBlocksButton()
		{
			if (blocksController.enableRarityBlocksNoAds)
			{
				EnableUnlockBlock(enabled: false);
			}
			else if (blocksController.enableRarityBlocks)
			{
				if (blocksController.IsCategoryFullyUnlocked(currentRarityCategory))
				{
					EnableUnlockBlock(enabled: false);
				}
			}
			else if (blocksController.IsCategoryFullyUnlocked(currentCategory))
			{
				EnableUnlockBlock(enabled: false);
			}
		}

		private void FillBlockList()
		{
			if (blocksController.enableRarityBlocks)
			{
				List<Voxel.RarityCategory> rarityCategories = blocksController.GetRarityCategories();
				foreach (Voxel.RarityCategory item in rarityCategories)
				{
					Manager.Get<StateMachineManager>().StartCoroutine(CreateBlocksOfCategory(item));
				}
			}
			else
			{
				List<Voxel.Category> categories = blocksController.GetCategories();
				foreach (Voxel.Category item2 in categories)
				{
					Manager.Get<StateMachineManager>().StartCoroutine(CreateBlocksOfCategory(item2));
				}
			}
		}

		private IEnumerator CreateBlocksOfCategory(Voxel.Category category)
		{
			int currentSpawnCount = 0;
			GameObject prefab = Manager.Get<ModelManager>().blocksUnlocking.IsUnlockedByCurrency() ? blockButtonPrefab : ((!Manager.Get<ModelManager>().blocksUnlocking.Is3dBlocksViewEnabled()) ? videoBlockButtonPrefab : block3dButtonPrefab);
			int categoryLength = blocksController.GetCategryLength(category);
			for (int i = 0; i < categoryLength; i++)
			{
				float time = timeToSpawn / Time.unscaledDeltaTime;
				int limitedTimeBlocks = Mathf.FloorToInt(time * (float)countToSpawn);
				GameObject spawned = SpawnBlock(prefab, blocksController.GetVoxel(category, i));
				CheckVoxelChild(spawned.transform);
				currentSpawnCount++;
				if (currentSpawnCount > limitedTimeBlocks)
				{
					yield return new WaitForEndOfFrame();
					currentSpawnCount = 0;
					if (this == null)
					{
						break;
					}
				}
			}
		}

		private IEnumerator CreateBlocksOfCategory(Voxel.RarityCategory category)
		{
			int currentSpawnCount = 0;
			GameObject prefab = block3dButtonPrefab;
			List<Voxel> voxels = blocksController.GetVoxelsForRarityCategory(category);
			foreach (Voxel voxel in voxels)
			{
				float time = timeToSpawn / Time.unscaledDeltaTime;
				int limitedTimeBlocks = Mathf.FloorToInt(time * (float)countToSpawn);
				GameObject spawned = SpawnBlock(prefab, voxel);
				CheckVoxelChild(spawned.transform);
				currentSpawnCount++;
				if (currentSpawnCount > limitedTimeBlocks)
				{
					yield return new WaitForEndOfFrame();
					currentSpawnCount = 0;
					if (this == null)
					{
						break;
					}
				}
			}
		}

		private void EnableUnlockBlock(bool enabled)
		{
			unlockBlocksButton.transform.parent.gameObject.SetActive(enabled);
			unlockCategoryButton.transform.parent.gameObject.SetActive(enabled);
			if (blocksController.enableRarityBlocks)
			{
				unlockRarityBlocksButton.transform.parent.gameObject.SetActive(enabled);
			}
		}

		private GameObject SpawnBlock(GameObject prefab, Voxel item)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.transform.SetParent(scrollListContentView.transform, worldPositionStays: false);
			gameObject.AddComponent<ChooseBlockButtonController>();
			gameObject.GetComponent<ChooseBlockButtonController>().blockID = item.GetUniqueID();
			gameObject.GetComponent<ChooseBlockButtonController>().onButtonClicked = OnUnlock;
			gameObject.GetComponent<ChooseBlockButtonController>().onCraftableClicked = OnCraftable;
			if (item.blockCategory == Voxel.Category.craftable)
			{
				SetCraftable(gameObject, item);
			}
			SetSprite(gameObject, item);
			return gameObject;
		}

		private void SetCraftable(GameObject spawned, Voxel item)
		{
			int craftableIdFromBlock = Manager.Get<CraftingManager>().GetCraftableIdFromBlock(item.GetUniqueID());
			spawned.GetComponent<CraftItem>().Init(craftableIdFromBlock, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableIdFromBlock), Manager.Get<CraftingManager>().GetCraftableStatus(craftableIdFromBlock), null);
		}

		private void SetSprite(GameObject spawned, Voxel item)
		{
			CraftItem component = spawned.GetComponent<CraftItem>();
			GameObject gameObject = component.blockObject.transform.GetChild(0).gameObject;
			gameObject.GetComponent<Image>().sprite = VoxelSprite.GetVoxelSprite(item);
			if (item.blockCategory != Voxel.Category.basic && (item.useCustomSprite || item.isThatFlower || item.GetComponent<VoxelDoorOpenClose>() != null))
			{
				gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(12f, 12f);
				gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-12f, -12f);
			}
			else
			{
				gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
				gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
			}
			if (item.GetUniqueID() == ExampleInventory.HeldBlock && !blocksController.BlocksUnlockingModule.Is3dBlocksViewEnabled())
			{
				Image component2 = spawned.GetComponent<Image>();
				Color color = component2.color;
				color.a = 255f;
				component2.color = color;
				Button component3 = spawned.GetComponent<Button>();
				ColorBlock colors = component3.colors;
				colors.normalColor = color;
				component3.colors = colors;
			}
		}

		private bool OnUnlock(ushort clickedId)
		{
			if (Manager.Get<ModelManager>().blocksUnlocking.GetUnlockType() == ItemsUnlockModel.Ads)
			{
				return OnUnlockAds(clickedId);
			}
			return OnUnlockCurrency(clickedId);
		}

		private bool OnUnlockAds(ushort clickedId)
		{
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				voxelSpritesToUnlock = blocksController.VoxelsSpritesToNextUnlock,
				type = AdsCounters.Blocks,
				blockCategory = (Voxel.Category)currentTabIndex,
				reason = ((!categoryBasedAds) ? StatsManager.AdReason.XCRAFT_BLOCKS_ALL : VoxelCategoryToAdReason(currentCategory))
			});
			return false;
		}

		private bool OnUnlockCurrency(ushort clickedId)
		{
			int cost = Manager.Get<AbstractSoftCurrencyManager>().BlockCost(clickedId);
			Voxel.Category category = Engine.GetVoxelType(clickedId).blockCategory;
			Manager.Get<AbstractSoftCurrencyManager>().TryToBuySomething(cost, delegate
			{
				Singleton<PlayerData>.get.playerItems.OnBlockUnlock(clickedId);
				Manager.Get<StatsManager>().BlockBought(clickedId, VoxelCategoryToBlockProduct(category), Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount(), cost);
			});
			return true;
		}

		private void OnCraftable(ushort blockId)
		{
			startParameter.pauseState.ChangeForCrafting(blockId);
		}

		private StatsManager.AdReason VoxelCategoryToAdReason(Voxel.Category category)
		{
			switch (category)
			{
			default:
				return StatsManager.AdReason.XCRAFT_BLOCKS_BASIC;
			case Voxel.Category.organic:
				return StatsManager.AdReason.XCRAFT_BLOCKS_ORGANIC;
			case Voxel.Category.custom:
				return StatsManager.AdReason.XCRAFT_BLOCKS_CUSTOM;
			case Voxel.Category.furniture:
				return StatsManager.AdReason.XCRAFT_BLOCKS_FURNITURE;
			}
		}

		private StatsManager.BoughtProduct VoxelCategoryToBlockProduct(Voxel.Category category)
		{
			switch (category)
			{
			case Voxel.Category.basic:
				return StatsManager.BoughtProduct.BASIC_BLOCK;
			case Voxel.Category.organic:
				return StatsManager.BoughtProduct.ORGANIC_BLOCK;
			case Voxel.Category.custom:
				return StatsManager.BoughtProduct.CUSTOM_BLOCK;
			case Voxel.Category.furniture:
				return StatsManager.BoughtProduct.FURNITURE_BLOCK;
			default:
				return StatsManager.BoughtProduct.INVALID;
			}
		}

		private StatsManager.BoughtProduct VoxelCategoryToUnlockProduct(Voxel.Category category)
		{
			return VoxelCategoryToBlockProduct(category) + 4;
		}

		private void UpdateCellSize()
		{
			gridLayout.constraintCount = Manager.Get<ModelManager>().blocksUnlocking.GetRarityBlocksViewColumnsCount(currentRarityCategory);
			float width = rectTransform.rect.width;
			float num = gridLayout.constraintCount - 1;
			Vector2 spacing = gridLayout.spacing;
			float num2 = width - num * spacing.x;
			gridLayout.cellSize = new Vector2(num2 / (float)gridLayout.constraintCount, num2 / (float)gridLayout.constraintCount);
		}

		private void CountUnseenBlocks()
		{
			for (int i = 0; i < tabRarityControllers.Length; i++)
			{
				int num = CountUnseenBlocksForCategory(tabRarityControllers[i].category);
				if (num > 0)
				{
					tabRarityControllers[i].notificationText.text = num.ToString();
					tabRarityControllers[i].notificationText.transform.parent.gameObject.SetActive(value: true);
				}
			}
		}

		private int CountUnseenBlocksForCategory(Voxel.RarityCategory category)
		{
			List<Voxel> voxelsForRarityCategory = blocksController.GetVoxelsForRarityCategory(category);
			int num = 0;
			foreach (Voxel item in voxelsForRarityCategory)
			{
				if (!blocksController.IsBlocked(item) && !blocksController.WasBlockShown(item))
				{
					num++;
				}
			}
			return num;
		}
	}
}
