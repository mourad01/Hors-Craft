// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.HoverActionsConnector
using Common.Audio;
using Common.Behaviours;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay;
using Gameplay.Audio;
using States;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class HoverActionsConnector
	{
		public const int DOORS_LAYER = 18;

		public const int MOBS_LAYER = 16;

		public const int CHANGE_CLOTHES_LAYER = 22;

		private bool blocksPlacementRestricted;

		private bool enableLimitedblocks;

		private const string keyBlocksToPlace = "blocks.left.to.place";

		public PlayerMovement movement
		{
			get;
			private set;
		}

		public HoverActionsConnector(PlayerMovement movement)
		{
			this.movement = movement;
			blocksPlacementRestricted = Manager.Get<ModelManager>().blocksUnlocking.BlocksPlacementRestricted();
			enableLimitedblocks = Manager.Get<ModelManager>().blocksUnlocking.IsBlocksLimitedEnabled();
		}

		public bool OnVoxelPlace(VoxelInfo raycast)
		{
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(raycast.GetVoxel());
			if (instanceForVoxelId != null)
			{
				if (instanceForVoxelId.GetType() == typeof(VoxelDoorOpenClose))
				{
					PlaySound(GameSound.BLOCK_PLACE);
					instanceForVoxelId.OnMouseDown(0, raycast);
					return true;
				}
				if (blocksPlacementRestricted)
				{
					int stockCount = AutoRefreshingStock.GetStockCount("blocks.left.to.place");
					if (stockCount <= 0)
					{
						Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
						{
							description = "You don't have any more blocks to place. Watch an ad to refill them.",
							translationKey = "blocks.energy.watch.ad.1",
							type = AdsCounters.None,
							configWatchButton = delegate(GameObject go)
							{
								TranslateText componentInChildren2 = go.GetComponentInChildren<TranslateText>();
								componentInChildren2.defaultText = "Refill Blocks";
								componentInChildren2.translationKey = "blocks.energy.refill";
								componentInChildren2.ForceRefresh();
							},
							onSuccess = delegate
							{
								AutoRefreshingStock.RefillStock("blocks.left.to.place");
							},
							reason = StatsManager.AdReason.XCRAFT_BLOCKS_REFILL
						});
						return false;
					}
					AutoRefreshingStock.DecrementStockCount("blocks.left.to.place");
				}
				else if (enableLimitedblocks && !MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper)
				{
					HeldBlockContext heldBlockContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault();
					if (heldBlockContext != null)
					{
						Voxel voxel = Engine.GetVoxelType(heldBlockContext.block);
						if (PlayerPrefs.GetInt("Max.blocks.Unlimited." + voxel.rarityCategory.ToString(), 0) == 0 && voxel.blockCategory != Voxel.Category.craftable)
						{
							int blockCount = Singleton<BlocksController>.get.GetBlockCount(voxel);
							if (blockCount <= 0)
							{
								Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
								{
									description = "You don't have any more blocks to place. Watch an ad to refill {0} blocks.",
									translationKey = "blocks.energy.watch.ad.1",
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
								return false;
							}
							Singleton<BlocksController>.get.DecrementBlockCount(voxel);
						}
					}
				}
				if (Singleton<BlocksController>.get.enableRarityBlocksNoAds)
				{
					Singleton<BlocksPlacedCounter>.get.Count(Engine.GetVoxelType(MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault().block));
				}
				else
				{
					Singleton<BlocksPlacedCounter>.get.Count();
				}
				PlaySound(GameSound.BLOCK_PLACE);
				if (Engine.GetVoxelGameObject(ExampleInventory.HeldBlock).GetComponent<SpawnPrefab>() != null)
				{
					Engine.GetVoxelGameObject(ExampleInventory.HeldBlock).GetComponent<SpawnPrefab>().Spawn(raycast);
				}
				else
				{
					instanceForVoxelId.OnMouseDown(0, raycast);
				}
				return true;
			}
			return false;
		}

		public void OnVoxelDestroyed(VoxelInfo raycast)
		{
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(raycast.GetVoxel());
			if (instanceForVoxelId != null)
			{
				PlaySound(GameSound.BLOCK_DESTROY);
				instanceForVoxelId.OnMouseDown(1, raycast);
			}
			Manager.Get<QuestManager>().OnBlockDestroyed(raycast.GetVoxel());
		}

		public void OnVoxelRotate(VoxelInfo raycast)
		{
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(raycast.GetVoxel());
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnBlockRotate(raycast);
			}
		}

		private void PlaySound(GameSound clip)
		{
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(clip);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}
}
