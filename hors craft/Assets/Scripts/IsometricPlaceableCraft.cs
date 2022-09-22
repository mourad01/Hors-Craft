// DecompilerFi decompiler from Assembly-CSharp.dll class: IsometricPlaceableCraft
using Common.Managers;
using Common.Utils;
using Uniblocks;
using UnityEngine;

public class IsometricPlaceableCraft : IsometricPlaceableObject
{
	private IsometricCamera isoCamera;

	private Vector3 startPosition;

	protected Transform playerTransform;

	protected GameObject objInstance;

	protected Voxel voxel;

	protected Vector3 previousPosition;

	protected Collider[] colliders = new Collider[1];

	private ushort lastBlockID;

	private bool start;

	private float loweringTime = 1f;

	private float startHeight = 30f;

	private float raycastHeight = 1000f;

	private float defaultPlayerYpos = 100f;

	private byte voxelRotation;

	public void Init(GameObject prefab, Voxel voxel, ushort lastBlockID)
	{
		this.voxel = voxel;
		this.lastBlockID = lastBlockID;
		playerTransform = PlayerGraphic.GetControlledPlayerInstance().transform;
		Vector3 origin = base.transform.position + Vector3.up * raycastHeight;
		VoxelInfo voxelInfo = Engine.VoxelRaycast(origin, Vector3.down, 9999.9f, ignoreTransparent: false);
		startPosition = Engine.VoxelInfoToPosition(voxelInfo);
		InitObjInstance(prefab);
	}

	protected virtual void InitObjInstance(GameObject prefab)
	{
		objInstance = Object.Instantiate(prefab, playerTransform.position + Vector3.up * startHeight, Quaternion.identity);
		SwitchColliders(on: false);
	}

	protected override void Update()
	{
		base.Update();
		if (!start)
		{
			MoveTowardsStart();
			return;
		}
		if (Vector3.Distance(previousPosition, base.transform.position) > 0.5f)
		{
			OnMove();
		}
		Transform transform = base.transform;
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = objInstance.transform.position;
		float y = position2.y + 3f;
		Vector3 position3 = base.transform.position;
		transform.position = new Vector3(x, y, position3.z);
	}

	private void MoveTowardsStart()
	{
		previousPosition = base.transform.position;
		float num = Vector3.Distance(objInstance.transform.position, startPosition);
		objInstance.transform.position = Vector3.MoveTowards(objInstance.transform.position, startPosition, deltaTime * startHeight / loweringTime);
		if (num < 0.1f)
		{
			start = true;
			OnMove();
		}
	}

	private void OnMove()
	{
		previousPosition = base.transform.position;
		Vector3 origin = base.transform.position + Vector3.up * raycastHeight;
		VoxelInfo voxelInfo = Engine.VoxelRaycast(origin, Vector3.down, 9999.9f, ignoreTransparent: false);
		Vector3 vector = Engine.VoxelInfoToPosition(voxelInfo);
		objInstance.transform.position = vector;
		Bounds bounds = GetBounds();
		Vector3 vector2 = MoveFromCollisions(vector);
		objInstance.transform.position += vector2;
	}

	protected virtual Vector3 MoveFromCollisions(Vector3 starPos)
	{
		bool flag = false;
		Vector3 vector = Vector3.zero;
		while (!flag)
		{
			vector += Vector3.up;
			Bounds bounds = GetBounds();
			bounds.center += starPos + vector;
			if (Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents + Vector3.down * 0.5f, colliders, Quaternion.identity) == 0)
			{
				flag = true;
			}
			else
			{
				previousPosition = base.transform.position;
			}
		}
		return vector;
	}

	private void SwitchColliders(bool on)
	{
		Collider[] componentsInChildren = objInstance.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.enabled = on;
		}
		Collider component = GetComponent<Collider>();
		if ((bool)component)
		{
			component.enabled = on;
		}
	}

	public override Bounds GetBounds()
	{
		return RenderersBounds.MaximumBounds(objInstance);
	}

	public override bool OnPlace()
	{
		PlacePlayer();
		PlaceCraft();
		DestroyObjects();
		return true;
	}

	protected virtual void PlaceCraft()
	{
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(objInstance.transform.position);
		if (voxelInfo != null)
		{
			Voxel.PlaceBlock(voxelInfo, ExampleInventory.HeldBlock);
		}
	}

	protected void SetVoxelRotation(VoxelInfo info)
	{
		info.chunk.SetRotation(info.rawIndex, (byte)rotation);
		UnityEngine.Debug.LogError(info.chunk.VoxelRotation[info.rawIndex]);
	}

	protected void DestroyObjects()
	{
		DecreaseCraftable();
		ExampleInventory.HeldBlock = lastBlockID;
		UnityEngine.Object.Destroy(base.gameObject);
		UnityEngine.Object.Destroy(objInstance);
	}

	protected virtual void PlacePlayer()
	{
		if (!(playerTransform.parent != null))
		{
			if (isoCamera == null)
			{
				isoCamera = Object.FindObjectOfType<IsometricCamera>();
			}
			if (isoCamera == null)
			{
				playerTransform.position = base.transform.position + Vector3.up * defaultPlayerYpos;
			}
			Vector3 origin = isoCamera.transform.position + Vector3.up * 100f;
			Ray ray = new Ray(origin, Vector3.down);
			VoxelInfo voxelInfo = Engine.VoxelRaycast(origin, Vector3.down, 9999.9f, ignoreTransparent: false);
			Vector3 a = Engine.VoxelInfoToPosition(voxelInfo);
			playerTransform.position = a + Vector3.up * 2f;
			RotateTowardsObj();
		}
	}

	protected void RotateTowardsObj()
	{
		Vector3 forward = objInstance.transform.position - playerTransform.position;
		forward.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(forward);
		playerTransform.rotation = rotation;
	}

	public override void OnDelete()
	{
		ExampleInventory.HeldBlock = lastBlockID;
		base.OnDelete();
		UnityEngine.Object.Destroy(objInstance);
	}

	public override void OnRotate()
	{
		base.OnRotate();
	}

	protected void DecreaseCraftable()
	{
		if (Manager.Get<CraftingManager>().IsBlockCraftable(ExampleInventory.HeldBlock))
		{
			Singleton<PlayerData>.get.playerItems.UseCraftableBlock(ExampleInventory.HeldBlock, 1);
		}
	}
}
