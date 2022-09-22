// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlocksPlacedCounter
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class BlocksPlacedCounter : Singleton<BlocksPlacedCounter>
	{
		private int counter;

		private const float SENDING_MIN_INTERVALS = 5f;

		private float lastSendTime;

		public void Count()
		{
			counter++;
			if (Time.realtimeSinceStartup > lastSendTime + 5f)
			{
				Manager.Get<StatsManager>().XCraftBlocksPlaced(counter);
				counter = 0;
				lastSendTime = Time.realtimeSinceStartup;
			}
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("blocks");
			}
		}

		public void Count(Voxel voxel)
		{
			counter++;
			if (Time.realtimeSinceStartup > lastSendTime + 5f)
			{
				Manager.Get<StatsManager>().XCraftBlocksPlaced(counter);
				counter = 0;
				lastSendTime = Time.realtimeSinceStartup;
			}
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("blocks." + voxel.rarityCategory.ToString().ToLower());
			}
		}
	}
}
