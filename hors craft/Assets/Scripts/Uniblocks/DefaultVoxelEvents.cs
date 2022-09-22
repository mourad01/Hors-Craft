// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.DefaultVoxelEvents
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class DefaultVoxelEvents : VoxelEvents
	{
		private const float DROP_RANGE = 1f;

		public override void OnMouseDown(int mouseButton, VoxelInfo voxelInfo)
		{
			if (Engine.EditMode)
			{
				switch (mouseButton)
				{
				case 1:
					Voxel.DestroyBlock(voxelInfo);
					break;
				case 0:
				{
					Voxel voxelType = voxelInfo.GetVoxelType();
					if (voxelType.isThatFlower)
					{
						Voxel.PlaceBlock(voxelInfo, ExampleInventory.HeldBlock);
						break;
					}
					VoxelInfo voxelForReplace = new VoxelInfo(voxelInfo.adjacentIndex, voxelInfo.chunk);
					Voxel.PlaceBlock(voxelForReplace, ExampleInventory.HeldBlock);
					break;
				}
				}
				return;
			}
			switch (mouseButton)
			{
			case 1:
				Voxel.DestroyBlock(voxelInfo);
				break;
			case 0:
			{
				Voxel voxelType2 = voxelInfo.GetVoxelType();
				if (voxelType2.isThatFlower)
				{
					Voxel.PlaceBlock(voxelInfo, ExampleInventory.HeldBlock);
					break;
				}
				VoxelInfo voxelForReplace2 = new VoxelInfo(voxelInfo.adjacentIndex, voxelInfo.chunk);
				Voxel.PlaceBlock(voxelForReplace2, ExampleInventory.HeldBlock);
				break;
			}
			}
		}

		public override void OnLook(VoxelInfo voxelInfo)
		{
		}

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			Index index = new Index(voxelInfo.index.x, voxelInfo.index.y - 1, voxelInfo.index.z);
			if (voxelInfo.GetVoxelType().VTransparency == Transparency.solid)
			{
				Voxel voxelType = Engine.GetVoxelType(voxelInfo.chunk.GetVoxel(index));
				if (voxelType.isItSomeKindOfGrass)
				{
					voxelInfo.chunk.SetVoxel(index, Engine.usefulIDs.dirtBlockID, updateMesh: true, 0);
				}
			}
			Vector3 pos = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
			if (!voxelInfo.GetVoxelType().VCustomMesh)
			{
				MonoBehaviourSingleton<VoxelPlacedParticles>.get.Play(pos, voxelInfo.GetVoxel());
			}
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.BLOCK_PLACED);
		}

		public override void OnBlockRotate(VoxelInfo voxelInfo)
		{
			ushort voxel = voxelInfo.GetVoxel();
			byte voxelRotation = voxelInfo.GetVoxelRotation();
			voxelRotation = (byte)((voxelRotation + 1) % 4);
			Voxel.ChangeBlock(voxelInfo, voxel, voxelRotation);
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			Index index = new Index(voxelInfo.index.x, voxelInfo.index.y + 1, voxelInfo.index.z);
			VoxelInfo voxelInfo2 = new VoxelInfo(index, voxelInfo.chunk);
			Voxel voxelType = Engine.GetVoxelType(voxelInfo.chunk.GetVoxel(index));
			Vector3 vector = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
			MonoBehaviourSingleton<VoxelDestroyedParticles>.get.Play(vector, voxelInfo.GetVoxel());
			if (voxelType.isThatFlower)
			{
				Voxel.DestroyBlock(voxelInfo2);
			}
			Spawn(voxelInfo, GetSpawnPosition(vector));
		}

		public override void OnBlockEnter(GameObject enteringObject, VoxelInfo voxelInfo)
		{
		}

		private Vector3 GetSpawnPosition(Vector3 position)
		{
			float x = position.x + Random.Range(-1f, 1f) * 1f;
			float z = position.z + Random.Range(-1f, 1f) * 1f;
			return new Vector3(x, position.y + 1f, z);
		}

		private void Spawn(VoxelInfo voxelInfo, Vector3 lootPosition)
		{
			if (Manager.Contains<PetManager>())
			{
				PetManager petManager = Manager.Get<PetManager>();
				if (petManager.resourceVoxelInfo != null && voxelInfo.index.IsEqual(petManager.resourceVoxelInfo.index))
				{
					if (petManager.currentPet.spawnedSign != null)
					{
						UnityEngine.Object.Destroy(petManager.currentPet.spawnedSign);
					}
					petManager.resourceVoxelInfo = null;
					Manager.Get<CraftingManager>().Spawn(petManager.currentPet.GetComponent<Pettable>(), lootPosition);
				}
				else
				{
					Manager.Get<CraftingManager>().Spawn(voxelInfo.GetVoxel(), lootPosition);
				}
			}
			else
			{
				Manager.Get<CraftingManager>().Spawn(voxelInfo.GetVoxel(), lootPosition);
			}
		}
	}
}
