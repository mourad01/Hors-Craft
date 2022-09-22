// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.InPlaceSimpleDropBehaviour
using UnityEngine;

namespace ItemVInventory
{
	public class InPlaceSimpleDropBehaviour : AbstractDropBehaviour
	{
		public int div = 10;

		public override void Drop(DropInfo info)
		{
			for (int i = 0; i < div; i++)
			{
				Vector3 position = info.position + Random.insideUnitSphere * 1.3f;
				position.y += 2.5f;
				GameObject gameObject = Object.Instantiate(info.dropPrefab, position, Quaternion.identity);
				ItemDrop componentInChildren = gameObject.GetComponentInChildren<ItemDrop>();
				componentInChildren.Init(info.itemDefinition, info.amount / div);
			}
		}
	}
}
