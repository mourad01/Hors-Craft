// DecompilerFi decompiler from Assembly-CSharp.dll class: PrizeGlamour
using Common.Managers;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Prizes/xcraft.dressup.ios/Glamour")]
public class PrizeGlamour : PrizeBase
{
	public override void Grant(Action onGrant, int amount)
	{
		Manager.Get<ProgressManager>().IncreaseExperience(amount);
		base.Grant(onGrant, amount);
	}
}
