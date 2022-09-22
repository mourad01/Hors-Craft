// DecompilerFi decompiler from Assembly-CSharp.dll class: TankTracksOffsetAnimator
using UnityEngine;

public class TankTracksOffsetAnimator : MonoBehaviour
{
	public GameObject leftTrack;

	public int leftMaterialIndex;

	public GameObject rightTrack;

	public int rightMaterialIndex;

	public bool reverse;

	private Rigidbody body;

	private Material leftMaterial;

	private Material rightMaterial;

	private void Awake()
	{
		body = GetComponentInParent<Rigidbody>();
		leftMaterial = leftTrack.GetComponent<MeshRenderer>().materials[leftMaterialIndex];
		rightMaterial = rightTrack.GetComponent<MeshRenderer>().materials[rightMaterialIndex];
	}

	private void Update()
	{
		if ((bool)leftTrack)
		{
			UpdateTracks(leftMaterial, 1f);
		}
		if ((bool)rightTrack)
		{
			UpdateTracks(rightMaterial, -1f);
		}
	}

	private void UpdateTracks(Material trackMaterial, float angularMultiplier)
	{
		Vector2 mainTextureOffset = trackMaterial.mainTextureOffset;
		Vector3 velocity = GetVelocity();
		Vector3 angularVelocity = GetAngularVelocity();
		float magnitude = velocity.magnitude;
		Vector3 vector = base.transform.InverseTransformDirection(velocity);
		float num = Mathf.Sign(vector.z);
		float num2 = magnitude * num * 3f + angularVelocity.y * 10f * angularMultiplier;
		if (reverse)
		{
			num2 *= -1f;
		}
		mainTextureOffset.y += num2 * 0.002f;
		mainTextureOffset.y = mod(mainTextureOffset.y, 1f);
		trackMaterial.mainTextureOffset = mainTextureOffset;
	}

	private Vector3 GetVelocity()
	{
		if (body != null)
		{
			return body.velocity;
		}
		CharacterController componentInParent = GetComponentInParent<CharacterController>();
		return (!(componentInParent != null)) ? Vector3.zero : componentInParent.velocity;
	}

	private Vector3 GetAngularVelocity()
	{
		return (!(body != null)) ? Vector3.zero : body.angularVelocity;
	}

	private float mod(float x, float m)
	{
		float num = x % m;
		return (!(num < 0f)) ? num : (num + m);
	}
}
