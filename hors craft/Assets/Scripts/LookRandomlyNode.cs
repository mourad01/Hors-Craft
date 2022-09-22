// DecompilerFi decompiler from Assembly-CSharp.dll class: LookRandomlyNode
using Common.BehaviourTrees;
using UnityEngine;

public class LookRandomlyNode : MobNode
{
	private float timeOffset;

	private float lastLookTime;

	private float interval;

	private const float LOOK_RANDOMLY_INTERVAL_FROM = 2f;

	private const float LOOK_RANDOMLY_INTERVAL_TO = 5f;

	public LookRandomlyNode(Mob mob)
		: base(mob)
	{
		interval = Random.Range(2f, 5f);
		timeOffset = Random.Range(0f, interval);
		lastLookTime = Time.time + timeOffset;
	}

	public override void Update()
	{
		if (Time.time > lastLookTime + interval)
		{
			float min = -1f;
			float max = 1f;
			float min2 = 1f;
			float max2 = 1f;
			float min3 = -0.3f;
			float max3 = 0.3f;
			Vector3 point = new Vector3(Random.Range(min, max), Random.Range(min3, max3), Random.Range(min2, max2));
			point = Quaternion.LookRotation(base.mob.transform.forward) * point;
			base.mob.LookAt(base.mob.lookConfig.headTransform.position + point);
			lastLookTime = Time.time;
		}
		base.status = Status.SUCCESS;
	}
}
