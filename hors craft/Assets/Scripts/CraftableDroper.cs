// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftableDroper
using Common.Managers;
using System.Linq;
using UnityEngine;

public class CraftableDroper : MonoBehaviour
{
	public bool hasToDropCraftable = true;

	private Health _health;

	private Health health => _health ?? (_health = GetComponentInChildren<Health>());

	private void OnDestroy()
	{
		CraftableList craftableListInstance = Manager.Get<CraftingManager>().GetCraftableListInstance();
		Craftable craftable = craftableListInstance.craftableList.FirstOrDefault((Craftable c) => c.customCraftableObject != null && base.gameObject.name.Contains(c.customCraftableObject.name));
		if (hasToDropCraftable && health != null && !health.IsDead() && craftable != null)
		{
			Manager.Get<CraftingManager>().SpawnCustomCraftable(base.transform.position + Vector3.up * 2f, craftable);
			PlayerPrefs.SetFloat(GetComponent<SurvivalCraftable>().Name + craftable.id, health.hp);
		}
		if (Singleton<PlayerData>.get != null && health.IsDead())
		{
			Singleton<PlayerData>.get.playerItems.AddCraftable(craftable.id, -1);
		}
	}
}
