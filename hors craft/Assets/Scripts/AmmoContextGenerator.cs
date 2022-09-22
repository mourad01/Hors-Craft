// DecompilerFi decompiler from Assembly-CSharp.dll class: AmmoContextGenerator
using Common.Managers;
using Gameplay;
using System.Linq;
using UnityEngine;

public class AmmoContextGenerator : MonoBehaviour, IGameCallbacksListener
{
	private const string BASE_KEY = "ammoConfig.";

	public AmmoConfig[] ammoConfigs;

	private AmmoContext context;

	public void TakeAmmo(AmmoType ammoType, int value)
	{
		int ammoIndex = context.GetAmmoIndex(ammoType);
		if (ammoIndex != -1)
		{
			context.currentAmmo[ammoIndex] = Mathf.Max(0, context.currentAmmo[ammoIndex] - value);
			SurvivalContextsBroadcaster.instance.UpdateContext(context);
		}
	}

	public void AddAmmo(AmmoType ammoType, int value = -1)
	{
		int ammoIndex = context.GetAmmoIndex(ammoType);
		if (ammoIndex != -1)
		{
			if (context.maxAmmo[ammoIndex] < 0)
			{
				value = 3;
			}
			if (value < 0)
			{
				context.currentAmmo[ammoIndex] = context.maxAmmo[ammoIndex];
			}
			else
			{
				context.currentAmmo[ammoIndex] += value;
				context.currentAmmo[ammoIndex] = ((context.maxAmmo[ammoIndex] <= 0) ? context.currentAmmo[ammoIndex] : Mathf.Min(context.currentAmmo[ammoIndex], context.maxAmmo[ammoIndex]));
			}
			SurvivalContextsBroadcaster.instance.UpdateContext(context);
		}
	}

	public void SetAmmoType(AmmoType ammoType)
	{
		context.currentAmmoType = ammoType;
		SurvivalContextsBroadcaster.instance.UpdateContext(context);
	}

	public void OnGameSavedFrequent()
	{
		for (int i = 0; i < ammoConfigs.Length; i++)
		{
			string key = "ammoConfig." + ammoConfigs[i].ammoType.name;
			PlayerPrefs.SetInt(key, ammoConfigs[i].currentAmount);
		}
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayStarted()
	{
		InitContext();
	}

	public void OnGameplayRestarted()
	{
		Restart();
	}

	private void Start()
	{
		InitContext();
		SurvivalContextsBroadcaster.instance.UpdateContext(context);
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	private void InitContext()
	{
		if (context == null)
		{
			context = new AmmoContext();
		}
		context.ammoTypes = new AmmoType[ammoConfigs.Length];
		context.maxAmmo = new int[ammoConfigs.Length];
		context.currentAmmo = new int[ammoConfigs.Length];
		context.ammoPrefabs = (from config in ammoConfigs
			where config.prefab != null
			select config.prefab).ToArray();
		for (int i = 0; i < ammoConfigs.Length; i++)
		{
			string key = "ammoConfig." + ammoConfigs[i].ammoType.name;
			context.ammoTypes[i] = ammoConfigs[i].ammoType;
			context.maxAmmo[i] = ammoConfigs[i].maxAmount;
			context.currentAmmo[i] = ((!PlayerPrefs.HasKey(key)) ? ammoConfigs[i].currentAmount : PlayerPrefs.GetInt(key));
		}
	}

	private void Restart()
	{
		if (context != null)
		{
			for (int i = 0; i < ammoConfigs.Length; i++)
			{
				string key = "ammoConfig." + ammoConfigs[i].ammoType.name;
				if (PlayerPrefs.HasKey(key))
				{
					PlayerPrefs.DeleteKey(key);
				}
			}
		}
		InitContext();
	}
}
