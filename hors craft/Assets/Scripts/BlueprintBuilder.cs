// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintBuilder
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay;
using Gameplay.Audio;
using States;
using System;
using System.Collections;
using Uniblocks;
using UnityEngine;

public class BlueprintBuilder : MonoBehaviour
{
	private const int REACHABLE_VOXELS_DISTANCE = 1;

	private BlueprintManager manager;

	private Sound _blockPlaceSound;

	private Sound blockPlaceSound
	{
		get
		{
			if (_blockPlaceSound == null)
			{
				_blockPlaceSound = new Sound
				{
					clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.BLOCK_PLACE),
					mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup
				};
			}
			return _blockPlaceSound;
		}
	}

	private void Awake()
	{
		manager = GetComponent<BlueprintManager>();
	}

	public void FixBlueprint(PlacedBlueprint placedBlueprint)
	{
		manager.GetBlueprintData(placedBlueprint.placedBlueprintData.dataName);
		placedBlueprint.loaded = true;
		Func<Vector3, bool> doWhile = delegate(Vector3 worldPosition)
		{
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(worldPosition);
			if (voxelInfo == null)
			{
				placedBlueprint.loaded = false;
				return true;
			}
			byte rotation;
			ushort realVoxelTypeFromWorldPosition = manager.GetRealVoxelTypeFromWorldPosition(placedBlueprint.placedBlueprintData, worldPosition, out rotation);
			if (realVoxelTypeFromWorldPosition != 0 && realVoxelTypeFromWorldPosition != 12)
			{
				if (placedBlueprint.placedVoxels.Contains(worldPosition))
				{
					if (voxelInfo.GetVoxel() != realVoxelTypeFromWorldPosition)
					{
						FillVoxelInPosition(worldPosition, manually: false);
					}
				}
				else
				{
					float y = placedBlueprint.placedBlueprintData.min.y;
					Vector3 localPosition = placedBlueprint.placedBlueprintData.GetLocalPosition(worldPosition);
					if (y == localPosition.y)
					{
						if (!voxelInfo.chunk.rebuildOnMainThread)
						{
							voxelInfo.chunk.rebuildOnMainThread = true;
						}
						voxelInfo.chunk.SetVoxel(voxelInfo.index, Engine.usefulIDs.blueprintID, updateMesh: true, rotation);
					}
				}
			}
			return false;
		};
		DoForAllInBounds(placedBlueprint.placedBlueprintData, doWhile);
	}

	private void ActivateNearbyVoxels(PlacedBlueprint placedBlueprint, Vector3 worldPosition)
	{
		PlacedBlueprintData placedBlueprintData = placedBlueprint.placedBlueprintData;
		Vector3 localPosition = placedBlueprintData.GetLocalPosition(worldPosition);
		for (int i = (!(localPosition.y - 1f > placedBlueprintData.min.y)) ? ((int)placedBlueprintData.min.y) : ((int)localPosition.y - 1); i <= (int)localPosition.y + 1 && i <= (int)placedBlueprintData.max.y; i++)
		{
			int num = 1;
			for (int j = (!(localPosition.x - (float)num > placedBlueprintData.min.x)) ? ((int)placedBlueprintData.min.x) : ((int)localPosition.x - num); j <= (int)localPosition.x + num && j <= (int)placedBlueprintData.max.x; j++)
			{
				int num2 = num;
				for (int k = (!(localPosition.z - (float)num2 > placedBlueprintData.min.z)) ? ((int)placedBlueprintData.min.z) : ((int)localPosition.z - num2); k <= (int)localPosition.z + num2 && k <= (int)placedBlueprintData.max.z; k++)
				{
					ActivateVoxel(placedBlueprint, new Vector3(j, i, k));
				}
			}
		}
	}

	private void ActivateVoxel(PlacedBlueprint placedBlueprint, Vector3 localPosition)
	{
		byte rotation;
		ushort realVoxelTypeFromLocalPosition = manager.GetRealVoxelTypeFromLocalPosition(placedBlueprint.placedBlueprintData, localPosition, out rotation);
		rotation = (byte)((rotation + placedBlueprint.placedBlueprintData.rotation) % 4);
		if (realVoxelTypeFromLocalPosition == 0 || realVoxelTypeFromLocalPosition == 12)
		{
			return;
		}
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(placedBlueprint.placedBlueprintData.GetWorldPosition(localPosition));
		ushort voxel = voxelInfo.GetVoxel();
		if (voxelInfo != null && voxel != realVoxelTypeFromLocalPosition && voxel != Engine.usefulIDs.blueprintID)
		{
			if (!voxelInfo.chunk.rebuildOnMainThread)
			{
				voxelInfo.chunk.rebuildOnMainThread = true;
			}
			voxelInfo.chunk.SetVoxel(voxelInfo.index, Engine.usefulIDs.blueprintID, updateMesh: true, rotation);
		}
	}

	public void FillVoxelInPosition(Vector3 worldPosition, bool manually = true)
	{
		PlacedBlueprint placedBlueprint = manager.GetPlacedBlueprint(worldPosition, accurate: true);
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(worldPosition);
		if (voxelInfo != null && (!manually || voxelInfo.GetVoxel() == Engine.usefulIDs.blueprintID))
		{
			FillVoxel(placedBlueprint, voxelInfo, worldPosition, manually);
			CheckProgress(placedBlueprint, manually);
		}
	}

	private void FillVoxel(PlacedBlueprint placedBlueprint, VoxelInfo info, Vector3 worldPosition, bool manually)
	{
		byte rotation;
		ushort realVoxelTypeFromWorldPosition = manager.GetRealVoxelTypeFromWorldPosition(placedBlueprint.placedBlueprintData, worldPosition, out rotation);
		Voxel voxelType = Engine.GetVoxelType(realVoxelTypeFromWorldPosition);
		if (realVoxelTypeFromWorldPosition == 0 || realVoxelTypeFromWorldPosition == 12)
		{
			return;
		}
		rotation = (byte)((rotation + placedBlueprint.placedBlueprintData.rotation) % 4);
		info.chunk.SetVoxel(info.index, realVoxelTypeFromWorldPosition, updateMesh: true, rotation);
		if (voxelType.hasStartBehaviour)
		{
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(realVoxelTypeFromWorldPosition);
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnBlockPlace(info);
			}
		}
		if (manually)
		{
			blockPlaceSound.Play();
		}
		placedBlueprint.placedVoxels.Add(worldPosition);
		ActivateNearbyVoxels(placedBlueprint, worldPosition);
	}

	private void CheckProgress(PlacedBlueprint placedBlueprint, bool manually)
	{
		if (placedBlueprint.progress >= 1f)
		{
			StartCoroutine(StartAnimation(placedBlueprint));
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.BLUEPRINTS_FINISHED);
			if (placedBlueprint.placedBlueprintData.instantFiledProgress < 0.0001f)
			{
				Manager.Get<StatsManager>().XcraftBlueprintCompleted(placedBlueprint.placedBlueprintData.dataName, wasInstant: false, placedBlueprint.placedBlueprintData.instantFiledProgress);
				Manager.Get<StatsManager>().Blueprint(StatsManager.BlueprintAction.MANUALLY_FILLED);
			}
			else
			{
				Manager.Get<StatsManager>().XcraftBlueprintCompleted(placedBlueprint.placedBlueprintData.dataName, wasInstant: true, placedBlueprint.placedBlueprintData.instantFiledProgress);
				Manager.Get<StatsManager>().Blueprint(StatsManager.BlueprintAction.AUTO_FILLED);
			}
			UnityEngine.Debug.LogError($"Blueprint: {placedBlueprint.placedBlueprintData.dataName}; {placedBlueprint.placedBlueprintData.instantFiledProgress}");
			manager.BuiltBlueprint(placedBlueprint);
			Manager.Get<GameCallbacksManager>().FrequentSave();
			Engine.SaveWorld();
		}
	}

	private IEnumerator StartAnimation(PlacedBlueprint placedBlueprint)
	{
		Vector3 position = placedBlueprint.placedBlueprintData.position;
		Vector3 radius = placedBlueprint.placedBlueprintData.max - placedBlueprint.placedBlueprintData.min;
		yield return new WaitForEndOfFrame();
		while (ChunkDataFiles.SavingChunks)
		{
			yield return new WaitForEndOfFrame();
		}
		Manager.Get<StateMachineManager>().PushState<BlueprintAnimationState>(new BlueprintAnimationStateStartParameter
		{
			position = position,
			size = radius
		});
	}

	public void InstantFillAllBlueprintAccurate(Vector3 worldPosition)
	{
		PlacedBlueprint placedBlueprint = manager.GetPlacedBlueprint(worldPosition, accurate: true);
		InstantFillAllBlueprint(placedBlueprint);
	}

	private void InstantFillAllBlueprint(PlacedBlueprint placedBlueprint)
	{
		StartCoroutine(FillAllBlocksBlueprint(placedBlueprint));
	}

	private IEnumerator FillAllBlocksBlueprint(PlacedBlueprint blueprint, float time = 3f)
	{
		float progressToFill = 1f / (float)blueprint.adsToFillBlueprint + blueprint.progress;
		PlacedBlueprintData placedBlueprintData = blueprint.placedBlueprintData;
		if (progressToFill > 1f)
		{
			placedBlueprintData.instantFiledProgress += 1f - blueprint.progress;
		}
		else
		{
			placedBlueprintData.instantFiledProgress += 1f / (float)blueprint.adsToFillBlueprint;
		}
		Vector3 localPosition = default(Vector3);
		float waitTime = time / placedBlueprintData.max.y - placedBlueprintData.min.y;
		localPosition.y = placedBlueprintData.min.y;
		while (localPosition.y <= placedBlueprintData.max.y)
		{
			localPosition.x = placedBlueprintData.min.x;
			while (localPosition.x <= placedBlueprintData.max.x)
			{
				localPosition.z = placedBlueprintData.min.z;
				while (localPosition.z <= placedBlueprintData.max.z)
				{
					Vector3 worldPosition = placedBlueprintData.GetWorldPosition(localPosition);
					FillVoxelInPosition(worldPosition, manually: false);
					if (blueprint.progress >= 1f)
					{
						yield break;
					}
					if (blueprint.progress >= progressToFill)
					{
						blueprint.lastPlacedVoxelYOffset = (int)localPosition.y;
						yield break;
					}
					localPosition.z += 1f;
				}
				localPosition.x += 1f;
			}
			if (localPosition.y >= (float)blueprint.lastPlacedVoxelYOffset)
			{
				yield return new WaitForSeconds(waitTime);
			}
			else
			{
				yield return null;
			}
			localPosition.y += 1f;
		}
	}

	public Vector3 GetDefaultBlockToBuild(Vector3 worldPosition)
	{
		PlacedBlueprint placedBlueprint = manager.GetPlacedBlueprint(worldPosition, accurate: false);
		if (placedBlueprint == null)
		{
			return Vector3.zero;
		}
		PlacedBlueprintData placedBlueprintData = placedBlueprint.placedBlueprintData;
		Vector3 localPosition = placedBlueprintData.GetLocalPosition(worldPosition);
		localPosition.x = (int)Mathf.Clamp(localPosition.x, placedBlueprintData.min.x, placedBlueprintData.max.x);
		localPosition.y = (int)Mathf.Clamp(localPosition.y, placedBlueprintData.min.y, placedBlueprintData.max.y);
		localPosition.z = (int)Mathf.Clamp(localPosition.z, placedBlueprintData.min.z, placedBlueprintData.max.z);
		int num = 0;
		bool flag = false;
		while (!flag)
		{
			flag = true;
			for (int i = (!(localPosition.y - (float)num > placedBlueprintData.min.y)) ? ((int)placedBlueprintData.min.y) : ((int)localPosition.y - num); i <= (int)localPosition.y + num && i <= (int)placedBlueprintData.max.y; i++)
			{
				int num2 = num - (int)Mathf.Abs(localPosition.y - (float)i);
				for (int j = (!(localPosition.x - (float)num2 > placedBlueprintData.min.x)) ? ((int)placedBlueprintData.min.x) : ((int)localPosition.x - num2); j <= (int)localPosition.x + num2 && j <= (int)placedBlueprintData.max.x; j++)
				{
					int num3 = num2 - (int)Mathf.Abs(localPosition.x - (float)j);
					if (localPosition.z - (float)num3 >= placedBlueprintData.min.z)
					{
						flag = false;
						Vector3 worldPosition2 = placedBlueprintData.GetWorldPosition(new Vector3(j, i, localPosition.z - (float)num3));
						VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(worldPosition2);
						if (voxelInfo != null && voxelInfo.GetVoxel() == Engine.usefulIDs.blueprintID)
						{
							return worldPosition2;
						}
					}
					if (localPosition.z + (float)num3 <= placedBlueprintData.max.z)
					{
						flag = false;
						Vector3 worldPosition3 = placedBlueprintData.GetWorldPosition(new Vector3(j, i, localPosition.z + (float)num3));
						VoxelInfo voxelInfo2 = Engine.PositionToVoxelInfo(worldPosition3);
						if (voxelInfo2 != null && voxelInfo2.GetVoxel() == Engine.usefulIDs.blueprintID)
						{
							return worldPosition3;
						}
					}
				}
			}
			num++;
		}
		return Vector3.zero;
	}

	public void DeleteBlueprint(PlacedBlueprintData placedBlueprint)
	{
		Func<Vector3, bool> doWhile = delegate(Vector3 vector)
		{
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(vector);
			if (voxelInfo != null)
			{
				Voxel.DestroyBlock(voxelInfo);
			}
			return false;
		};
		DoForAllInBounds(placedBlueprint, doWhile);
	}

	private void DoForAllInBounds(PlacedBlueprintData placedBlueprintData, Func<Vector3, bool> doWhile)
	{
		Vector3 zero = Vector3.zero;
		zero.x = placedBlueprintData.min.x;
		while (zero.x <= placedBlueprintData.max.x)
		{
			zero.y = placedBlueprintData.min.y;
			while (zero.y <= placedBlueprintData.max.y)
			{
				zero.z = placedBlueprintData.min.z;
				while (zero.z <= placedBlueprintData.max.z)
				{
					if (doWhile(placedBlueprintData.GetWorldPosition(zero)))
					{
						return;
					}
					zero.z += 1f;
				}
				zero.y += 1f;
			}
			zero.x += 1f;
		}
	}
}
