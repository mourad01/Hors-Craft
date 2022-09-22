// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimatorBoolBasedOnVelocity
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AnimatorBoolBasedOnVelocity : MonoBehaviour
{
	private Rigidbody body;

	private VehicleController controller;

	public float speedToActivateBool = 3f;

	public float speedToDeactivateBool = 0.5f;

	public string boolName;

	private int boolId;

	public Animator[] targets;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		controller = GetComponent<VehicleController>();
		boolId = Animator.StringToHash(boolName);
	}

	private void FixedUpdate()
	{
		Animator[] array = targets;
		foreach (Animator animator in array)
		{
			UpdateAnimator(animator);
		}
	}

	private void UpdateAnimator(Animator animator)
	{
		Vector3 velocity = body.velocity;
		int num;
		if (Mathf.Abs(velocity.x) < speedToDeactivateBool)
		{
			Vector3 velocity2 = body.velocity;
			if (Mathf.Abs(velocity2.z) < speedToDeactivateBool)
			{
				num = 1;
				goto IL_006b;
			}
		}
		num = ((controller != null && !controller.IsInUse) ? 1 : 0);
		goto IL_006b;
		IL_006b:
		bool flag = (byte)num != 0;
		animator.SetBool(boolId, !flag);
	}
}
