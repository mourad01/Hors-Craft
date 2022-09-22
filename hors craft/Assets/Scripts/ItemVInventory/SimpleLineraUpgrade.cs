// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.SimpleLineraUpgrade
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace ItemVInventory
{
	public class SimpleLineraUpgrade : AbstractUpgradeBehaviour
	{
		public string customShardName;

		public override bool CanUpgrade(int level)
		{
			return true;
		}

		public override UpgradeRequirements UpgradeRequirements(int level, ItemDefinition itemDefinition)
		{
			UpgradeRequirements upgradeRequirements = new UpgradeRequirements();
			upgradeRequirements.itemsIds = new List<string>
			{
				(!string.IsNullOrEmpty(customShardName)) ? customShardName : (itemDefinition.id + "Shard")
			};
			upgradeRequirements.itemsCount = new List<int>
			{
				Manager.Get<ModelManager>().itemsUpgradeRequirementsSettings.GetUpgradeStats(level, (int)Mathf.Pow(2f, level))
			};
			return upgradeRequirements;
		}

		public override void OnUpgrade()
		{
		}
	}
}
