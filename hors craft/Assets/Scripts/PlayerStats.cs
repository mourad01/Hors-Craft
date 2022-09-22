// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerStats
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "ScriptableObjects/ItemVInventory/PlayerStat")]
public class PlayerStats : ScriptableObject
{
	public class Modifier
	{
		public float value;

		public int priority = 100;

		public Func<float, float, float> Action;

		public float DoAction(float oldValue)
		{
			return Action(value, oldValue);
		}

		public bool Equals(Modifier other)
		{
			return value == other.value && priority == other.priority;
		}
	}

	protected Action onApply;

	protected List<Modifier> modifiers = new List<Modifier>();

	public void Register(Action onApplyAction)
	{
		onApply = (Action)Delegate.Remove(onApply, onApplyAction);
		onApply = (Action)Delegate.Combine(onApply, onApplyAction);
	}

	public void Unregister(Action onApplyAction)
	{
		onApply = (Action)Delegate.Remove(onApply, onApplyAction);
	}

	public void Add(Modifier modifier)
	{
		if (!modifiers.Contains(modifier))
		{
			modifiers.Add(modifier);
			modifiers.Sort((Modifier x, Modifier y) => x.priority.CompareTo(y.priority));
		}
		if (onApply != null)
		{
			onApply();
		}
	}

	public void Remove(Modifier modifier)
	{
		int num = modifiers.FindIndex((Modifier mod) => mod.Equals(modifier));
		if (num != -1)
		{
			modifiers.RemoveAt(num);
		}
	}

	public void RemoveAll()
	{
		modifiers = new List<Modifier>();
	}

	public float GetStats()
	{
		float num = 0f;
		foreach (Modifier modifier in modifiers)
		{
			num = modifier.DoAction(num);
		}
		return num;
	}

	public void Reset()
	{
		modifiers.Clear();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("Modifiers count: ");
		stringBuilder.AppendLine(modifiers.Count.ToString());
		float oldValue = 0f;
		foreach (Modifier modifier in modifiers)
		{
			oldValue = modifier.DoAction(oldValue);
			stringBuilder.AppendLine(oldValue.ToString());
		}
		return stringBuilder.ToString();
	}
}
