// DecompilerFi decompiler from Assembly-CSharp.dll class: PlaneCraftable
using Common.Managers;
using States;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class PlaneCraftable : MonoBehaviour, ICustomCraftingItem
{
	public void OnCraftAction()
	{
	}

	public void OnUseAction(int id)
	{
		GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
		if (gameObject.GetComponentInParent<PlaneController>() == null)
		{
			PlaneController[] source = UnityEngine.Object.FindObjectsOfType<PlaneController>();
			PlaneController planeController = (from t in source
				where t.gameObject.name.Equals(base.gameObject.name)
				select t).FirstOrDefault();
			int num = 0;
			if (planeController != null)
			{
				num = (int)planeController.GetComponentInParent<Health>().hp;
				UnityEngine.Object.Destroy(planeController.GetComponentInParent<Health>().gameObject);
				Singleton<PlayerData>.get.playerItems.AddCraftable(id, 1);
			}
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			PlayerMovement componentInParent = gameObject.GetComponentInParent<PlayerMovement>();
			GameObject gameObject2 = componentInParent.SpawnAndMountVehicle(base.gameObject.GetComponentInChildren<VehicleController>());
			if (num > 0)
			{
				gameObject2.GetComponentInChildren<Health>().hp = num;
			}
			if (WorldPlayerPrefs.get.HasKey("plane" + id))
			{
				float @float = WorldPlayerPrefs.get.GetFloat("plane" + id);
				WorldPlayerPrefs.get.DeleteKey("plane" + id);
				gameObject2.GetComponentInParent<Health>().hp = @float;
			}
		}
		else
		{
			string text = Manager.Get<TranslationsManager>().GetText("has.plane.already", "Leave your current plane first!").ToUpper();
			Manager.Get<ToastManager>().ShowToast(text);
		}
	}
}
