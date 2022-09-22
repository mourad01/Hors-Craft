// DecompilerFi decompiler from Assembly-CSharp.dll class: LowpolyTreeFall
using com.ootii.Cameras;
using Common.Utils;
using Uniblocks;
using UnityEngine;

public class LowpolyTreeFall : Fall
{
	public MeshFilter filter;

	public MeshRenderer renderer;

	public SphereCollider sphereTrigger;

	public Rigidbody rig;

	public float force = 5f;

	public float fallShakeStrenght = 0.075f;

	public float fallShakeDuration = 0.075f;

	public float distroyTime = 1f;

	public override void Init(GameObject oldTree, Vector3 fallDirection)
	{
		filter.sharedMesh = oldTree.GetComponentInChildren<MeshFilter>().sharedMesh;
		renderer.sharedMaterial = oldTree.GetComponentInChildren<MeshRenderer>().sharedMaterial;
		CapsuleCollider capsuleCollider = base.gameObject.AddComponent(oldTree.GetComponentInChildren<CapsuleCollider>());
		rig.AddForceAtPosition(fallDirection * force, base.transform.position + new Vector3(0f, capsuleCollider.height / 2f, 0f), ForceMode.Impulse);
		sphereTrigger.radius = capsuleCollider.radius + 1f;
		sphereTrigger.transform.localPosition = new Vector3(0f, capsuleCollider.height / 2f);
		UnityEngine.Object.Destroy(base.gameObject, distroyTime);
	}

	private void FixedUpdate()
	{
		rig.velocity = rig.velocity.normalized * force;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
			CameraController.instance.Shake(fallShakeStrenght, fallShakeDuration);
			return;
		}
		InteractiveObject componentInChildren = other.gameObject.GetComponentInChildren<InteractiveObject>();
		if (componentInChildren != null)
		{
			componentInChildren.Destroy();
			return;
		}
		componentInChildren = other.gameObject.GetComponentInParent<InteractiveObject>();
		if (componentInChildren != null)
		{
			componentInChildren.Destroy();
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		InteractiveObject componentInChildren = other.gameObject.GetComponentInChildren<InteractiveObject>();
		if (componentInChildren != null)
		{
			componentInChildren.Destroy();
			return;
		}
		componentInChildren = other.gameObject.GetComponentInParent<InteractiveObject>();
		if (componentInChildren != null)
		{
			componentInChildren.Destroy();
		}
	}
}
