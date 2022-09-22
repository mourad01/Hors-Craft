// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintVoxel
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class BlueprintVoxel : Voxel
	{
		public Material sheetBlueprintMat;

		public Material defaultBlueprintMat;

		public Material customMeshMat;

		[HideInInspector]
		public static int defaultBlueprintIndex;

		[HideInInspector]
		public static int customMeshIndex;

		[HideInInspector]
		public static int sheetBlueprintIndex;

		private static BlueprintManager _blueprintManager;

		private static BlueprintManager blueprintManager
		{
			get
			{
				if (_blueprintManager == null)
				{
					_blueprintManager = Manager.Get<BlueprintManager>();
				}
				return _blueprintManager;
			}
		}

		public override void PrepareVoxel(Vector3 position)
		{
			byte rotation;
			Voxel realVoxel = blueprintManager.GetRealVoxel(position, out rotation);
			if (realVoxel.GetUniqueID() == 0)
			{
				Convert(position, realVoxel.GetUniqueID());
				return;
			}
			VMesh = realVoxel.VMesh;
			VTexture = realVoxel.VTexture;
			VCustomMesh = realVoxel.VCustomMesh;
			VCustomSides = realVoxel.VCustomSides;
			VRotation = realVoxel.VRotation;
			vTextureNames = realVoxel.vTextureNames;
			VName = realVoxel.VName;
			moveUvsByVTexture = realVoxel.moveUvsByVTexture;
			VTransparency = realVoxel.VTransparency;
			if (VTransparency == Transparency.solid)
			{
				VTransparency = Transparency.semiTransparent;
			}
			if (realVoxel.materialName.Contains("CustomMesh"))
			{
				VMaterial = customMeshMat;
				VSubmeshIndex = customMeshIndex;
			}
			else if (realVoxel.materialName.Contains("Sheet"))
			{
				VMaterial = sheetBlueprintMat;
				VSubmeshIndex = sheetBlueprintIndex;
			}
			else
			{
				VMaterial = defaultBlueprintMat;
				VSubmeshIndex = defaultBlueprintIndex;
			}
		}

		private void Convert(Vector3 position, ushort voxel)
		{
			Voxel.PlaceBlock(Engine.PositionToVoxelInfo(position), voxel);
		}
	}
}
