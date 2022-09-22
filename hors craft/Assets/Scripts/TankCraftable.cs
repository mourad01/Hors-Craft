// DecompilerFi decompiler from Assembly-CSharp.dll class: TankCraftable
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class TankCraftable : MonoBehaviour, ICustomCraftingItem
{
	[SerializeField]
	private float spawnCd = 150f;

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
		if (gameObject.GetComponentInParent<Tank>() == null)
		{
			Tank[] source = UnityEngine.Object.FindObjectsOfType<Tank>();
			Tank tank = source.FirstOrDefault((Tank t) => t.gameObject.name.Equals(base.gameObject.name));
			if (tank != null)
			{
				UnityEngine.Object.Destroy(tank.GetComponentInParent<Health>().gameObject);
				Singleton<PlayerData>.get.playerItems.AddCraftable(id, 1);
			}
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			PlayerMovement componentInParent = gameObject.GetComponentInParent<PlayerMovement>();
			GameObject gameObject2 = componentInParent.SpawnAndMountVehicle(base.gameObject.GetComponentInChildren<VehicleController>());
			UpdateCD(id);
			if (WorldPlayerPrefs.get.HasKey("tank" + id))
			{
				float @float = WorldPlayerPrefs.get.GetFloat("tank" + id);
				WorldPlayerPrefs.get.DeleteKey("tank" + id);
				gameObject2.GetComponentInParent<Health>().hp = @float;
			}
		}
		else
		{
			string text = Manager.Get<TranslationsManager>().GetText("has.tank.already", "Leave your current tank first!").ToUpper();
			Manager.Get<ToastManager>().ShowToast(text);
		}
	}

	private void UpdateCD(int id)
	{
		if (spawnCooldownTimer.ContainsKey(id))
		{
			spawnCooldownTimer[id] = Time.time + spawnCd;
		}
		else
		{
			spawnCooldownTimer.Add(id, Time.time + spawnCd);
		}
	}

	private void CantSpawnCDToast(int id)
	{
		float num = spawnCooldownTimer[id] - Time.time;
		string text = Manager.Get<TranslationsManager>().GetText("tank.spawn.colldown", "You must wait {0}sec to use this tank");
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
