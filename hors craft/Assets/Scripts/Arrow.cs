// DecompilerFi decompiler from Assembly-CSharp.dll class: Arrow
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
	public ushort voxelId;

	public float damage = 1f;

	[HideInInspector]
	public Vector3 translation;

	private const float POSITION_ADJUST_TIME = 0.2f;

	private float positionAdjustEndTime;

	private bool isFlying;

	private Rigidbody foundBody;

	public Rigidbody body
	{
		get
		{
			if (foundBody == null)
			{
				foundBody = GetComponent<Rigidbody>();
			}
			return foundBody;
		}
	}

	private void Start()
	{
		body.useGravity = false;
		body.isKinematic = true;
		positionAdjustEndTime = 0f;
	}

	private void Update()
	{
		if (Time.time <= positionAdjustEndTime)
		{
			base.transform.position += translation * (Time.deltaTime / 0.2f);
		}
		Vector3 position = base.transform.position;
		if (position.y < -64f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Fire(float force)
	{
		body.useGravity = true;
		body.isKinematic = false;
		body.AddForce(base.transform.forward * force, ForceMode.Impulse);
		isFlying = true;
		positionAdjustEndTime = Time.time + 0.2f;
		base.transform.SetParent(null);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isFlying)
		{
			Health componentInParent = collision.gameObject.GetComponentInParent<Health>();
			if (componentInParent != null)
			{
				componentInParent.OnHit(damage, base.transform.forward);
				Sound sound = new Sound();
				sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.HIT_ANIMAL);
				sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
				Sound sound2 = sound;
				sound2.Play();
			}
			else
			{
				Sound sound = new Sound();
				sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.HIT_DIRT);
				sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
				Sound sound3 = sound;
				sound3.Play();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
