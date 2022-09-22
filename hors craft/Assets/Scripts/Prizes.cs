// DecompilerFi decompiler from Assembly-CSharp.dll class: Prizes
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Quests/Prizes/Prizes")]
public class Prizes : ScriptableObject
{
	[SerializeField]
	protected List<PrizeContainer> prizes = new List<PrizeContainer>();

	public virtual void Grant(Action onGrant, int index = 0)
	{
		index = ((index >= 0) ? index : 0);
		foreach (PrizeContainer prize in prizes)
		{
			UnityEngine.Debug.Log("Granting...");
			prize.prize.Grant(null, prize.GetCorrectAmount(index));
		}
		onGrant?.Invoke();
	}
}
