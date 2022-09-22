// DecompilerFi decompiler from Assembly-CSharp.dll class: DebugLog
using Common.BehaviourTrees;
using UnityEngine;

public class DebugLog : MobNode
{
	private string msg;

	public DebugLog(Mob mob, string message)
		: base(mob)
	{
		msg = message;
	}

	public override void Update()
	{
		UnityEngine.Debug.LogWarning(msg);
		base.status = Status.SUCCESS;
	}
}
