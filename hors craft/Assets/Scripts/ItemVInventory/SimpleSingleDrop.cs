// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.SimpleSingleDrop
using UnityEngine;

namespace ItemVInventory
{
	public class SimpleSingleDrop : AbstractDropBehaviour
	{
		public override void Drop(DropInfo info)
		{
			for (int i = 0; i < info.amount; i++)
			{
				Vector3 position = info.position + Random.insideUnitSphere * 1.3f;
				position.y += 2.5f;
				GameObject gameObject = Object.Instantiate(info.dropPrefab, position, Quaternion.identity);
				ItemDrop componentInChildren = gameObject.GetComponentInChildren<ItemDrop>();
				componentInChildren.Init(info.itemDefinition, 1);
			}
		}
	}
}
