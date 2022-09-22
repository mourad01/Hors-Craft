// DecompilerFi decompiler from Assembly-CSharp.dll class: ParticleEmitterBasedOnVelocity
using UnityEngine;

public class ParticleEmitterBasedOnVelocity : MonoBehaviour
{
	private const float SPEED_TO_PLAY_FULL_VOLUME = 6f;

	private const float SPEED_TO_STOP_ENGINE_SOUNDS = 3f;

	private Rigidbody body;

	private VehicleController controller;

	[SerializeField]
	private ParticleSystem particleSystemToControl;

	private float maxEmissionRate;

	private void Awake()
	{
		if (particleSystemToControl != null)
		{
			maxEmissionRate = particleSystemToControl.emission.rate.constant;
		}
		body = GetComponent<Rigidbody>();
		controller = GetComponent<VehicleController>();
	}

	private void FixedUpdate()
	{
		UpdateEngineSounds();
	}

	private void UpdateEngineSounds()
	{
		Vector3 velocity = body.velocity;
		int num;
		if (Mathf.Abs(velocity.x) < 3f)
		{
			Vector3 velocity2 = body.velocity;
			if (Mathf.Abs(velocity2.z) < 3f)
			{
				num = 1;
				goto IL_0069;
			}
		}
		num = ((controller != null && !controller.IsInUse) ? 1 : 0);
		goto IL_0069;
		IL_0069:
		if (num != 0)
		{
			if (particleSystemToControl.isPlaying)
			{
				particleSystemToControl.Stop();
			}
		}
		else if (!particleSystemToControl.isPlaying)
		{
			particleSystemToControl.Play();
		}
		float magnitude = body.velocity.magnitude;
		//particleSystemToControl.emission.rate = ((!(magnitude < 6f)) ? ((ParticleSystem.MinMaxCurve)maxEmissionRate) : ((ParticleSystem.MinMaxCurve)(magnitude / 6f * maxEmissionRate)));
	}
}
