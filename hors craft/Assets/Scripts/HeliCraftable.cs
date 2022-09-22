// DecompilerFi decompiler from Assembly-CSharp.dll class: HeliCraftable
using Common.Managers;
using States;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class HeliCraftable : MonoBehaviour, ICustomCraftingItem
{
	public void OnCraftAction()
	{
	}

	public void OnUseAction(int id)
	{
		GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
		if (gameObject.GetComponentInParent<HeliController>() == null)
		{
			HeliController[] source = UnityEngine.Object.FindObjectsOfType<HeliController>();
			HeliController heliController = (from t in source
				where t.gameObject.name.Equals(base.gameObject.name)
				select t).FirstOrDefault();
			int num = 0;
			if (heliController != null)
			{
				num = (int)heliController.GetComponentInParent<Health>().hp;
				UnityEngine.Object.Destroy(heliController.GetComponentInParent<Health>().gameObject);
				Singleton<PlayerData>.get.playerItems.AddCraftable(id, 1);
			}
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			PlayerMovement componentInParent = gameObject.GetComponentInParent<PlayerMovement>();
			GameObject gameObject2 = componentInParent.SpawnAndMountVehicle(base.gameObject.GetComponentInChildren<VehicleController>());
			if (num > 0)
			{
				gameObject2.GetComponentInChildren<Health>().hp = num;
			}
			if (WorldPlayerPrefs.get.HasKey("heli" + id))
			{
				float @float = WorldPlayerPrefs.get.GetFloat("heli" + id);
				WorldPlayerPrefs.get.DeleteKey("heli" + id);
				gameObject2.GetComponentInParent<Health>().hp = @float;
			}
		}
		else
		{
			string text = Manager.Get<TranslationsManager>().GetText("has.heli.already", "Leave your current helicopter first!").ToUpper();
			Manager.Get<ToastManager>().ShowToast(text);
		}
	}
}
