// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.AbstractDropBehaviour
using UnityEngine;

namespace ItemVInventory
{
	public abstract class AbstractDropBehaviour : MonoBehaviour
	{
		public struct DropInfo
		{
			public ItemDefinition itemDefinition;

			public int amount;

			public Vector3 position;

			public GameObject dropPrefab;
		}

		public abstract void Drop(DropInfo info);
	}
}
