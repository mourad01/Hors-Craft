// DecompilerFi decompiler from Assembly-CSharp.dll class: CooldownNode
using Common.BehaviourTrees;
using UnityEngine;

public class CooldownNode : Node
{
	private float endTime;

	private bool autoSetCd;

	private float cd;

	public CooldownNode(bool autoSetCooldown = false, float cooldown = 0f)
	{
		endTime = 0f;
		autoSetCd = autoSetCooldown;
		cd = cooldown;
	}

	public void SetCooldown(float cd)
	{
		endTime = Time.time + cd;
	}

	public override void Update()
	{
		base.status = ((!(Time.time > endTime)) ? Status.FAILURE : Status.SUCCESS);
		if (autoSetCd)
		{
			SetCooldown(cd);
		}
	}
}
