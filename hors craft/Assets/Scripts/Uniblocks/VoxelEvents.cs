// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class VoxelEvents : MonoBehaviour
	{
		public virtual void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
		}

		public virtual void OnMouseDown(int mouseButton, VoxelInfo voxelInfo)
		{
		}

		public virtual void OnMouseUp(int mouseButton, VoxelInfo voxelInfo)
		{
		}

		public virtual void OnMouseHold(int mouseButton, VoxelInfo voxelInfo)
		{
		}

		public virtual void OnLook(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockPlace(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockPlace(VoxelInfo voxelInfo, ushort previousVoxel)
		{
			OnBlockPlace(voxelInfo);
		}

		public virtual void OnBlockPlaceMultiplayer(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockDestroy(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockDestroyMultiplayer(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockChange(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockChangeMultiplayer(VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockEnter(GameObject enteringObject, VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockStay(GameObject stayingObject, VoxelInfo voxelInfo)
		{
		}

		public virtual void OnBlockRotate(VoxelInfo voxelInfo)
		{
		}
	}
}
