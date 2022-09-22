// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.Voxel
using com.ootii.Cameras;
using System;
using UnityEngine;

namespace Uniblocks
{
	public class Voxel : MonoBehaviour
	{
		public enum Category
		{
			none,
			basic,
			organic,
			custom,
			furniture,
			craftable
		}

		public enum RarityCategory
		{
			COMMON = 0,
			RARE = 1,
			LEGENDARY = 2,
			UNLIMITED = 999
		}

		[NonSerialized]
		public int VSubmeshIndex;

		public bool editorOnly;

		public bool renderCustomSprite;

		public bool useCustomSprite;

		public Category blockCategory = Category.basic;

		public RarityCategory rarityCategory;

		public int priority;

		public bool isItSomeKindOfGrass;

		public string VName;

		public Mesh VMesh;

		public Mesh VColliderMesh;

		public bool VCustomMesh;

		public bool VCustomSides;

		public Vector2[] VTexture;

		public Transparency VTransparency;

		public ColliderType VColliderType;

		public float VColliderHeight = 1f;

		public Material VMaterial;

		public MeshRotation VRotation;

		public bool isMob;

		public bool isometricPlacment;

		public bool rotateDuringRender;

		public string[] vTextureNames;

		public Sprite voxelSprite;

		public bool isThatFlower;

		public bool moveUvsByVTexture;

		public bool isPlacingInWater;

		[SerializeField]
		[HideInInspector]
		private ushort index;

		private static Transform _camera;

		[Header("Allows OnBlockGenerated VoxelEvents callback.")]
		public bool hasStartBehaviour;

		public bool canRotate;

		public string materialName;

		private static Transform cam
		{
			get
			{
				if (_camera == null)
				{
					_camera = CameraController.instance.MainCamera.transform;
				}
				return _camera;
			}
		}

		public string spriteIndentifier => VName + "_" + GetNameID();

		public ushort GetUniqueID()
		{
			return index;
		}

		public void SetUniqueID(ushort id)
		{
			index = id;
		}

		public string GetUniqueKey()
		{
			return $"block.{index}";
		}

		public virtual void PrepareVoxel(Vector3 position)
		{
		}

		public static void DestroyBlock(VoxelInfo voxelInfo)
		{
			if (!Engine.NotDestroyableBlocks.Contains(voxelInfo.GetVoxel()))
			{
				VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxelInfo.GetVoxel());
				if (instanceForVoxelId != null)
				{
					instanceForVoxelId.OnBlockDestroy(voxelInfo);
				}
				Index index = new Index(voxelInfo.index.x, voxelInfo.index.y + 1, voxelInfo.index.z);
				Index index2 = new Index(voxelInfo.index.x - 1, voxelInfo.index.y, voxelInfo.index.z);
				Index index3 = new Index(voxelInfo.index.x + 1, voxelInfo.index.y, voxelInfo.index.z);
				Index index4 = new Index(voxelInfo.index.x, voxelInfo.index.y, voxelInfo.index.z + 1);
				Index index5 = new Index(voxelInfo.index.x, voxelInfo.index.y, voxelInfo.index.z - 1);
				ushort waterBlockID = Engine.usefulIDs.waterBlockID;
				if (voxelInfo.chunk.GetVoxel(index) == waterBlockID || voxelInfo.chunk.GetVoxel(index4) == waterBlockID || voxelInfo.chunk.GetVoxel(index5) == waterBlockID || voxelInfo.chunk.GetVoxel(index3) == waterBlockID || voxelInfo.chunk.GetVoxel(index2) == waterBlockID)
				{
					PlaceBlock(voxelInfo, waterBlockID);
				}
				else
				{
					voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: true, 0);
				}
			}
		}

		public static void PlaceBlock(VoxelInfo voxelForReplace, ushort voxelId)
		{
			ushort previousVoxel = voxelForReplace.GetVoxel();
			if (voxelId == Engine.usefulIDs.waterBlockID)
			{
				UnityEngine.Debug.Log("Trying to place water...");
				voxelForReplace.chunk.SetVoxelWater(voxelForReplace.index, voxelId, updateMesh: true, 0);
			}
			else
			{
				Voxel voxelType = Engine.GetVoxelType(voxelId);
				byte voxelRotation = 0;
				if (voxelType.canRotate)
				{
					Vector3 forward = cam.forward;
					if (forward.z < 0f && forward.x < 0.5f && forward.x > -0.5f)
					{
						voxelRotation = 2;
					}
					else if (forward.x < 0f && forward.z < 0.5f && forward.z > -0.5f)
					{
						voxelRotation = 3;
					}
					else if (forward.x > 0f && forward.z < 0.5f && forward.z > -0.5f)
					{
						voxelRotation = 1;
					}
				}
				if (CanBePlaced(voxelForReplace.GetVoxelType(), voxelType))
				{
					if (voxelType.VCustomMesh && !voxelType.moveUvsByVTexture)
					{
						Vector3 pos = Engine.VoxelInfoToPosition(voxelForReplace);
						MonoBehaviourSingleton<VoxelPlacedParticles>.get.PlaceCustomObject(voxelType, pos, voxelRotation, delegate
						{
							voxelForReplace.chunk.SetVoxel(voxelForReplace.index, voxelId, updateMesh: true, voxelRotation);
							RunEvents(voxelForReplace, voxelId, previousVoxel);
						});
						return;
					}
					voxelForReplace.chunk.SetVoxel(voxelForReplace.index, voxelId, updateMesh: true, voxelRotation);
				}
				UnityEngine.Debug.Log("Trying to place block with ID " + voxelType.GetUniqueID().ToString() + ", prefab name: " + voxelType.name);
			}
			RunEvents(voxelForReplace, voxelId, previousVoxel);
		}

		private static void RunEvents(VoxelInfo voxelForReplace, ushort voxelId, ushort previousVoxel)
		{
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxelId);
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnBlockPlace(voxelForReplace, previousVoxel);
			}
		}

		private static bool CanBePlaced(Voxel voxelForReplace, Voxel voxelToPlace)
		{
			return !voxelToPlace.isPlacingInWater || (voxelForReplace.GetUniqueID() == Engine.usefulIDs.waterBlockID && voxelToPlace.isPlacingInWater);
		}

		public static void ChangeBlock(VoxelInfo voxelInfo, ushort data, byte rotation)
		{
			voxelInfo.chunk.SetVoxel(voxelInfo.index, data, updateMesh: true, rotation);
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(data);
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnBlockChange(voxelInfo);
			}
		}

		public ushort GetNameID()
		{
			return ushort.Parse(base.gameObject.name.Split('_')[1]);
		}

		public void SetName(ushort id)
		{
			base.gameObject.name = "block_" + id.ToString();
		}

		public string GetTextureName(int index)
		{
			if (vTextureNames != null && vTextureNames.Length != 0 && vTextureNames.Length > index)
			{
				return vTextureNames[index];
			}
			if (voxelSprite != null && index == 0)
			{
				return VName;
			}
			return null;
		}

		public int GetMaterialTextureSize()
		{
			if (VMaterial != null && VMaterial.mainTexture != null)
			{
				return VMaterial.mainTexture.width;
			}
			return 1024;
		}
	}
}
