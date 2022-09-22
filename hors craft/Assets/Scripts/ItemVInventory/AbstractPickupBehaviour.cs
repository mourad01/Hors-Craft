// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.AbstractPickupBehaviour
using UnityEngine;

namespace ItemVInventory
{
	public abstract class AbstractPickupBehaviour : MonoBehaviour
	{
		public abstract bool Pickup(int amount, int level);

		public abstract void OnFailPickup(int amount, int level);
	}
}
