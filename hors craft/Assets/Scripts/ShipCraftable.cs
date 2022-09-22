// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipCraftable
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class ShipCraftable : MonoBehaviour, ICustomCraftingItem
{
	private const float SPAWN_CD = 150f;

	private static Dictionary<int, float> spawnCooldownTimer;

	public void OnCraftAction()
	{
	}

	public void OnUseAction(int id)
	{
		if (spawnCooldownTimer == null)
		{
			spawnCooldownTimer = new Dictionary<int, float>();
		}
		if (!CanSpawnCooldown(id))
		{
			CantSpawnCDToast(id);
			return;
		}
		GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
		if (gameObject.GetComponentInParent<Ship>() == null)
		{
			Ship[] source = UnityEngine.Object.FindObjectsOfType<Ship>();
			Ship ship = (from t in source
				where t.gameObject.name.Equals(base.gameObject.name)
				select t).FirstOrDefault();
			int num = 0;
			if (ship != null)
			{
				ship.dropCraftableAfterDestroy = false;
				num = (int)ship.GetComponentInParent<Health>().hp;
				UnityEngine.Object.Destroy(ship.GetComponentInParent<Health>().gameObject);
				Singleton<PlayerData>.get.playerItems.AddCraftable(id, 1);
			}
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			PlayerMovement componentInParent = gameObject.GetComponentInParent<PlayerMovement>();
			GameObject gameObject2 = componentInParent.SpawnAndMountVehicle(base.gameObject.GetComponentInChildren<VehicleController>());
			if (gameObject2 == null)
			{
				string text = Manager.Get<TranslationsManager>().GetText("ship.spawn.on.land", "You must be in water to use ship").ToUpper();
				Manager.Get<ToastManager>().ShowToast(text);
				return;
			}
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.UNDERWATER, enabled: false);
			UpdateCD(id);
			if (num > 0)
			{
				gameObject2.GetComponentInChildren<Health>().hp = num;
			}
			if (WorldPlayerPrefs.get.HasKey("ship" + id))
			{
				float @float = WorldPlayerPrefs.get.GetFloat("ship" + id);
				WorldPlayerPrefs.get.DeleteKey("ship" + id);
				gameObject2.GetComponentInParent<Health>().hp = @float;
			}
		}
		else
		{
			string text2 = Manager.Get<TranslationsManager>().GetText("has.ship.already", "Leave your current ship first!").ToUpper();
			Manager.Get<ToastManager>().ShowToast(text2);
		}
	}

	private void UpdateCD(int id)
	{
		if (spawnCooldownTimer.ContainsKey(id))
		{
			spawnCooldownTimer[id] = Time.time + 150f;
		}
		else
		{
			spawnCooldownTimer.Add(id, Time.time + 150f);
		}
	}

	private void CantSpawnCDToast(int id)
	{
		float num = spawnCooldownTimer[id] - Time.time;
		string text = Manager.Get<TranslationsManager>().GetText("ship.spawn.colldown", "You must wait {0}sec to use this ship");
		Manager.Get<ToastManager>().ShowToast(string.Format(text, num.ToString("f0")));
	}

	private bool CanSpawnCooldown(int id)
	{
		if (spawnCooldownTimer.ContainsKey(id) && spawnCooldownTimer[id] > Time.time)
		{
			return false;
		}
		return true;
	}
}
