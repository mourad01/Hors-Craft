// DecompilerFi decompiler from Assembly-CSharp.dll class: AmmoDisplayer
using System;
using UnityEngine;

public class AmmoDisplayer : MonoBehaviour, ISurvivalContextListener
{
	public bool showAmmoWhenNotFhightTime;

	private SurvivalAmmoType ammoTypeContext;

	private AmmoContext _ammoContext;

	private AmmoContext ammoContext
	{
		get
		{
			if (_ammoContext == null)
			{
				_ammoContext = SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>();
			}
			return _ammoContext;
		}
	}

	private void Start()
	{
		ammoTypeContext = (MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalAmmoType>(Fact.SURVIVAL_AMMO_TYPE) ?? new SurvivalAmmoType());
	}

	public void OnContextsUpdated()
	{
		if (ammoContext == null)
		{
			return;
		}
		if (ammoContext.currentAmmoType == null)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.SURVIVAL_AMMO_TYPE);
			return;
		}
		AmmoContext.AmmoPair ammoForType = ammoContext.GetAmmoForType(ammoContext.currentAmmoType);
		ammoTypeContext.ammoCount = ammoForType.currentAmmo;
		ammoTypeContext.ammoMax = ammoForType.maxAmmo;
		ammoTypeContext.ammoIco = ammoContext.currentAmmoType.icon;
		SurvivalPhaseContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
		if (factContext == null || !factContext.isCombat)
		{
			ammoTypeContext.show = showAmmoWhenNotFhightTime;
		}
		else
		{
			ammoTypeContext.show = true;
		}
		if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_AMMO_TYPE, ammoTypeContext))
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_AMMO_TYPE);
		}
	}

	public bool AddBySurvivalManager()
	{
		return true;
	}

	public Type[] ContextTypes()
	{
		return new Type[1]
		{
			typeof(AmmoContext)
		};
	}

	Action[] ISurvivalContextListener.OnContextsUpdated()
	{
		return new Action[1]
		{
			OnContextsUpdated
		};
	}
}
