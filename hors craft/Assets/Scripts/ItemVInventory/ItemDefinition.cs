// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.ItemDefinition
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace ItemVInventory
{
	public class ItemDefinition : MonoBehaviour
	{
		[SerializeField]
		private ItemType _itemType;

		public GameObject EquipmentBehavioursParent;

		public GameObject DropBehaviourParent;

		public GameObject UpgradeBehaviuorParent;

		public Sprite itemSprite;

		public Mesh itemMesh;

		public ItemType itemType => _itemType;

		public string id
		{
			get;
			private set;
		}

		public int level
		{
			get;
			private set;
		}

		public void Init(string id)
		{
			this.id = id;
		}

		public bool CanEquip()
		{
			AbstractEquipmentBehaviour[] componentsInChildren = EquipmentBehavioursParent.GetComponentsInChildren<AbstractEquipmentBehaviour>();
			if (componentsInChildren == null || componentsInChildren.Length < 1)
			{
				return false;
			}
			bool flag = true;
			AbstractEquipmentBehaviour[] array = componentsInChildren;
			foreach (AbstractEquipmentBehaviour abstractEquipmentBehaviour in array)
			{
				flag = (flag && abstractEquipmentBehaviour.CanEquip());
			}
			return flag;
		}

		public void OnEquip(int level)
		{
			AbstractEquipmentBehaviour[] componentsInChildren = EquipmentBehavioursParent.GetComponentsInChildren<AbstractEquipmentBehaviour>();
			if (componentsInChildren != null && componentsInChildren.Length >= 1)
			{
				AbstractEquipmentBehaviour[] array = componentsInChildren;
				foreach (AbstractEquipmentBehaviour abstractEquipmentBehaviour in array)
				{
					abstractEquipmentBehaviour.Equip(level);
				}
				this.level = level;
			}
		}

		public void OnUnequip()
		{
		}

		public List<UpgradeRequirements> GetUpgradeRequirements()
		{
			List<UpgradeRequirements> list = new List<UpgradeRequirements>();
			AbstractUpgradeBehaviour[] components = UpgradeBehaviuorParent.GetComponents<AbstractUpgradeBehaviour>();
			AbstractUpgradeBehaviour[] array = components;
			foreach (AbstractUpgradeBehaviour abstractUpgradeBehaviour in array)
			{
				list.Add(abstractUpgradeBehaviour.UpgradeRequirements(level + 1, this));
			}
			return list;
		}

		public void OnUpgrade(bool consumeRquired = false, Backpack backpack = null)
		{
			AbstractUpgradeBehaviour[] components = UpgradeBehaviuorParent.GetComponents<AbstractUpgradeBehaviour>();
			AbstractUpgradeBehaviour[] array = components;
			foreach (AbstractUpgradeBehaviour abstractUpgradeBehaviour in array)
			{
				if (consumeRquired)
				{
					backpack.ConsumeRequirements(abstractUpgradeBehaviour.UpgradeRequirements(level + 1, this));
				}
				abstractUpgradeBehaviour.OnUpgrade();
			}
			AbstractEquipmentBehaviour[] componentsInChildren = EquipmentBehavioursParent.GetComponentsInChildren<AbstractEquipmentBehaviour>();
			if (componentsInChildren != null && componentsInChildren.Length >= 1)
			{
				AbstractEquipmentBehaviour[] array2 = componentsInChildren;
				foreach (AbstractEquipmentBehaviour abstractEquipmentBehaviour in array2)
				{
					abstractEquipmentBehaviour.OnLevelChanged(level + 1, level);
				}
				level++;
			}
		}

		public bool CanBeUpgraded(Backpack backpack)
		{
			AbstractUpgradeBehaviour[] components = UpgradeBehaviuorParent.GetComponents<AbstractUpgradeBehaviour>();
			AbstractUpgradeBehaviour[] array = components;
			foreach (AbstractUpgradeBehaviour abstractUpgradeBehaviour in array)
			{
				if (!backpack.MeetRequirements(abstractUpgradeBehaviour.UpgradeRequirements(level + 1, this)))
				{
					return false;
				}
			}
			return true;
		}

		public void OnDrop(int amount, Vector3 position)
		{
			AbstractDropBehaviour[] components = DropBehaviourParent.GetComponents<AbstractDropBehaviour>();
			AbstractDropBehaviour.DropInfo dropInfo = default(AbstractDropBehaviour.DropInfo);
			dropInfo.itemDefinition = this;
			dropInfo.amount = amount;
			dropInfo.position = position;
			dropInfo.dropPrefab = Manager.Get<ItemsManager>().GetDropPrefab();
			AbstractDropBehaviour.DropInfo info = dropInfo;
			AbstractDropBehaviour[] array = components;
			foreach (AbstractDropBehaviour abstractDropBehaviour in array)
			{
				abstractDropBehaviour.Drop(info);
			}
		}

		public bool HasCustomPickupBehaviours()
		{
			AbstractPickupBehaviour[] components = DropBehaviourParent.GetComponents<AbstractPickupBehaviour>();
			return components.Length > 0;
		}

		public bool OnPickup(int amount, int level = 0)
		{
			AbstractPickupBehaviour[] components = DropBehaviourParent.GetComponents<AbstractPickupBehaviour>();
			int num = 0;
			bool flag = true;
			while (flag && num < components.Length)
			{
				flag = (flag && components[num].Pickup(amount, level));
				num++;
			}
			if (!flag)
			{
				while (num >= 0)
				{
					components[num].OnFailPickup(amount, level);
					num--;
				}
			}
			return flag;
		}
	}
}
