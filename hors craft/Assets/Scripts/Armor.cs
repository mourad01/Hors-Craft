// DecompilerFi decompiler from Assembly-CSharp.dll class: Armor
using System;
using UnityEngine;

public class Armor : MonoBehaviour
{
	public PlayerStats stat;

	public Action onArmorChanged;

	private float _maxValue;

	private float _armorValue;

	public float maxValue
	{
		get
		{
			return _maxValue;
		}
		set
		{
			_maxValue = value;
			if (onArmorChanged != null)
			{
				onArmorChanged();
			}
		}
	}

	[HideInInspector]
	public float armorValue
	{
		get
		{
			return _armorValue;
		}
		set
		{
			_armorValue = value;
			if (onArmorChanged != null)
			{
				onArmorChanged();
			}
		}
	}

	public void Init(float arm)
	{
		_maxValue = arm;
		_armorValue = arm;
		Health component = GetComponent<Health>();
		if (component != null)
		{
			component.armor = this;
			onArmorChanged = component.ArmorChanged;
		}
		if (stat != null)
		{
			stat.Add(new PlayerStats.Modifier
			{
				value = arm,
				priority = 0,
				Action = ((float toAction, float value) => value + toAction)
			});
			stat.Register(OnStatsChanged);
		}
	}

	public virtual float TakeDamage(float damage)
	{
		armorValue -= damage;
		return (!(armorValue < 0f)) ? 0f : (armorValue * -1f);
	}

	public virtual void TryDestroy()
	{
		if (armorValue <= 0f)
		{
			Health component = GetComponent<Health>();
			if (component != null && component.armor == this)
			{
				component.armor = null;
				onArmorChanged();
			}
			UnityEngine.Object.Destroy(this);
		}
	}

	private void OnStatsChanged()
	{
		float num = stat.GetStats() - maxValue;
		maxValue = stat.GetStats();
		armorValue += num;
	}
}
