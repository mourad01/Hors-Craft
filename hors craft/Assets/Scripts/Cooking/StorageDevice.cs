// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.StorageDevice
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class StorageDevice : Device
	{
		public Transform workerPlace;

		protected int slotsUnlocked = 1;

		public StorageDeviceSlot[] slots
		{
			get;
			protected set;
		}

		public List<IPickable> GetPlacedProducts()
		{
			return (from s in slots
				where s.placedItem != null
				select s.placedItem).ToList();
		}

		private void Awake()
		{
			slots = GetComponentsInChildren<StorageDeviceSlot>();
		}

		protected void UpdateSlotsAvaible()
		{
			bool flag = Unlocked();
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].gameObject.SetActive(flag && i < slotsUnlocked);
			}
		}

		protected override void SetUpgradeValues(UpgradeEffect effect, float value)
		{
			if (effect == UpgradeEffect.CAPACITY)
			{
				slotsUnlocked = (int)value;
				UpdateSlotsAvaible();
			}
		}
	}
}
