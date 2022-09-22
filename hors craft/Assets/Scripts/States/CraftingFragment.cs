// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingFragment
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class CraftingFragment : Fragment
	{
		public enum State
		{
			Recipes,
			Crafting,
			Requirements,
			FullyUpgraded,
			None
		}

		public class CrafttStartParameter : FragmentStartParameter
		{
			public CraftingFragment parentFragment;

			public State toInitialize;

			public int sourceBlockId;

			public CrafttStartParameter(CraftingFragment parentFragment, State toInitialize = State.None, int sourceBlockId = -1)
			{
				this.parentFragment = parentFragment;
				this.toInitialize = toInitialize;
				this.sourceBlockId = sourceBlockId;
				if (parentFragment.startParameter != null)
				{
					pauseState = parentFragment.startParameter.pauseState;
					pauseStartParameter = parentFragment.startParameter.pauseStartParameter;
				}
			}
		}

		public GameObject fragmentContainer;

		public GameObject resourcesPrefab;

		public GameObject recipesPrefab;

		public GameObject craftPrefab;

		public GameObject requirementsPrefab;

		public GameObject fullyUpgradedPrefab;

		public Action<bool> onSuccess;

		public bool enableResourcesForAd;

		public int savedCraftableId = -1;

		public bool useCraftableId;

		protected GameObject[] fragmentInstances;

		protected GameObject resourcesInstance;

		public void UpdateRecipesFragment()
		{
			fragmentInstances[0].GetComponent<Fragment>().UpdateFragment();
		}

		public override void UpdateFragment()
		{
			if (startParameter != null)
			{
				InitWithBlockNumber(startParameter.pauseState.blockClicked);
			}
			UpdateChildFragments();
		}

		public void UpdateChildFragments()
		{
			Fragment component = resourcesInstance.GetComponent<Fragment>();
			if (component != null)
			{
				component.UpdateFragment();
			}
			for (int i = 0; i < fragmentInstances.Length; i++)
			{
				if (!(fragmentInstances[i] == null))
				{
					component = fragmentInstances[i].GetComponent<Fragment>();
					if (!(component == null))
					{
						component.UpdateFragment();
					}
				}
			}
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			Manager.Get<CraftingManager>().AddStartingResources();
			enableResourcesForAd = checkIfResourcesForAdEnabled();
			if (fragmentInstances == null)
			{
				fragmentInstances = new GameObject[4];
			}
			startParameter = parameter;
			if (resourcesInstance == null)
			{
				resourcesInstance = UnityEngine.Object.Instantiate(resourcesPrefab, fragmentContainer.transform);
				resourcesInstance.GetComponent<Fragment>().Init(new CrafttStartParameter(this));
				resourcesInstance.transform.SetAsFirstSibling();
			}
			if (startParameter != null)
			{
				InitWithBlockNumber(startParameter.pauseState.blockClicked);
			}
			else
			{
				SetState(State.Recipes, new CrafttStartParameter(this, State.Recipes, 0));
			}
		}

		protected void InitWithBlockNumber(int blockId)
		{
			int craftableIdFromBlock = Manager.Get<CraftingManager>().GetCraftableIdFromBlock(blockId);
			if (useCraftableId)
			{
				craftableIdFromBlock = savedCraftableId;
				savedCraftableId = -1;
				useCraftableId = false;
			}
			CraftableStatus craftableStatus = Manager.Get<CraftingManager>().GetCraftableStatus(craftableIdFromBlock);
			State state = State.None;
			switch (craftableStatus)
			{
			case CraftableStatus.Undefined:
				state = State.Recipes;
				break;
			case CraftableStatus.NoResources:
			case CraftableStatus.Craftable:
				state = State.Crafting;
				break;
			case CraftableStatus.FullyUpgraded:
				state = State.FullyUpgraded;
				break;
			default:
				state = State.Requirements;
				break;
			}
			SetState(state, new CrafttStartParameter(this, state, craftableIdFromBlock));
		}

		public void SetState(State stateToActive, int craftableId)
		{
			SetState(stateToActive, new CrafttStartParameter(this, stateToActive, craftableId));
		}

		public virtual void SetState(State stateToActive, CrafttStartParameter parameter)
		{
			savedCraftableId = parameter.sourceBlockId;
			DisableIfNot(stateToActive);
			switch (stateToActive)
			{
			case State.Recipes:
				SetRecipiesFragment(parameter);
				break;
			case State.Crafting:
				SetCraftFragmet(parameter);
				break;
			case State.Requirements:
				if (Manager.Get<ModelManager>().craftingSettings.AreCraftingFree() && Manager.Get<CraftingManager>().GetCraftable(parameter.sourceBlockId).recipeCategory != 0)
				{
					CraftingRequirementsFragment.ShowAd(parameter.sourceBlockId);
				}
				else
				{
					SetRequiermentsFragment(parameter);
				}
				break;
			case State.FullyUpgraded:
				SetFullyUpgradedFragment(parameter);
				break;
			}
		}

		private void DisableIfNot(State stateToActive)
		{
			for (int i = 0; i < fragmentInstances.Length; i++)
			{
				if (fragmentInstances[i] != null)
				{
					fragmentInstances[i].SetActive(stateToActive == (State)i);
				}
			}
		}

		private void SetRecipiesFragment(CrafttStartParameter parameter)
		{
			if (fragmentInstances[0] == null)
			{
				fragmentInstances[0] = UnityEngine.Object.Instantiate(recipesPrefab, fragmentContainer.transform);
				fragmentInstances[0].transform.SetAsLastSibling();
			}
			fragmentInstances[0].GetComponent<Fragment>().Init(parameter);
		}

		private void SetCraftFragmet(CrafttStartParameter parameter)
		{
			if (fragmentInstances[1] == null)
			{
				fragmentInstances[1] = UnityEngine.Object.Instantiate(craftPrefab, fragmentContainer.transform);
				fragmentInstances[1].transform.SetAsLastSibling();
			}
			fragmentInstances[1].GetComponent<Fragment>().Init(parameter);
		}

		private void SetRequiermentsFragment(CrafttStartParameter parameter)
		{
			if (fragmentInstances[2] == null)
			{
				fragmentInstances[2] = UnityEngine.Object.Instantiate(requirementsPrefab, fragmentContainer.transform);
				fragmentInstances[2].transform.SetAsLastSibling();
			}
			fragmentInstances[2].GetComponent<Fragment>().Init(parameter);
		}

		private void SetFullyUpgradedFragment(CrafttStartParameter parameter)
		{
			if (fragmentInstances[3] == null)
			{
				fragmentInstances[3] = UnityEngine.Object.Instantiate(fullyUpgradedPrefab, fragmentContainer.transform);
				fragmentInstances[3].transform.SetAsLastSibling();
			}
			fragmentInstances[3].GetComponent<Fragment>().Init(parameter);
		}

		public virtual void onCraft(int itemId)
		{
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
			if (craftable.status == CraftableStatus.Craftable)
			{
				Singleton<PlayerData>.get.playerItems.Craft(itemId);
				ReportCrafting(succes: true, craftable);
				resourcesInstance.GetComponent<Fragment>().Init(new CrafttStartParameter(this));
				return;
			}
			if (enableResourcesForAd)
			{
				OnGetResourcesForAds("You don't have enough resources. Watch an ad to get more", "menu.not.enough.resources", _useCraftableId: true);
			}
			else
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("crafting.noresources", "Not enough resources"));
			}
			ReportCrafting(succes: false, craftable);
		}

		private void ReportCrafting(bool succes, Craftable craftable)
		{
			Manager.Get<StatsManager>().XcraftCraftAttempted(succes, craftable.GetStatsCategory(), craftable.GetStatsName());
		}

		public void OnGetResourcesForAds(string _defaultText, string _translationKey, bool _useCraftableId)
		{
			useCraftableId = _useCraftableId;
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				description = _defaultText,
				translationKey = _translationKey,
				type = AdsCounters.None,
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.defaultText = "Get resources";
					componentInChildren.translationKey = "menu.get.resources";
					componentInChildren.ForceRefresh();
				},
				onSuccess = delegate
				{
					CraftableList craftableListInstance = Manager.Get<CraftingManager>().GetCraftableListInstance();
					List<CraftableList.ResourceSpawn> resourcesList = craftableListInstance.resourcesList;
					HashSet<int> hashSet = new HashSet<int>(from r in resourcesList
						select r.id);
					foreach (int item in hashSet)
					{
						Singleton<PlayerData>.get.playerItems.AddToResources(item, Manager.Get<ModelManager>().craftingSettings.GetResourceAmountPerAd(item));
					}
				},
				reason = StatsManager.AdReason.XCRAFT_GET_RESOURCES
			});
		}

		public void OnBlockSelected(Craftable craftable)
		{
			if (craftable.craftableType == Craftable.type.Weapon)
			{
				OnWeaponSelected(craftable);
			}
			else if (craftable.craftableType == Craftable.type.Upgradeable)
			{
				Manager.Get<FishingManager>().SetRodConfig(craftable.upgradeId);
			}
			else if (craftable.craftableType == Craftable.type.Custom)
			{
				OnCustomSelected(craftable);
			}
			else
			{
				OnDefaultBlockSelected(craftable.id);
			}
		}

		private void OnWeaponSelected(Craftable craftable)
		{
			if (!Manager.Get<SurvivalManager>().IsCombatTime())
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("crafting.noweaponsduringday", "Cannot equip weapons during the day."));
			}
			else if (Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftable.id) > 0)
			{
				GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
				gameObject.GetComponent<ArmedPlayer>().EquipWeaponWithId(craftable.weaponId);
				Manager.Get<StateMachineManager>().PopState();
			}
		}

		private void OnCustomSelected(Craftable craftable)
		{
			ICustomCraftingItem component = craftable.customCraftableObject.GetComponent<ICustomCraftingItem>();
			bool flag = Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftable.id) > 0;
			if (component != null && flag)
			{
				component.OnUseAction(craftable.id);
			}
		}

		private void OnDefaultBlockSelected(int itemId)
		{
			if (Singleton<PlayerData>.get.playerItems.GetCraftableCount(itemId) > 0)
			{
				ushort num = 0;
				Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
				num = ((craftable.blockId < 0) ? craftable.TryGetMobBlockId() : ((ushort)craftable.blockId));
				ushort heldBlock = ExampleInventory.HeldBlock;
				ExampleInventory.HeldBlock = num;
				Voxel voxelType = Engine.GetVoxelType(num);
				ISpawnableVoxelEvent component = voxelType.GetComponent<ISpawnableVoxelEvent>();
				if (component != null && voxelType.isometricPlacment)
				{
					SwitchToIsometric(component.GetPrefabs()[0], voxelType, heldBlock);
				}
				else
				{
					Manager.Get<StateMachineManager>().PopState();
				}
			}
			else
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("crafting.no.Items", "No items"));
			}
		}

		public void SwitchToIsometric(GameObject prefab, Voxel voxel, ushort lastBlockID)
		{
			GameObject gameObject = new GameObject("Craft placement");
			gameObject.transform.position = PlayerGraphic.GetControlledPlayerInstance().transform.position;
			IsometricObjectPlacementStateStartParameter isometricObjectPlacementStateStartParameter = new IsometricObjectPlacementStateStartParameter();
			isometricObjectPlacementStateStartParameter.canRotate = false;
			if ((bool)voxel.GetComponent<OneTimeBoatSpawnerEvents>())
			{
				((IsometricPlaceableCraft)(isometricObjectPlacementStateStartParameter.obj = gameObject.AddComponent<IsometricPlaceableBoat>())).Init(prefab, voxel, lastBlockID);
			}
			else if ((bool)voxel.GetComponent<PrefabSpawnerVoxelEvents>())
			{
				((IsometricPlaceableCraft)(isometricObjectPlacementStateStartParameter.obj = ((!voxel.GetComponent<PrefabSpawnerVoxelEvents>().placeOnWaterSurface) ? gameObject.AddComponent<IsometricPlaceableCraft>() : gameObject.AddComponent<IsometricPlaceablePrefabOnWater>()))).Init(prefab, voxel, lastBlockID);
			}
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			Manager.Get<StateMachineManager>().PushState<IsometricObjectPlacementState>(isometricObjectPlacementStateStartParameter);
		}

		public bool checkIfResourcesForAdEnabled()
		{
			return Manager.Get<ModelManager>().craftingSettings.IsResourcesPerAdEnabled();
		}

		public void UpdateResourceTab(int index)
		{
			if (!(resourcesInstance == null))
			{
				bool active = (!Manager.Get<ModelManager>().craftingSettings.AreCraftingFree() && index != 0) || (!Manager.Get<ModelManager>().craftingSettings.AreBlueprintsFree() && index == 0);
				resourcesInstance.SetActive(active);
			}
		}
	}
}
