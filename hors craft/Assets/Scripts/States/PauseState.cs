// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PauseState
using Common.Behaviours;
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;

namespace States
{
	public class PauseState : XCraftUIState<PauseStateConnector>
	{
		public bool enableCollections;

		public bool enableHostpitalUpgrades;

		public bool enableDressup;

		public int wardrobeMenuButtonPosition = 1;

		public bool openWardrobeFirst;

		public GameObject buttonPrefab;

		public string defaultCategoryToOpen = "Blocks";

		public List<Voxel.Category> allowedBlockCategories = new List<Voxel.Category>
		{
			Voxel.Category.basic,
			Voxel.Category.organic,
			Voxel.Category.custom,
			Voxel.Category.craftable,
			Voxel.Category.furniture
		};

		public List<Craftable.RecipeCategory> allowedCraftingCategories = new List<Craftable.RecipeCategory>
		{
			Craftable.RecipeCategory.BLUEPRINT,
			Craftable.RecipeCategory.FURNITURE,
			Craftable.RecipeCategory.OTHER
		};

		[Reorderable]
		public List<FragmentComponent> fragments;

		[HideInInspector]
		public ushort blockClicked;

		private PauseStateStartParameter startParameter;

		private string currentFragment = string.Empty;

		private float pauseTimeStamp;

		private bool packWasShown;

		public List<Voxel.Category> allowedCategoriesFiltered
		{
			get
			{
				if (!Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
				{
					allowedBlockCategories.Remove(Voxel.Category.craftable);
				}
				return allowedBlockCategories;
			}
		}

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				autoAdsConfig.properAdReason = StatsManager.AdReason.XCRAFT_PAUSE;
				return autoAdsConfig;
			}
		}

		public FragmentComponent getCurrentFragment => fragments.First((FragmentComponent frag) => frag.fragmentName == currentFragment);

		private void ResetBackgroundOverlay()
		{
			Manager.Get<CommonUIManager>().SetBackground(hasBackground, hasBackgroundOverlay);
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			if (Singleton<BlocksController>.get.enableRarityBlocks)
			{
				FragmentComponent fragmentComponent = (from f in fragments
					where f.fragmentName == "Blocks"
					select f).FirstOrDefault();
				if (fragmentComponent != null)
				{
					fragmentComponent.prefabResource = "UIPrefabs/PauseFragmentsPrefabs/BlocksNew";
				}
			}
			packWasShown = false;
			startParameter = (parameter as PauseStateStartParameter);
			currentFragment = string.Empty;
			CheckIfEnableTabs();
			CheckIfEnableShop();
			CheckIfEnableAchievements();
			CheckIfEnableBlueprints();
			CheckIfEnableCraftingAndClothes();
			CheckIfEnableServerRankings();
			base.connector.onReturnButtonClicked = OnReturn;
			if (startParameter != null)
			{
				blockClicked = startParameter.blockCaused;
			}
			foreach (FragmentComponent fragment in fragments)
			{
				AddFragment(fragment.fragmentName);
			}
			if (AutoRefreshingStock.GetStockCount("free.chest") < 1 || !(startParameter.categoryToOpen != "Shop") || !TrySwitchFragmenTo("Shop"))
			{
				if (startParameter != null)
				{
					OnFragmentButton(startParameter.categoryToOpen.IsNullOrEmpty() ? defaultCategoryToOpen : startParameter.categoryToOpen);
				}
				else
				{
					OnFragmentButton(defaultCategoryToOpen);
				}
			}
			base.connector.openWardrobeFirst = openWardrobeFirst;
			base.connector.showWardrobeButton = enableDressup;
			base.connector.wardrobeMenuButtonPosition = wardrobeMenuButtonPosition;
			base.connector.onWardrobeButtonClicked = OnWardrobe;
			base.connector.ShowWardrobe();
			ShowRateUsIfNeeded();
			InitDevMode();
			pauseTimeStamp = Time.realtimeSinceStartup;
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.WAS_IN_PAUSE, enabled: true);
		}

		private void CheckIfEnableTabs()
		{
			List<string> disabledPauseTabs = Manager.Get<ModelManager>().configSettings.GetDisabledPauseTabs();
			foreach (string item in disabledPauseTabs)
			{
				RemoveFragment(item);
			}
		}

		private void CheckIfEnableCraftingAndClothes()
		{
			if (!Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
			{
				RemoveFragment("Crafting");
			}
			if (!Manager.Get<ModelManager>().clothesSetting.GetClothesEnabled())
			{
				RemoveFragment("Customization");
			}
		}

		private void CheckIfEnableShop()
		{
			if (!Manager.Get<ModelManager>().lootSettings.IsShopEnabled())
			{
				RemoveFragment("Shop");
			}
		}

		private void CheckIfEnableAchievements()
		{
			if (!Manager.Get<AbstractModelManager>().achievementsSettings.IsAchievementsEnabled())
			{
				RemoveFragment("Achievements");
			}
		}

		private void CheckIfEnableServerRankings()
		{
			if (!Manager.Contains<RankingManager>() || !Manager.Get<ModelManager>().rankingsSettings.RankingsEnabled())
			{
				RemoveFragment("Ranking");
			}
		}

		private void CheckIfEnableBlueprints()
		{
			if (!Manager.Get<ModelManager>().craftingSettings.AreBlueprintsEnabled() && allowedCraftingCategories.Contains(Craftable.RecipeCategory.BLUEPRINT))
			{
				allowedCraftingCategories.Remove(Craftable.RecipeCategory.BLUEPRINT);
			}
		}

		private void RemoveFragment(string name)
		{
			FragmentComponent fragmentComponent = fragments.FirstOrDefault((FragmentComponent f) => f.fragmentName == name);
			if (fragmentComponent != null)
			{
				fragments.Remove(fragmentComponent);
			}
		}

		protected void AddFragment(string name)
		{
			FragmentComponent fragmentComponent = fragments.First((FragmentComponent frag) => frag.fragmentName == name);
			GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab, base.connector.MenuButtons.transform, worldPositionStays: false);
			gameObject.name = name;
			if (fragmentComponent.preferredWidth != 0)
			{
				gameObject.GetComponent<LayoutElement>().preferredWidth = fragmentComponent.preferredWidth;
			}
			TranslateText componentInChildren = gameObject.GetComponentInChildren<TranslateText>();
			componentInChildren.defaultText = fragmentComponent.defaultText;
			componentInChildren.translationKey = fragmentComponent.translationKey;
			componentInChildren.ForceRefresh();
			fragmentComponent.button = gameObject.GetComponentInChildren<Button>();
			InitFragment(fragmentComponent.button, name);
		}

		public override void UpdateState()
		{
			base.UpdateState();
			CheckOffersFromUpdate();
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			if (!(base.connector == null))
			{
				foreach (FragmentComponent fragment in fragments)
				{
					if (fragment.instance != null && fragment.instance.activeSelf)
					{
						fragment.instance.GetComponent<Fragment>().UpdateFragment();
					}
				}
			}
		}

		public void RefreshAllFragments()
		{
			foreach (FragmentComponent fragment in fragments)
			{
				if (fragment.instance != null && fragment.instance.activeSelf)
				{
					fragment.instance.GetComponent<Fragment>().UpdateFragment();
				}
			}
		}

		public override void FinishState()
		{
			foreach (FragmentComponent fragment in fragments)
			{
				if (fragment.instance != null)
				{
					fragment.instance.GetComponent<Fragment>().Destroy();
				}
			}
			packWasShown = false;
			base.FinishState();
		}

		private void InitFragment(Button button, string name)
		{
			button.onClick.AddListener(delegate
			{
				OnFragmentButton(name);
			});
		}

		private void ShowRateUsIfNeeded()
		{
			if (startParameter != null && !startParameter.disableAds)
			{
				int @int = PlayerPrefs.GetInt("timeSinceStartup", 0);
				if (Manager.Get<ModelManager>().timeBasedRateUs.HasToShowRateReminder(@int))
				{
					Manager.Get<StateMachineManager>().PushState<RateUsState>();
				}
				else
				{
					//TryToShowAd();
				}
			}
		}

		public bool TrySwitchFragmenTo(string name)
		{
			FragmentComponent fragmentComponent = fragments.FirstOrDefault((FragmentComponent frag) => frag.fragmentName == name);
			if (fragmentComponent == null)
			{
				return false;
			}
			StateMachineManager stateMachineManager = Manager.Get<StateMachineManager>();
			if (!stateMachineManager.IsStateInStack(typeof(PauseState)))
			{
				stateMachineManager.PopStatesUntil<GameplayState>();
				stateMachineManager.PushState<PauseState>(new PauseStateStartParameter
				{
					categoryToOpen = name,
					disableAds = true
				});
				return true;
			}
			OnFragmentButton(name);
			return true;
		}

		private void OnFragmentButton(string name)
		{
			if (currentFragment != name)
			{
				DisableAllFragments();
				ResetBackgroundOverlay();
				ActivateFragment(name);
				DisableAllButtons();
				ActivateButton(name);
				currentFragment = name;
			}
		}

		private void DisableAllFragments()
		{
			IEnumerator enumerator = base.connector.fragmentParent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					transform.gameObject.SetActive(value: false);
					transform.gameObject.GetComponent<Fragment>().Disable();
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

		public void ChangeFor(string name)
		{
			OnFragmentButton(name);
		}

		public void ChangeForCrafting(ushort blockId)
		{
			blockClicked = blockId;
			OnFragmentButton("Crafting");
		}

		public void RevalidateFragmentsNotfications()
		{
		}

		private void ActivateFragment(string name)
		{
			FragmentComponent fragmentComponent = fragments.FirstOrDefault((FragmentComponent frag) => frag.fragmentName == name);
			if (fragmentComponent == null)
			{
				UnityEngine.Debug.LogErrorFormat("THERE IS NO CATEGORY: {0}; FIX IT NOW NOW NOW!!!", name);
				fragments.First((FragmentComponent frag) => frag.fragmentName == defaultCategoryToOpen);
			}
			if (fragmentComponent.instance == null)
			{
				if (fragmentComponent.prefab == null)
				{
					fragmentComponent.prefab = Resources.Load<GameObject>(fragmentComponent.prefabResource);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(fragmentComponent.prefab, base.connector.fragmentParent);
				gameObject.GetComponent<Fragment>().Init(new FragmentStartParameter
				{
					pauseState = this,
					pauseStartParameter = startParameter
				});
				fragmentComponent.instance = gameObject;
			}
			else
			{
				fragmentComponent.instance.SetActive(value: true);
				fragmentComponent.instance.GetComponent<Fragment>().UpdateFragment();
			}
		}

		private void DisableAllButtons()
		{
			foreach (FragmentComponent fragment in fragments)
			{
				fragment.button.gameObject.GetComponent<ColorController>().category = ColorManager.ColorCategory.SECONDARY_COLOR;
				fragment.button.gameObject.GetComponent<ColorController>().UpdateColor();
			}
		}

		private void ActivateButton(string name)
		{
			ColorController component = fragments.First((FragmentComponent frag) => frag.fragmentName == name).button.gameObject.GetComponent<ColorController>();
			component.category = ColorManager.ColorCategory.HIGHLIGHT_COLOR;
			component.UpdateColor();
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		private void OnWardrobe()
		{
			Manager.Get<StateMachineManager>().PushState<DressupState>();
		}

		private void InitDevMode()
		{
			fragments.First((FragmentComponent frag) => frag.fragmentName == "Developer").button.transform.parent.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
		}

		private void CheckOffersFromUpdate()
		{
			if (!packWasShown && Manager.Contains<OfferPackManager>())
			{
				OfferPackManager offerPackManager = Manager.Get<OfferPackManager>();
				if (offerPackManager.ShouldShowStarterPack(Mathf.RoundToInt(pauseTimeStamp)))
				{
					Manager.Get<StateMachineManager>().PushState<StarterPackState>();
					packWasShown = true;
				}
			}
		}
	}
}
