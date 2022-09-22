// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.VoxelGrowthStage
using Common.Managers;
using System;

namespace Gameplay
{
	[Serializable]
	public class VoxelGrowthStage : GrowthStage
	{
		public ushort nextBlockIndex;

		public override float GrowDuration(GrowableInfo info)
		{
			return Manager.Get<ModelManager>().growthSettings.GetGrowthDuration(info.settingsKey, info.StageIndex(this), defaultGrowDuration);
		}
	}
}
