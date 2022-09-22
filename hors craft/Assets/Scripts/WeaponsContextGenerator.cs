// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponsContextGenerator
using Gameplay;
using System.Linq;
using UnityEngine;
using UnityToolbag;

public class WeaponsContextGenerator : MonoBehaviour, IGameCallbacksListener
{
	private const string GENERATOR_KEY = "WeaponsContextGenerator";

	private WeaponsContext _context;

	[Reorderable]
	public WeaponConfig[] weaponsConfigs;

	private void Start()
	{
		InitContext();
		SurvivalContextsBroadcaster.instance.UpdateContext(_context);
	}

	private void InitContext()
	{
		_context = new WeaponsContext
		{
			weaponsConfigs = (from config in weaponsConfigs
				where config.prefab.GetComponent<Weapon>() != null
				select config).ToArray()
		};
		for (int i = 0; i < _context.weaponsConfigs.Length; i++)
		{
			weaponsConfigs[i].claimed = (_context.weaponsConfigs[i].isFree || PlayerPrefs.GetInt("weaponConfig." + i, 0) == 1);
			Weapon component = _context.weaponsConfigs[i].prefab.GetComponent<Weapon>();
			WeaponHandler weaponHandler = component as WeaponHandler;
			if (weaponHandler != null)
			{
				if (_context.weaponsConfigs[i].maxMagAmmoAmount > 0)
				{
					weaponHandler.magazineBulletsSize = _context.weaponsConfigs[i].maxMagAmmoAmount;
				}
				weaponHandler.isUnlockedAtStart = weaponsConfigs[i].claimed;
			}
			if (component is IAmmoWeapon)
			{
				((IAmmoWeapon)component).SetAmmoType(_context.weaponsConfigs[i].ammoType);
			}
		}
	}

	private void SaveContext()
	{
		if (_context != null)
		{
			PlayerPrefs.SetInt("WeaponsContextGenerator", 1);
			for (int i = 0; i < _context.weaponsConfigs.Length; i++)
			{
				PlayerPrefs.SetInt("weaponsConfigs." + i, weaponsConfigs[i].claimed ? 1 : 0);
			}
		}
	}

	private void ResetContext()
	{
		for (int i = 0; i < _context.weaponsConfigs.Length; i++)
		{
			PlayerPrefs.SetInt("weaponsConfig." + i, 0);
		}
	}

	public void OnGameSavedFrequent()
	{
		SaveContext();
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
		ResetContext();
	}
}
