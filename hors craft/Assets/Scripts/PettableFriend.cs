// DecompilerFi decompiler from Assembly-CSharp.dll class: PettableFriend
using com.ootii.Cameras;
using Common.Managers;
using System;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class PettableFriend : MonoBehaviour
{
	public enum SearchingForVoxel
	{
		NONE = 9999,
		COAL = 0,
		IRON = 1,
		SILVER = 4,
		DIAMOND = 5,
		SAND = 6,
		COPPER = 2,
		GOLD = 3,
		CLAY = 7,
		WOOD = 8,
		COLORANT = 9,
		EGG = 10
	}

	public enum Speciality
	{
		SEARCHING,
		MOUNTABLE,
		NONE
	}

	[NonSerialized]
	public GameObject spawnedSign;

	[NonSerialized]
	public bool shouldGoToResource;

	private const float SEARCHING_TIMER_MIN = 8f;

	private const float SEARCHING_TIMER_MAX = 12f;

	private float currentSearchingTimer;

	private AnimalMob mob;

	public Action OnAwake;

	public Pettable pettable
	{
		get;
		private set;
	}

	private void Awake()
	{
		mob = GetComponentInParent<AnimalMob>();
		pettable = GetComponent<Pettable>();
		if (!Manager.Get<PetManager>().resourcesEnabled)
		{
			pettable.searchingForVoxel = SearchingForVoxel.NONE;
		}
		TryAttachSaveTransform();
		if (OnAwake != null)
		{
			OnAwake();
		}
	}

	private void Update()
	{
		if (pettable.searchingForVoxel != SearchingForVoxel.NONE)
		{
			TryToSearch();
		}
	}

	private void TryToSearch()
	{
		if (currentSearchingTimer <= 0f)
		{
			currentSearchingTimer = UnityEngine.Random.Range(8f, 12f);
			if (spawnedSign != null)
			{
				UnityEngine.Object.Destroy(spawnedSign);
				spawnedSign = null;
			}
			int num = 0;
			VoxelInfo voxelInfo;
			while (true)
			{
				if (num < 3)
				{
					voxelInfo = FindTopVoxelBlock();
					if (voxelInfo != null && voxelInfo.GetVoxel() == Engine.usefulIDs.grassBlockID)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			spawnedSign = GameObject.CreatePrimitive(PrimitiveType.Cube);
			spawnedSign = UnityEngine.Object.Instantiate(Manager.Get<PetManager>().digIndicator);
			spawnedSign.transform.position = Engine.VoxelInfoToPosition(voxelInfo) + Vector3.up;
			Manager.Get<PetManager>().resourceVoxelInfo = voxelInfo;
			shouldGoToResource = true;
			ResourceModeChange();
		}
		else
		{
			currentSearchingTimer -= Time.deltaTime;
		}
	}

	private VoxelInfo FindTopVoxelBlock()
	{
		float num = 6f;
		float num2 = float.PositiveInfinity;
		VoxelInfo result = null;
		Vector3 a = Vector3.zero;
		a = CameraController.instance.MainCamera.transform.position + CameraController.instance.MainCamera.transform.forward * 10f + UnityEngine.Random.insideUnitSphere * 5f;
		Vector3 position = base.transform.position;
		a.y = position.y;
		if (Physics.Raycast(a + Vector3.up * 1000f, Vector3.down, out RaycastHit hitInfo, 1500f))
		{
			Vector3 point = hitInfo.point;
			float y = point.y;
			Vector3 position2 = base.transform.position;
			num2 = Mathf.Abs(y - position2.y);
			if (num2 <= num)
			{
				result = Engine.PositionToVoxelInfo(hitInfo.point);
			}
		}
		return result;
	}

	public void ResourceModeChange()
	{
		mob.logic = AnimalMob.AnimalLogic.RUN_TO_RESOURCE;
		mob.ReconstructBehaviourTree();
	}

	private void OnDestroy()
	{
		if (spawnedSign != null)
		{
			UnityEngine.Object.Destroy(spawnedSign);
		}
	}

	private void TryAttachSaveTransform()
	{
		STPetFriendModule module = Manager.Get<SaveTransformsManager>().modules.FirstOrDefault((AbstractSTModule m) => m is STPetFriendModule) as STPetFriendModule;
		if (module == null)
		{
			UnityEngine.Debug.LogError("Add STPetFriendModule to SaveTransformManager");
			return;
		}
		SaveTransform component = base.gameObject.GetComponent<SaveTransform>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		base.gameObject.AddComponentWithInit(delegate(SaveTransform f)
		{
			f.module = module;
			f.PrefabName = base.gameObject.name.Replace("(Clone)", string.Empty);
		});
	}
}
