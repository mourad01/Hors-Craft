// DecompilerFi decompiler from Assembly-CSharp.dll class: PrizeCash
using Common.Managers;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Prizes/xcraft.dressup.ios/Cash")]
public class PrizeCash : PrizeBase
{
	public override void Grant(Action onGrant, int amount)
	{
		Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(amount);
		base.Grant(onGrant, amount);
	}
}
