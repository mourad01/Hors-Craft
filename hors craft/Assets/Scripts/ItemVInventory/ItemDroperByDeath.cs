// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.ItemDroperByDeath
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ItemVInventory
{
	public class ItemDroperByDeath : MonoBehaviour
	{
		public int monsterIndex;

		public int tier;

		private void Start()
		{
			Health componentInChildren = GetComponentInChildren<Health>();
			Health health = componentInChildren;
			health.onDieAction = (Action)Delegate.Combine(health.onDieAction, new Action(Drop));
		}

		private void Drop()
		{
			ItemsManager itemsManager = Manager.Get<ItemsManager>();
			List<ItemsManager.DropConfig> dropConfigs = itemsManager.TryGetDropConfig(base.gameObject, monsterIndex, tier);
			itemsManager.Drop(dropConfigs, base.transform.position);
		}
	}
}
