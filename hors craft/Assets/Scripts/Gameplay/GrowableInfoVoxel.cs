// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GrowableInfoVoxel
using System;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	[Serializable]
	public class GrowableInfoVoxel : GrowableInfo
	{
		public ushort initialBlockIndex;

		[NonSerialized]
		public VoxelInfo info;

		[SerializeField]
		public VoxelGrowthStage[] stages;

		public GrowableInfoVoxel(VoxelInfo info)
		{
			this.info = info;
			grow = OnGrow;
			getUniqueId = GetKey;
			growStages = stages;
		}

		public GrowableInfoVoxel()
		{
		}

		public GrowableInfoVoxel(GrowableInfoVoxel serializedInfo, VoxelInfo info)
		{
			settingsKey = serializedInfo.settingsKey;
			growStages = serializedInfo.stages;
			this.info = info;
			grow = OnGrow;
			getUniqueId = GetKey;
		}

		public static string GetKey(VoxelInfo info)
		{
			return "growable." + info.GetGlobalIndex();
		}

		private void OnGrow()
		{
			VoxelGrowthStage voxelGrowthStage = CurrentStage() as VoxelGrowthStage;
			if (voxelGrowthStage != null)
			{
				ushort nextBlockIndex = voxelGrowthStage.nextBlockIndex;
				Voxel.PlaceBlock(info, nextBlockIndex);
			}
		}

		private VoxelGrowthStage GetGrowthStage(int stage)
		{
			return growStages[stage] as VoxelGrowthStage;
		}

		private string GetKey()
		{
			return GetKey(info);
		}

		public override string GetDescription()
		{
			string text = info.GetGlobalIndex();
			for (int i = 0; i < growStages.Length; i++)
			{
				text = text + " | " + (growStages[i] as VoxelGrowthStage).nextBlockIndex;
			}
			return text;
		}
	}
}
