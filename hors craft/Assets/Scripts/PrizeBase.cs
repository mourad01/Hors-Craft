// DecompilerFi decompiler from Assembly-CSharp.dll class: PrizeBase
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Quests/Prizes/Base Class")]
public class PrizeBase : ScriptableObject
{
	[SerializeField]
	protected QuestPrizeType prizeType;

	public virtual QuestPrizeType PrizeType => prizeType;

	public virtual void Grant(Action onGrant, int amount)
	{
		if (onGrant == null)
		{
			UnityEngine.Debug.LogError("Cannot grant!");
		}
		else
		{
			onGrant();
		}
	}
}
