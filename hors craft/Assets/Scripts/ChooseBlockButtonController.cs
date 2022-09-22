// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseBlockButtonController
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBlockButtonController : MonoBehaviour
{
	public delegate bool Check();

	public delegate bool OnClick(ushort id);

	public ushort blockID;

	public Check active;

	public OnClick onButtonClicked;

	public Action<ushort> onCraftableClicked;

	public bool IsActive()
	{
		return active == null || active();
	}

	private bool IsPauseOrBlocksPopup()
	{
		StateMachineManager stateMachineManager = Manager.Get<StateMachineManager>();
		return stateMachineManager.IsCurrentStateA<PauseState>() || stateMachineManager.IsCurrentStateA<BlocksPopupState>();
	}

	private void Awake()
	{
		Button component = GetComponent<Button>();
		if (component != null)
		{
			component.onClick.AddListener(delegate
			{
				if (IsPauseOrBlocksPopup() && CheckIfCanPutThis())
				{
					Voxel voxel = Engine.Blocks[blockID];
					if (Singleton<BlocksController>.get.IsBlocked(blockID))
					{
						if (Singleton<BlocksController>.get.singleAdUnlockMode > (int)voxel.rarityCategory)
						{
							ShowBlocksPopup(voxel);
						}
						else if (PlayerPrefs.GetInt("TutorialChestShowed", 0) == 0)
						{
							PlayerPrefs.SetInt("TutorialChestShowed", 1);
							Manager.Get<StateMachineManager>().PushState<TutorialState>(new TutorialStartParameter
							{
								prefabToSpawn = "ChestTutorial"
							});
						}
						else
						{
							Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Shop");
						}
					}
					else if (GetComponent<CraftItem>().Locked())
					{
						if (onButtonClicked(blockID))
						{
							GetComponent<CraftItem>().Lock(enable: false);
						}
					}
					else
					{
						SetBlockImageAndButton();
						if (!Manager.Get<ModelManager>().blocksUnlocking.Is3dBlocksViewEnabled())
						{
							Image component2 = GetComponent<Image>();
							Color color = component2.color;
							color.a = 255f;
							component2.color = color;
							Button component3 = GetComponent<Button>();
							ColorBlock colors = component3.colors;
							colors.normalColor = color;
							component3.colors = colors;
						}
						ushort heldBlock = ExampleInventory.HeldBlock;
						ExampleInventory.HeldBlock = blockID;
						float verticalNormalizedPosition = GetComponentInParent<ScrollRect>().verticalNormalizedPosition;
						CheckSwitchingToIsometric(voxel, heldBlock);
						if (Singleton<BlocksController>.get.enableRarityBlocks)
						{
							PlayerPrefs.SetFloat("scrollRectY" + voxel.rarityCategory.ToString().ToLower(), verticalNormalizedPosition);
							PlayerPrefs.SetInt("lastOpenedRarityBlocksCategory", (int)voxel.rarityCategory);
							Singleton<BlocksController>.get.SeeBlock(voxel);
						}
						else
						{
							PlayerPrefs.SetFloat("scrollRectY" + voxel.blockCategory.ToString().ToLower(), verticalNormalizedPosition);
							PlayerPrefs.SetInt("lastOpenedBlocksCategory", (int)voxel.blockCategory);
						}
					}
				}
			});
		}
	}

	private void ShowBlocksPopup(Voxel v)
	{
		Manager.Get<StateMachineManager>().PushState<UniversalPopupState>(new UniversalPopupStateStartParameter
		{
			prefabToSpawn = "ChestPopup",
			configPopup = delegate(DefaultUniversalPopup popup)
			{
				popup.returnButton.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
				popup.GetActionButton("watchButton").onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
					{
						translationKey = "watch.ad.unlock.block",
						description = "Watch an ad to unlock this block",
						reason = StatsManager.AdReason.XCRAFT_UNLOCK_BLOCK,
						type = AdsCounters.None,
						voxelSpritesToUnlock = new List<Sprite>
						{
							VoxelSprite.GetVoxelSprite(v.GetUniqueID())
						},
						onSuccess = delegate(bool success)
						{
							if (success)
							{
								Manager.Get<StateMachineManager>().PopState();
								Singleton<BlocksController>.get.UnlockBlock(v);
							}
						},
						configWatchButton = delegate(GameObject go)
						{
							TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
							componentInChildren.defaultText = "Watch";
							componentInChildren.translationKey = "menu.watch";
							componentInChildren.ForceRefresh();
						}
					});
				});
				popup.GetActionButton("shopButton").onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
					Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Shop");
				});
			}
		});
	}

	private void CheckSwitchingToIsometric(Voxel voxel, ushort ID)
	{
		ISpawnableVoxelEvent component = voxel.GetComponent<ISpawnableVoxelEvent>();
		if (component != null && voxel.isometricPlacment)
		{
			SwitchToIsometric(component.GetPrefabs()[0], voxel, ID);
		}
		else
		{
			Manager.Get<StateMachineManager>().PopState();
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

	private void SetBlockImageAndButton()
	{
		if (!Manager.Get<ModelManager>().blocksUnlocking.Is3dBlocksViewEnabled())
		{
			Image component = GetComponent<Image>();
			Color color = component.color;
			color.a = 255f;
			component.color = color;
			Button component2 = GetComponent<Button>();
			ColorBlock colors = component2.colors;
			colors.normalColor = color;
			component2.colors = colors;
		}
	}

	private bool CheckIfCanPutThis()
	{
		if (Manager.Get<CraftingManager>().IsBlockCraftable(blockID) && Singleton<PlayerData>.get.playerItems.GetCraftableCountByBlock(blockID) <= 0)
		{
			if (onCraftableClicked != null)
			{
				onCraftableClicked(blockID);
			}
			return false;
		}
		return true;
	}
}
