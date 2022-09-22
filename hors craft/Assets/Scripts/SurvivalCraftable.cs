// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalCraftable
using Common.Managers;
using States;
using Uniblocks;
using UnityEngine;

public class SurvivalCraftable : MonoBehaviour, ICustomCraftingItem
{
	public string Name;

	protected GameObject usedObject;

	public void OnCraftAction()
	{
	}

	public void OnUseAction(int id)
	{
		AbstractSurvivalCraftableUse[] components = GetComponents<AbstractSurvivalCraftableUse>();
		for (int i = 0; i < components.Length; i++)
		{
			if (!components[i].CanBeUse(id))
			{
				Failed(id, i, components);
				return;
			}
		}
		for (int j = 0; j < components.Length; j++)
		{
			components[j].PrepareToUse(id);
		}
		Use(id);
		for (int k = 0; k < components.Length; k++)
		{
			components[k].OnSuccess(id, usedObject);
		}
	}

	protected virtual void Use(int id)
	{
		Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		PlayerMovement componentInParent = PlayerGraphic.GetControlledPlayerInstance().gameObject.GetComponentInParent<PlayerMovement>();
		componentInParent.SpawnAndMountVehicle(base.gameObject.GetComponentInChildren<VehicleController>());
	}

	protected void Failed(int id, int index, AbstractSurvivalCraftableUse[] behaviors)
	{
		for (int num = index; num > 0; num--)
		{
			behaviors[num].OnFailed(id);
		}
	}
}
