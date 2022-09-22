// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelChangeClothes
using UnityEngine;

namespace Uniblocks
{
	public class VoxelChangeClothes : VoxelEvents
	{
		public GameObject changeRoomColliderPrefab;

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
					PerformClothesChanging(voxelInfo);
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
					PerformClothesChanging(voxelInfo);
					break;
				}
			}
		}

		public void PerformClothesChanging(VoxelInfo voxelInfo)
		{
		}

		public override void OnBlockRotate(VoxelInfo voxelInfo)
		{
			ushort voxel = voxelInfo.GetVoxel();
			byte voxelRotation = voxelInfo.GetVoxelRotation();
			voxelRotation = (byte)((voxelRotation + 1) % 4);
			Voxel.ChangeBlock(voxelInfo, voxel, voxelRotation);
		}

		public override void OnLook(VoxelInfo voxelInfo)
		{
			GameObject gameObject = GameObject.Find("selected block graphics doors");
			if (gameObject != null)
			{
				gameObject.transform.position = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
				gameObject.GetComponent<Renderer>().enabled = true;
				gameObject.transform.rotation = Engine.PositionToChunk(gameObject.transform.position).transform.rotation;
			}
		}

		private void RemoveVoxel(VoxelInfo voxelInfo)
		{
			Voxel.DestroyBlock(voxelInfo);
		}

		public override void OnBlockPlace(VoxelInfo info)
		{
			Vector3 position = info.chunk.VoxelIndexToPosition(info.index);
			GameObject gameObject = Object.Instantiate(changeRoomColliderPrefab, position, Quaternion.identity);
			gameObject.transform.parent = Engine.PositionToChunk(position).transform;
			gameObject.GetComponent<DoorTrigger>().AssignVoxelInfo(info);
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			Vector3 position = chunk.VoxelIndexToPosition(x, y, z);
			GameObject gameObject = Object.Instantiate(changeRoomColliderPrefab, position, Quaternion.identity);
			gameObject.transform.parent = Engine.PositionToChunk(position).transform;
			VoxelInfo info = new VoxelInfo(new Index(x, y, z), chunk);
			gameObject.GetComponent<DoorTrigger>().AssignVoxelInfo(info);
		}
	}
}
