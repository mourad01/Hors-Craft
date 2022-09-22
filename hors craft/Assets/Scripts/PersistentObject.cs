// DecompilerFi decompiler from Assembly-CSharp.dll class: PersistentObject
using Uniblocks;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
	public float resetTimer;

	public Voxel parentVoxel;

	private const float CHECK_FREQUENCY = 0.1f;

	private float timer;

	private bool isActive;

	private bool marked;

	private float resetMarkTimer;

	public float creationTime
	{
		get;
		private set;
	}

	public bool MarkToStay(float otherTime)
	{
		if (creationTime < otherTime)
		{
			marked = true;
			return true;
		}
		return false;
	}

	private void Awake()
	{
		creationTime = Time.realtimeSinceStartup;
	}

	private void Start()
	{
		base.transform.SetParent(null, worldPositionStays: true);
		timer = resetTimer;
		SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		sphereCollider.radius = 0.1f;
		base.gameObject.layer = LayerMask.NameToLayer("PersistentObject");
	}

	private void Update()
	{
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position);
		if (isActive && (voxelInfo == null || voxelInfo.GetVoxel() != parentVoxel.GetUniqueID()))
		{
			if (timer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				timer -= Time.deltaTime;
			}
		}
		else
		{
			timer = resetTimer;
			isActive = true;
		}
		if (marked && resetMarkTimer <= 0f)
		{
			marked = false;
		}
		else if (marked)
		{
			resetMarkTimer -= Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!marked && collider.gameObject.layer == LayerMask.NameToLayer("PersistentObject") && collider.gameObject.GetComponent<PersistentObject>().MarkToStay(creationTime))
		{
			GetComponent<Collider>().enabled = false;
			resetMarkTimer = 1f;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
