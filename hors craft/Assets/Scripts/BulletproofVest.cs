// DecompilerFi decompiler from Assembly-CSharp.dll class: BulletproofVest
using Common.Managers;
using UnityEngine;

public class BulletproofVest : MonoBehaviour, ICustomCraftingItem
{
	public float armorValue = 3f;

	public void OnCraftAction()
	{
	}

	public void OnUseAction(int id)
	{
		Health componentInParent = PlayerGraphic.GetControlledPlayerInstance().transform.GetComponentInParent<Health>();
		Armor component = componentInParent.gameObject.GetComponent<Armor>();
		if (component == null)
		{
			component = componentInParent.gameObject.AddComponent<Armor>();
			component.Init(armorValue);
			Singleton<PlayerData>.get.playerItems.AddCraftable(id, -1);
			Manager.Get<StateMachineManager>().PopState();
		}
		else
		{
			string text = Manager.Get<TranslationsManager>().GetText("bulletproof.already", "You already have this item in use!").ToUpper();
			Manager.Get<ToastManager>().ShowToast(text);
		}
	}
}
