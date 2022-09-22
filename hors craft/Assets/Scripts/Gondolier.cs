// DecompilerFi decompiler from Assembly-CSharp.dll class: Gondolier
using UnityEngine;

public class Gondolier : MonoBehaviour
{
	private const string SPEED = "speed";

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Boat boat;

	[SerializeField]
	private Rigidbody body;

	private float maxSpeed;

	private void Start()
	{
		maxSpeed = boat.MaxVelocity;
	}

	private void Update()
	{
		if (!boat.IsInUse)
		{
			animator.SetFloat("speed", 0f);
			return;
		}
		float value = body.velocity.magnitude / maxSpeed;
		animator.SetFloat("speed", value);
	}
}
