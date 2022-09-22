// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ItemReward
using Common.Gameplay;
using ItemVInventory;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class ItemReward : Reward
	{
		public ItemDefinition item;

		public bool dropOnWorld;

		public GameObject itemDropPrefab;

		public Vector3 dropPlace = Vector3.zero;

		public override void ClaimReward()
		{
			if (dropOnWorld)
			{
				GameObject gameObject = Object.Instantiate(itemDropPrefab, dropPlace, Quaternion.identity);
				ItemDrop componentInChildren = gameObject.GetComponentInChildren<ItemDrop>();
				componentInChildren.Init(item, amount);
				return;
			}
			GameObject gameObject2 = PlayerGraphic.GetControlledPlayerInstance().gameObject;
			Backpack componentInChildren2 = gameObject2.GetComponentInChildren<Backpack>();
			Equipment componentInChildren3 = gameObject2.GetComponentInChildren<Equipment>();
			for (int i = 0; i < amount; i++)
			{
				if (!componentInChildren2.TryPlace(item))
				{
					componentInChildren3.Equip(item, item.level);
				}
			}
		}

		public override List<Sprite> GetSprites()
		{
			return item.itemSprite.AsList();
		}
	}
}
