// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerHUD
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour, IFactChangedListener
{
	private SurvivalHealth healthContext;

	private SurvivalArmor armorContext;

	private void Start()
	{
		healthContext = new SurvivalHealth();
		armorContext = new SurvivalArmor();
		AssainPlayerHealth();
		MonoBehaviourSingleton<GameplayFacts>.get.RegisterFactChangedListener(this, Fact.IN_VEHICLE);
	}

	private void UpdateHealth()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_HEALTH);
	}

	private void ArmorChanged(Armor armor)
	{
		if (armor != null)
		{
			armorContext.armorComponent = armor;
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_ARMOR, armorContext))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_ARMOR);
			}
		}
		else
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.SURVIVAL_ARMOR);
		}
	}

	public void OnFactsChanged(HashSet<Fact> facts)
	{
		SurvivalVehicleContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalVehicleContext>(Fact.IN_VEHICLE);
		if (factContext != null && healthContext.healthComponent != null)
		{
			RemoveListeners();
			healthContext.healthComponent = factContext.health;
			Health healthComponent = healthContext.healthComponent;
			healthComponent.onHpChangeAction = (Health.DoOnHpChange)Delegate.Combine(healthComponent.onHpChangeAction, new Health.DoOnHpChange(UpdateHealth));
			Health healthComponent2 = healthContext.healthComponent;
			healthComponent2.onArmorChanged = (Action<Armor>)Delegate.Combine(healthComponent2.onArmorChanged, new Action<Armor>(ArmorChanged));
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_HEALTH, healthContext))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_HEALTH);
			}
			ArmorChanged(null);
		}
		else
		{
			RemoveListeners();
			AssainPlayerHealth();
		}
	}

	private void AssainPlayerHealth()
	{
		Health componentInChildren = GetComponentInChildren<Health>();
		if (componentInChildren != null)
		{
			healthContext.healthComponent = componentInChildren;
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_HEALTH, healthContext))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_HEALTH);
			}
			ArmorChanged(healthContext.healthComponent.armor);
			Health health = componentInChildren;
			health.onHpChangeAction = (Health.DoOnHpChange)Delegate.Combine(health.onHpChangeAction, new Health.DoOnHpChange(UpdateHealth));
			Health health2 = componentInChildren;
			health2.onArmorChanged = (Action<Armor>)Delegate.Combine(health2.onArmorChanged, new Action<Armor>(ArmorChanged));
		}
	}

	private void RemoveListeners()
	{
		if (!(healthContext.healthComponent == null))
		{
			Health healthComponent = healthContext.healthComponent;
			healthComponent.onHpChangeAction = (Health.DoOnHpChange)Delegate.Remove(healthComponent.onHpChangeAction, new Health.DoOnHpChange(UpdateHealth));
			Health healthComponent2 = healthContext.healthComponent;
			healthComponent2.onArmorChanged = (Action<Armor>)Delegate.Remove(healthComponent2.onArmorChanged, new Action<Armor>(ArmorChanged));
		}
	}

	private void OnDestroy()
	{
		if (MonoBehaviourSingleton<GameplayFacts>.get != null)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.UnregisterFactChangedListener(this, Fact.IN_VEHICLE);
		}
		RemoveListeners();
	}
}
