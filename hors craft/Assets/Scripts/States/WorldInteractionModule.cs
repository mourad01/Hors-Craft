// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WorldInteractionModule
using Common.Managers;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class WorldInteractionModule : GameplayModule
	{
		public Button addButton;

		public Image addButtonImage;

		public Button digButton;

		public Button rotateButton;

		public Slider blocksEnergySlider;

		public Text blocksEnergyText;

		public Button blocksEnergyButton;

		private bool restrictBlocksPlacement;

		private bool enableLimitedblocks;

		private int maxBlocksToPlace;

		private int currentMaxBlocks;

		private int previousBlocksCount;

		private const string keyBlocksToPlace = "blocks.left.to.place";

		private SelectedVoxelContext voxelInteraction;

		private RotateContext rotateContext;

		private ushort lastVoxelSet;

		protected override Fact[] listenedFacts => new Fact[4]
		{
			Fact.BLOCK_HELD,
			Fact.IN_FRONT_OF_ROTATABLE,
			Fact.VOXEL_SELECTED,
			Fact.MCPE_STEERING
		};

		public override void Init()
		{
			base.Init();
			if (!Application.isPlaying)
			{
				return;
			}
			restrictBlocksPlacement = Manager.Get<ModelManager>().blocksUnlocking.BlocksPlacementRestricted();
			enableLimitedblocks = Manager.Get<ModelManager>().blocksUnlocking.IsBlocksLimitedEnabled();
			if (restrictBlocksPlacement)
			{
				blocksEnergySlider.gameObject.SetActive(value: true);
				maxBlocksToPlace = Manager.Get<ModelManager>().blocksUnlocking.MaxBlocksToPlace();
				if (Manager.Get<ModelManager>().blocksUnlocking.BlocksRefillEnabled())
				{
					AutoRefreshingStock.InitStockItem("blocks.left.to.place", Manager.Get<ModelManager>().blocksUnlocking.GetTimeForBlockRefill(), maxBlocksToPlace);
				}
				else
				{
					AutoRefreshingStock.InitStockItem("blocks.left.to.place", 1E+07f, maxBlocksToPlace);
				}
				SetListenerToButton(blocksEnergyButton, OnRefillBlocksEnergy);
			}
			else if (enableLimitedblocks)
			{
				blocksEnergySlider.gameObject.SetActive(value: true);
				SetListenerToButton(blocksEnergyButton, OnRefillBlocks);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (restrictBlocksPlacement)
			{
				int stockCount = AutoRefreshingStock.GetStockCount("blocks.left.to.place");
				blocksEnergySlider.value = (float)stockCount / (float)maxBlocksToPlace;
				blocksEnergyText.text = stockCount + "/" + maxBlocksToPlace;
			}
			if (!enableLimitedblocks)
			{
				return;
			}
			HeldBlockContext heldBlockContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault();
			if (heldBlockContext == null)
			{
				return;
			}
			Voxel voxelType = Engine.GetVoxelType(heldBlockContext.block);
			Singleton<BlocksController>.get.CheckRemoveAds();
			if (PlayerPrefs.GetInt("Max.blocks.Unlimited." + voxelType.rarityCategory.ToString(), 0) == 0 && voxelType.blockCategory != Voxel.Category.craftable)
			{
				blocksEnergySlider.gameObject.SetActive(value: true);
				int blockCount = Singleton<BlocksController>.get.GetBlockCount(voxelType);
				if (blockCount > previousBlocksCount)
				{
					UpdateMaxBlocks();
				}
				previousBlocksCount = blockCount;
				if (currentMaxBlocks == 0)
				{
					blocksEnergySlider.value = 0f;
				}
				else
				{
					blocksEnergySlider.value = (float)blockCount / (float)currentMaxBlocks;
				}
				blocksEnergyText.text = blockCount.ToString();
			}
			else
			{
				blocksEnergySlider.gameObject.SetActive(value: false);
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (changedFacts.Contains(Fact.IN_FRONT_OF_ROTATABLE))
			{
				OnRotatableChange();
			}
			if (changedFacts.Contains(Fact.BLOCK_HELD))
			{
				OnBlockHeldChange();
			}
			if (changedFacts.Contains(Fact.VOXEL_SELECTED))
			{
				OnVoxelSelectionChange();
			}
			if (changedFacts.Contains(Fact.MCPE_STEERING))
			{
				OnSteeringChange();
			}
			CheckForAnyObjectEnabled();
		}

		private void CheckForAnyObjectEnabled()
		{
			base.gameObject.SetActive(addButton.gameObject.activeSelf || digButton.gameObject.activeSelf || rotateButton.gameObject.activeSelf);
		}

		private void OnRotatableChange()
		{
			rotateButton.gameObject.SetActive(MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_ROTATABLE));
			rotateContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<RotateContext>(Fact.IN_FRONT_OF_ROTATABLE).FirstOrDefault();
			if (rotateContext != null)
			{
				SetListenerToButton(rotateButton, rotateContext.onRotate);
			}
		}

		private void OnBlockHeldChange()
		{
			HeldBlockContext heldBlockContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault();
			if (heldBlockContext != null)
			{
				Voxel voxelType = Engine.GetVoxelType(heldBlockContext.block);
				Sprite voxelSprite = VoxelSprite.GetVoxelSprite(voxelType);
				addButtonImage.sprite = voxelSprite;
				if (voxelType.GetUniqueID() != lastVoxelSet)
				{
					Manager.Get<StateMachineManager>().StartCoroutine(AnimateButtonFocus());
					lastVoxelSet = voxelType.GetUniqueID();
				}
				if (enableLimitedblocks)
				{
					UpdateMaxBlocks();
				}
			}
		}

		private void OnVoxelSelectionChange()
		{
			if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				voxelInteraction = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SelectedVoxelContext>(Fact.VOXEL_SELECTED).FirstOrDefault();
				if (voxelInteraction != null)
				{
					SetListenerToButton(addButton, voxelInteraction.onAdd);
					SetListenerToButton(digButton, voxelInteraction.onDig);
				}
			}
		}

		private void OnSteeringChange()
		{
			bool flag = MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING);
			addButton.gameObject.SetActive(!flag);
			digButton.gameObject.SetActive(!flag);
		}

		private IEnumerator AnimateButtonFocus()
		{
			yield return new WaitForSecondsRealtime(0.1f);
			if (addButton != null)
			{
				addButton.GetComponent<Animator>().SetTrigger("ButtonFocus");
			}
		}

		private void OnRefillBlocksEnergy()
		{
			if (AutoRefreshingStock.GetStockCount("blocks.left.to.place") < maxBlocksToPlace)
			{
				Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
				{
					description = "Your blocks will refill with time. Watch an ad to refill them now.",
					translationKey = "blocks.energy.watch.ad.2",
					type = AdsCounters.None,
					configWatchButton = delegate(GameObject go)
					{
						TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
						componentInChildren.defaultText = "Refill Blocks";
						componentInChildren.translationKey = "blocks.energy.refill";
						componentInChildren.ForceRefresh();
					},
					onSuccess = delegate
					{
						AutoRefreshingStock.RefillStock("blocks.left.to.place");
					},
					reason = StatsManager.AdReason.XCRAFT_BLOCKS_REFILL
				});
			}
		}

		private void OnRefillBlocks()
		{
			HeldBlockContext heldBlockContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault();
			if (heldBlockContext != null)
			{
				Voxel voxel = Engine.GetVoxelType(heldBlockContext.block);
				Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
				{
					description = "Watch an ad to refill {0} blocks.",
					translationKey = "blocks.energy.watch.ad.2",
					numberOfAdsNeeded = Singleton<BlocksController>.get.GetMaxRarityBlocks(voxel),
					type = AdsCounters.None,
					configWatchButton = delegate(GameObject go)
					{
						TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
						componentInChildren.defaultText = "Refill Blocks";
						componentInChildren.translationKey = "blocks.energy.refill";
						componentInChildren.ForceRefresh();
					},
					onSuccess = delegate
					{
						Singleton<BlocksController>.get.FillBlock(voxel);
					},
					reason = StatsManager.AdReason.XCRAFT_BLOCKS_REFILL
				});
			}
		}

		private void UpdateMaxBlocks()
		{
			HeldBlockContext heldBlockContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault();
			if (heldBlockContext != null)
			{
				Voxel voxelType = Engine.GetVoxelType(heldBlockContext.block);
				currentMaxBlocks = Singleton<BlocksController>.get.GetBlockCount(voxelType);
			}
		}
	}
}
