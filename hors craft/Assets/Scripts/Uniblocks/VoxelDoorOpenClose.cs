// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelDoorOpenClose
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class VoxelDoorOpenClose : VoxelEvents
	{
		public GameObject doorColliderPrefab;

		public override void OnMouseDown(int mouseButton, VoxelInfo voxelInfo)
		{
			if (Engine.EditMode)
			{
				switch (mouseButton)
				{
				case 1:
					RemoveVoxel(voxelInfo);
					break;
				case 0:
					PerformDoorAction(voxelInfo);
					break;
				}
			}
			else
			{
				switch (mouseButton)
				{
				case 1:
					RemoveVoxel(voxelInfo);
					break;
				case 0:
					PerformDoorAction(voxelInfo);
					break;
				}
			}
		}

		public void PerformDoorAction(VoxelInfo voxelInfo)
		{
			bool flag = voxelInfo.chunk.OpenDoor(voxelInfo.rawIndex);
			byte b = voxelInfo.chunk.VoxelRotation[voxelInfo.rawIndex];
			byte b2 = (!flag) ? ((byte)((b + 1) % 4)) : voxelInfo.chunk.GetDoorBasicRoot(voxelInfo.rawIndex);
			Voxel.ChangeBlock(voxelInfo, voxelInfo.GetVoxel(), b2);
			voxelInfo.chunk.SetRotation(voxelInfo.rawIndex, b2);
		}

		public override void OnLook(VoxelInfo voxelInfo)
		{
			GameObject gameObject = GameObject.Find("selected block graphics doors");
			if (gameObject != null)
			{
				gameObject.transform.position = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
				gameObject.GetComponent<Renderer>().enabled = true;
				Vector3 position = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
				gameObject.transform.rotation = Engine.PositionToChunk(position).transform.rotation;
			}
		}

		private void RemoveVoxel(VoxelInfo voxelInfo)
		{
			voxelInfo.chunk.doorsStates.Remove(voxelInfo.rawIndex);
			Voxel.DestroyBlock(voxelInfo);
		}

		public override void OnBlockRotate(VoxelInfo voxelInfo)
		{
			voxelInfo.chunk.CloseLogicDoor(voxelInfo.rawIndex);
			byte b = (byte)((voxelInfo.chunk.VoxelRotation[voxelInfo.rawIndex] + 1) % 4);
			Voxel.ChangeBlock(voxelInfo, voxelInfo.GetVoxel(), b);
			voxelInfo.chunk.ChangeDoorBasicRoot(voxelInfo.rawIndex, b);
		}

		public override void OnBlockPlace(VoxelInfo info)
		{
			Vector3 position = info.chunk.VoxelIndexToPosition(info.index);
			GameObject gameObject = Object.Instantiate(doorColliderPrefab, position, Quaternion.identity);
			gameObject.transform.parent = Engine.PositionToChunk(position).transform;
			info.chunk.AddDoor(info.rawIndex, info.chunk.GetVoxelRotation(info.index));
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			Vector3 position = chunk.VoxelIndexToPosition(x, y, z);
			GameObject gameObject = Object.Instantiate(doorColliderPrefab, position, Quaternion.identity);
			gameObject.transform.parent = Engine.PositionToChunk(position).transform;
			VoxelInfo info = new VoxelInfo(new Index(x, y, z), chunk);
			gameObject.GetComponent<DoorTrigger>().AssignVoxelInfo(info);
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			Manager.Get<CraftingManager>().Spawn(voxelInfo.GetVoxel(), voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index));
		}
	}
}
