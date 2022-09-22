// DecompilerFi decompiler from Assembly-CSharp.dll class: Tank
using com.ootii.Cameras;
using Gameplay.Audio;
using System;
using UnityEngine;

public class Tank : HoverCar
{
	[Header("References")]
	public GameObject leftTrack;

	public GameObject rightTrack;

	public AudioClip engineTurnOnClip;

	private float currentWheelRotationRight;

	private float currentWheelRotationLeft;

	protected override void Update()
	{
		base.Update();
		if ((bool)leftTrack)
		{
			UpdateTracks(leftTrack, 1f);
		}
		if ((bool)rightTrack)
		{
			UpdateTracks(rightTrack, -1f);
		}
	}

	protected override void MoveCarWheel(float value)
	{
		Vector3 vector = base.transform.InverseTransformDirection(body.velocity);
		float num = Mathf.Sign(vector.z);
		if (body.velocity.magnitude > 0.2f)
		{
			float num2 = 360f * (body.velocity.magnitude / (float)Math.PI) * Time.deltaTime * num;
			Vector3 angularVelocity = body.angularVelocity;
			float num3 = angularVelocity.y * 3f;
			currentWheelRotationLeft += num2 + num3;
			currentWheelRotationRight += num2 - num3;
		}
		for (int i = 0; i < wheels.Length; i++)
		{
			float num4 = 0f;
			Vector3 localPosition = wheels[i].transform.localPosition;
			num4 = ((!(localPosition.x < 0f)) ? currentWheelRotationRight : currentWheelRotationLeft);
			wheels[i].transform.localRotation = Quaternion.Euler(num4, 0f, 0f);
		}
		if (steeringWheel != null)
		{
			Transform transform = steeringWheel.transform;
			Vector3 eulerAngles = steeringWheel.transform.localRotation.eulerAngles;
			float x = eulerAngles.x;
			Vector3 eulerAngles2 = steeringWheel.transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(x, eulerAngles2.y, value * 80f);
		}
	}

	public override void StopUsing()
	{
		base.StopUsing();
		if (!(player == null))
		{
			body.drag = 0.1f;
			player.SetActive(value: true);
			CameraController.instance.MainCamera.GetComponent<Animator>().enabled = true;
		}
	}

	public override void VehicleActivate(GameObject player)
	{
		base.VehicleActivate(player);
		if (engineTurnOnClip != null)
		{
			MixersManager.Play(engineTurnOnClip);
		}
		CameraController.instance.MainCamera.GetComponent<Animator>().enabled = false;
	}

	protected override void UpdateFov(float factor)
	{
	}

	private void UpdateTracks(GameObject track, float angularMultiplier)
	{
		Vector2 mainTextureOffset = track.GetComponent<Renderer>().material.mainTextureOffset;
		float magnitude = body.velocity.magnitude;
		Vector3 vector = base.transform.InverseTransformDirection(body.velocity);
		float num = Mathf.Sign(vector.z);
		float num2 = magnitude * num * 3f;
		Vector3 angularVelocity = body.angularVelocity;
		float num3 = num2 + angularVelocity.y * 10f * angularMultiplier;
		mainTextureOffset.y += num3 * 0.002f;
		mainTextureOffset.y = Mod(mainTextureOffset.y, 1f);
		track.GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
	}

	private float Mod(float x, float m)
	{
		float num = x % m;
		return (!(num < 0f)) ? num : (num + m);
	}
}
