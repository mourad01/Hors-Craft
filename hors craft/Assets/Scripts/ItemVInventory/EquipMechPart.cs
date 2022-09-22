// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.EquipMechPart
using Gameplay;
using System;
using System.Linq;

namespace ItemVInventory
{
	public class EquipMechPart : EquipMesh
	{
		public string key;

		public override bool CanEquip()
		{
			return true;
		}

		public override void OnLevelChanged(int newLevel, int oldLevel)
		{
			FormChanger componentInChildren = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<FormChanger>();
			MeshChangeData meshChangeData = prefabsConfig.LastOrDefault((MeshChangeData c) => c.level <= newLevel);
			componentInChildren.SetPart(key, Array.IndexOf(prefabsConfig, meshChangeData));
			ItemDefinition componentInParent = base.gameObject.GetComponentInParent<ItemDefinition>();
			if (!(componentInParent == null))
			{
				componentInParent.itemSprite = meshChangeData.sprite;
			}
		}

		protected override void TryGetPrefabHolder(int level)
		{
		}

		public override void Equip(int level)
		{
			base.Equip(level);
			OnLevelChanged(level, level - 1);
		}
	}
}
