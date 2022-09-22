// DecompilerFi decompiler from Assembly-CSharp.dll class: Looting
using Uniblocks;
using UnityEngine;

public class Looting : MonoBehaviour
{
	public delegate void OnCollect(CollectibleVoxel.Type type);

	public OnCollect onCollectAction;

	private Inventory inventory;

	private void Start()
	{
		inventory = GetComponent<Inventory>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		CollectibleVoxel component = collider.gameObject.GetComponent<CollectibleVoxel>();
		if (component != null)
		{
			CollectLoot(component);
		}
	}

	private void CollectLoot(CollectibleVoxel collectible)
	{
		if (collectible.type == CollectibleVoxel.Type.INVENTORY_ITEM)
		{
			LootToInventory(collectible);
		}
		else if (collectible.type == CollectibleVoxel.Type.HEALTH)
		{
			LootHealth(collectible);
		}
		else if (collectible.type == CollectibleVoxel.Type.FOOD)
		{
			LootFood(collectible);
		}
	}

	private void LootToInventory(CollectibleVoxel collectible)
	{
		if (inventory != null && inventory.GetQuantity(collectible.id) < 64)
		{
			inventory.AddItem(collectible.id, collectible.quantity);
			collectible.OnCollected();
			onCollectAction(collectible.type);
		}
	}

	private void LootHealth(CollectibleVoxel collectible)
	{
		Health componentInParent = GetComponentInParent<Health>();
		if (componentInParent != null && componentInParent.hp < componentInParent.maxHp)
		{
			componentInParent.hp += collectible.quantity;
			collectible.OnCollected();
			onCollectAction(collectible.type);
		}
	}

	private void LootFood(CollectibleVoxel collectible)
	{
		Hunger componentInParent = GetComponentInParent<Hunger>();
		if (componentInParent != null && componentInParent.feedLevel < componentInParent.maxFeed)
		{
			componentInParent.Feed(collectible.quantity);
			collectible.OnCollected();
			onCollectAction(collectible.type);
		}
	}
}
