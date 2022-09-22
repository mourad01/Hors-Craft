// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.EquipHpChange
using System;

namespace ItemVInventory
{
	public class EquipHpChange : AbstractEquipmentBehaviour
	{
		[Serializable]
		public class EquipLevelTabel
		{
			public int level;

			public int hpChange = 1;
		}

		public EquipLevelTabel[] levelsChange;

		public override bool CanEquip()
		{
			return true;
		}

		public override void Equip(int level)
		{
			Health componentInChildren = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<Health>();
			componentInChildren.maxHp += level;
			componentInChildren.hp += level;
		}

		public override void OnLevelChanged(int newLevel, int oldLevel)
		{
			int num = newLevel - oldLevel;
			Health componentInChildren = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<Health>();
			componentInChildren.maxHp += num;
			componentInChildren.hp += num;
		}
	}
}
