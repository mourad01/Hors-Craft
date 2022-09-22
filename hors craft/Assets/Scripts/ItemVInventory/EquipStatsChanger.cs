// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.EquipStatsChanger
using Common.Managers;
using Gameplay;

namespace ItemVInventory
{
	public class EquipStatsChanger : AbstractEquipmentBehaviour
	{
		public PlayerStats statsToChange;

		public override bool CanEquip()
		{
			return true;
		}

		public override void Equip(int level)
		{
			ItemsUpgradeStatsModule itemsUpgradeStatsSettings = Manager.Get<ModelManager>().itemsUpgradeStatsSettings;
			for (int i = 0; i <= level; i++)
			{
				statsToChange.Add(new PlayerStats.Modifier
				{
					value = itemsUpgradeStatsSettings.GetUpgradeStats(base.itemDefinition.id, i, 1),
					priority = 2,
					Action = ((float toAction, float value) => value + toAction)
				});
			}
		}

		public override void OnLevelChanged(int newLevel, int oldLevel)
		{
			ItemsUpgradeStatsModule itemsUpgradeStatsSettings = Manager.Get<ModelManager>().itemsUpgradeStatsSettings;
			for (int i = oldLevel + 1; i <= newLevel; i++)
			{
				statsToChange.Add(new PlayerStats.Modifier
				{
					value = itemsUpgradeStatsSettings.GetUpgradeStats(base.itemDefinition.id, i, 1),
					priority = 2,
					Action = ((float toAction, float value) => value + toAction)
				});
			}
		}
	}
}
