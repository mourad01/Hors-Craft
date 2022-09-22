// DecompilerFi decompiler from Assembly-CSharp.dll class: MinigameEntrence
using Common.Managers;
using Uniblocks;
using UnityEngine;

public class MinigameEntrence : InteractiveObject
{
	[SerializeField]
	private string levelName;

	public override void OnUse()
	{
		base.OnUse();
		Manager.Get<MinigamesManager>().PlayLevel(levelName);
	}
}
