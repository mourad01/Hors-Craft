// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.FishingHoverAction
using com.ootii.Cameras;
using Common.Managers;
using States;
using UnityEngine;

namespace Uniblocks
{
	public class FishingHoverAction : HoverAction
	{
		private NearbyWaterContext fishingContext;

		private PlayerFishingController playerFishingController;

		private Vector3 forwardPosition;

		private Index lastPlayerVoxelIndex;

		private VoxelInfo firstWaterBlock;

		private int radius = 2;

		public FishingHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			fishingContext = new NearbyWaterContext
			{
				OnFishingAction = OnFishing
			};
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (playerFishingController == null)
			{
				playerFishingController = CameraController.instance.Anchor.GetComponentInChildren<PlayerFishingController>();
			}
			if (playerFishingController != null)
			{
				CheckNearbyVoxels();
			}
		}

		public void OnFishing()
		{
			CheckNearbyVoxels(recheckWaterBlock: true);
			PreparePlayerToFishing();
			Manager.Get<StateMachineManager>().PushState<FishingState>();
		}

		private void CheckNearbyVoxels(bool recheckWaterBlock = false)
		{
			forwardPosition = playerFishingController.transform.position + playerFishingController.transform.forward * 3f;
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(forwardPosition);
			if (voxelInfo == null)
			{
				return;
			}
			ChunkData chunk = voxelInfo.chunk;
			if (chunk != null && chunk.NeighborChunks != null)
			{
				Index index = voxelInfo.index;
				if (recheckWaterBlock)
				{
					lastPlayerVoxelIndex = new Index(0, 0, 0);
				}
				if (!index.IsEqual(lastPlayerVoxelIndex))
				{
					bool enabled = IsPlayerNearbyWater(chunk, index, recheckWaterBlock);
					MonoBehaviourSingleton<GameplayFacts>.get.SetContext(Fact.NEARBY_WATER, fishingContext, enabled);
				}
			}
		}

		private void PreparePlayerToFishing()
		{
			int num = 10;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 1; i < num; i++)
			{
				ushort voxel = firstWaterBlock.chunk.GetVoxel(firstWaterBlock.index.x, firstWaterBlock.index.y, firstWaterBlock.index.z + i);
				if (IsWaterVoxel(voxel))
				{
					num2++;
					continue;
				}
				break;
			}
			for (int j = 1; j < num; j++)
			{
				ushort voxel = firstWaterBlock.chunk.GetVoxel(firstWaterBlock.index.x, firstWaterBlock.index.y, firstWaterBlock.index.z - j);
				if (IsWaterVoxel(voxel))
				{
					num3++;
					continue;
				}
				break;
			}
			for (int k = 1; k < num; k++)
			{
				ushort voxel = firstWaterBlock.chunk.GetVoxel(firstWaterBlock.index.x - k, firstWaterBlock.index.y, firstWaterBlock.index.z);
				if (IsWaterVoxel(voxel))
				{
					num4++;
					continue;
				}
				break;
			}
			for (int l = 1; l < num; l++)
			{
				ushort voxel = firstWaterBlock.chunk.GetVoxel(firstWaterBlock.index.x + l, firstWaterBlock.index.y, firstWaterBlock.index.z);
				if (IsWaterVoxel(voxel))
				{
					num5++;
					continue;
				}
				break;
			}
			Vector3 vector = new Vector3(num5 - num4, 0f, num2 - num3);
			if ((!playerFishingController.BoatFishing && vector != Vector3.zero) || playerFishingController.HaveRotate)
			{
				playerFishingController.TurnToWater(vector);
			}
			VoxelInfo voxelInfo = firstWaterBlock.chunk.GetVoxelInfo(firstWaterBlock.index.x, firstWaterBlock.index.y, firstWaterBlock.index.z + 1);
			if (!IsWaterVoxel(voxelInfo.GetVoxel()))
			{
				Engine.VoxelInfoToPosition(voxelInfo);
			}
			voxelInfo = firstWaterBlock.chunk.GetVoxelInfo(firstWaterBlock.index.x, firstWaterBlock.index.y, firstWaterBlock.index.z - 1);
			if (!IsWaterVoxel(voxelInfo.GetVoxel()))
			{
				Engine.VoxelInfoToPosition(voxelInfo);
			}
			voxelInfo = firstWaterBlock.chunk.GetVoxelInfo(firstWaterBlock.index.x + 1, firstWaterBlock.index.y, firstWaterBlock.index.z);
			if (!IsWaterVoxel(voxelInfo.GetVoxel()))
			{
				Engine.VoxelInfoToPosition(voxelInfo);
			}
			voxelInfo = firstWaterBlock.chunk.GetVoxelInfo(firstWaterBlock.index.x - 1, firstWaterBlock.index.y, firstWaterBlock.index.z);
			if (!IsWaterVoxel(voxelInfo.GetVoxel()))
			{
				Engine.VoxelInfoToPosition(voxelInfo);
			}
			float num6 = Vector3.Distance(playerFishingController.transform.position, playerFishingController.transform.position + vector);
			if (num6 == 0f)
			{
				num6 = num;
			}
			if (playerFishingController.isInBoat)
			{
				num6 = num;
			}
			Manager.Get<FishingManager>().SetWaterMaxDistance(num6);
		}

		private bool IsPlayerNearbyWater(ChunkData chunk, Index index, bool recheckWaterBlock = false)
		{
			if (recheckWaterBlock)
			{
				firstWaterBlock = null;
			}
			radius = playerFishingController.GetFishingRange();
			int fishingDeep = playerFishingController.GetFishingDeep();
			lastPlayerVoxelIndex = index;
			for (int i = 1; i < fishingDeep; i++)
			{
				for (int j = -radius; j <= radius; j++)
				{
					for (int k = -radius; k <= radius; k++)
					{
						if (k * k + j * j > radius * radius)
						{
							continue;
						}
						ushort voxel = chunk.GetVoxel(index.x + k, index.y - i, index.z + j);
						if (!IsWaterVoxel(voxel))
						{
							continue;
						}
						if (IsAirVoxel(chunk.GetVoxel(index.x + k, index.y - i + 1, index.z + j)))
						{
							if (firstWaterBlock == null)
							{
								firstWaterBlock = chunk.GetVoxelInfo(index.x + k, index.y - i, index.z + j);
							}
							return true;
						}
						return false;
					}
				}
			}
			firstWaterBlock = null;
			return false;
		}

		private bool IsWaterVoxel(ushort id)
		{
			return id == Engine.usefulIDs.waterBlockID;
		}

		private bool IsAirVoxel(ushort id)
		{
			return id == 0;
		}
	}
}
