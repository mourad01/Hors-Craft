// DecompilerFi decompiler from Assembly-CSharp.dll class: EngineSoundsBasedOnVelocity
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EngineSoundsBasedOnVelocity : MonoBehaviour
{
	private const float SPEED_TO_PLAY_FULL_VOLUME = 3f;

	private const float SPEED_TO_STOP_ENGINE_SOUNDS = 0.5f;

	private const float MAX_PITCH_CHANGE = 0.3f;

	public AudioSource engineSound;

	public float speedToMaxPitch = 10f;

	private Rigidbody body;

	private VehicleController controller;

	private void Awake()
	{
		if (engineSound == null)
		{
			engineSound = GetComponent<AudioSource>();
		}
		body = GetComponent<Rigidbody>();
		controller = GetComponent<VehicleController>();
		PrepareEngineSounds();
	}

	private void PrepareEngineSounds()
	{
		engineSound.loop = true;
		engineSound.volume = 0f;
	}

	private void FixedUpdate()
	{
		UpdateEngineSounds();
	}

	public void PlayAtVolume(float volume)
	{
		if (!engineSound.isPlaying)
		{
			engineSound.Play();
		}
		engineSound.volume = volume;
	}

	private void UpdateEngineSounds()
	{
		Vector3 velocity = body.velocity;
		int num;
		if (Mathf.Abs(velocity.x) < 0.5f)
		{
			Vector3 velocity2 = body.velocity;
			if (Mathf.Abs(velocity2.z) < 0.5f)
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
			if (engineSound.isPlaying)
			{
				engineSound.Stop();
			}
		}
		else if (!engineSound.isPlaying)
		{
			engineSound.Play();
		}
		float magnitude = body.velocity.magnitude;
		if (magnitude < 3f)
		{
			engineSound.volume = magnitude / 3f;
		}
		else
		{
			engineSound.volume = 1f;
		}
		float num2 = body.velocity.magnitude / speedToMaxPitch;
		engineSound.pitch = 1f + num2 * 0.3f;
	}
}
