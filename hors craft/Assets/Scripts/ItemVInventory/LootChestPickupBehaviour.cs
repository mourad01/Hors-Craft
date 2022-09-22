// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.LootChestPickupBehaviour
using Common.Managers;
using Gameplay;

namespace ItemVInventory
{
	public class LootChestPickupBehaviour : AbstractPickupBehaviour
	{
		public LootChestManager.Rarity rarity;

		public override bool Pickup(int amount, int level)
		{
			LootChest chest = Manager.Get<LootChestManager>().GenerateChest(rarity);
			Manager.Get<LootChestManager>().OpenChest(chest);
			return true;
		}

		public override void OnFailPickup(int amount, int level)
		{
		}
	}
}
