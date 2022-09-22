// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.AbstractEquipmentBehaviour
using UnityEngine;

namespace ItemVInventory
{
	public abstract class AbstractEquipmentBehaviour : MonoBehaviour
	{
		private ItemDefinition _itemDefinition;

		protected ItemDefinition itemDefinition => _itemDefinition ?? (_itemDefinition = GetComponentInParent<ItemDefinition>());

		public abstract bool CanEquip();

		public abstract void Equip(int level);

		public abstract void OnLevelChanged(int newLevel, int oldLevel);
	}
}
