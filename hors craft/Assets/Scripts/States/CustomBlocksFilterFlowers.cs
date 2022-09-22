// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomBlocksFilterFlowers
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;

namespace States
{
	public class CustomBlocksFilterFlowers : CustomBlocksFilter
	{
		public override List<Voxel> GetCustomBlocks()
		{
			List<Voxel> source = (from v in Engine.Blocks
				where v.blockCategory == Voxel.Category.none
				select v).ToList();
			source = (from v in source
				where IsInitialVoxel(v)
				select v).ToList();
			source.Sort(delegate(Voxel a, Voxel b)
			{
				GrowableInfoVoxel growableInfoVoxel = Manager.Get<FarmingManager>().growablesData.FirstOrDefault((GrowableInfoVoxel gd) => gd.initialBlockIndex == a.GetUniqueID());
				GrowableInfoVoxel growableInfoVoxel2 = Manager.Get<FarmingManager>().growablesData.FirstOrDefault((GrowableInfoVoxel gd) => gd.initialBlockIndex == b.GetUniqueID());
				int requiredLevel = Manager.Get<ModelManager>().progressSettings.GetRequiredLevel(growableInfoVoxel.settingsKey);
				int requiredLevel2 = Manager.Get<ModelManager>().progressSettings.GetRequiredLevel(growableInfoVoxel2.settingsKey);
				return requiredLevel - requiredLevel2;
			});
			return source;
		}

		private bool IsInitialVoxel(Voxel voxel)
		{
			return Manager.Get<FarmingManager>().growablesData.Any((GrowableInfoVoxel d) => d.initialBlockIndex == voxel.GetUniqueID());
		}
	}
}
