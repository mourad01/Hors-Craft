// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChunkMeshCreator
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class ChunkMeshCreator : MonoBehaviour, IMeshCreator
	{
		private enum AdjacentCheckResult
		{
			DONT_RENDER,
			RENDER,
			RENDER_WITHOUT_COLLIDER
		}

		public const int MAX_VERTEX_COUNT = 200;

		private Chunk chunk;

		private ChunkData data;

		private int SideLength;

		private int materialsLength;

		private GameObject noCollideCollider;

		private Vector3[] cubeVertices;

		private int[] cubeTriangles;

		private List<Vector3> Vertices;

		private List<List<int>> Faces;

		private List<Vector2> UVs;

		private int FaceCount;

		private List<Vector3> SolidColliderVertices;

		private List<int> SolidColliderFaces;

		private int SolidFaceCount;

		private List<Vector3> NoCollideVertices;

		private List<int> NoCollideFaces;

		private int NoCollideFaceCount;

		private bool triggerVoxelEvents;

		private MeshRenderer meshRenderer;

		private Material[] materialsCache;

		private GameObject additionalCustomMeshPrefab;

		public void LogSizes()
		{
			UnityEngine.Debug.LogWarning("SolidColliderVertices.Capacity: " + SolidColliderVertices.Capacity);
			UnityEngine.Debug.LogWarning("SolidColliderFaces.Capacity: " + SolidColliderFaces.Capacity);
			UnityEngine.Debug.LogWarning("NoCollideVertices.Capacity: " + NoCollideVertices.Capacity);
			UnityEngine.Debug.LogWarning("NoCollideFaces.Capacity: " + NoCollideFaces.Capacity);
			UnityEngine.Debug.LogWarning("Vertices.Capacity: " + Vertices.Capacity);
			UnityEngine.Debug.LogWarning("UVs.Capacity: " + UVs.Capacity);
			int num = int.MinValue;
			for (int i = 0; i < Faces.Count; i++)
			{
				if (Faces[i].Capacity > num)
				{
					num = Faces[i].Capacity;
				}
			}
			UnityEngine.Debug.LogWarning("Faces max capacity: " + num);
		}

		private void Awake()
		{
		}

		public void Init()
		{
			SolidColliderVertices = new List<Vector3>(8192);
			SolidColliderFaces = new List<int>(8192);
			NoCollideVertices = new List<Vector3>(2048);
			NoCollideFaces = new List<int>(4096);
			Vertices = new List<Vector3>(16384);
			UVs = new List<Vector2>(16384);
			cubeVertices = new Vector3[24]
			{
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f)
			};
			cubeTriangles = new int[36]
			{
				0,
				2,
				3,
				0,
				3,
				1,
				8,
				4,
				5,
				8,
				5,
				9,
				10,
				6,
				7,
				10,
				7,
				11,
				12,
				13,
				14,
				12,
				14,
				15,
				16,
				17,
				18,
				16,
				18,
				19,
				20,
				21,
				22,
				20,
				22,
				23
			};
		}

		public void InitSubmeshArrays(Material[] sharedMaterials, Chunk chunk)
		{
			materialsCache = new List<Material>(sharedMaterials).ToArray();
			materialsLength = sharedMaterials.Length;
			Faces = new List<List<int>>(sharedMaterials.Length);
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Faces.Add(new List<int>(32768));
			}
		}

		public void Generate(ChunkData chunkD, bool triggerVoxelEvents = true)
		{
			data = chunkD;
			ChunkManager.Chunks.TryGetValue(data.ChunkIndex, out chunk);
			if (chunk == null)
			{
				chunk = ChunkManager.chunkPool.Take().Init(data);
			}
			if (!chunk.chunkData.Empty)
			{
				SideLength = ChunkData.SideLength;
				meshRenderer = chunk.GetComponent<MeshRenderer>();
				this.triggerVoxelEvents = triggerVoxelEvents;
				RebuildMesh();
			}
		}

		private void RebuildMesh()
		{
			IEnumerator enumerator = chunk.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (!transform.CompareTag("DontDestroyOnRebild"))
					{
						UnityEngine.Object.Destroy(transform.gameObject);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			int i = 0;
			int j = 0;
			for (int k = 0; k < SideLength; k++)
			{
				for (; j < SideLength; j++)
				{
					for (; i < SideLength; i++)
					{
						ushort voxel = data.GetVoxel(i, j, k);
						byte voxelRotation = data.GetVoxelRotation(i, j, k);
						if (voxel == 0)
						{
							continue;
						}
						Vector3 position = chunk.transform.position + new Vector3(i, j, k);
						Voxel voxelType = Engine.GetVoxelType(voxel, position);
						if (triggerVoxelEvents && voxelType.hasStartBehaviour)
						{
							VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxel);
							if (instanceForVoxelId != null)
							{
								instanceForVoxelId.OnBlockRebuilded(chunk.chunkData, i, j, k);
							}
						}
						if (!voxelType.VCustomMesh)
						{
							Transparency vTransparency = voxelType.VTransparency;
							ColliderType vColliderType = voxelType.VColliderType;
							AdjacentCheckResult adjacentCheckResult = CheckAdjacent(voxel, i, j, k, Direction.forward, vTransparency);
							if (adjacentCheckResult != 0)
							{
								CreateFace(voxel, Facing.forward, vColliderType, i, j, k, voxelRotation, adjacentCheckResult);
							}
							adjacentCheckResult = CheckAdjacent(voxel, i, j, k, Direction.back, vTransparency);
							if (adjacentCheckResult != 0)
							{
								CreateFace(voxel, Facing.back, vColliderType, i, j, k, voxelRotation, adjacentCheckResult);
							}
							adjacentCheckResult = CheckAdjacent(voxel, i, j, k, Direction.up, vTransparency);
							if (adjacentCheckResult != 0)
							{
								CreateFace(voxel, Facing.up, vColliderType, i, j, k, voxelRotation, adjacentCheckResult);
							}
							adjacentCheckResult = CheckAdjacent(voxel, i, j, k, Direction.down, vTransparency);
							if (adjacentCheckResult != 0)
							{
								CreateFace(voxel, Facing.down, vColliderType, i, j, k, voxelRotation, adjacentCheckResult);
							}
							adjacentCheckResult = CheckAdjacent(voxel, i, j, k, Direction.right, vTransparency);
							if (adjacentCheckResult != 0)
							{
								CreateFace(voxel, Facing.right, vColliderType, i, j, k, voxelRotation, adjacentCheckResult);
							}
							adjacentCheckResult = CheckAdjacent(voxel, i, j, k, Direction.left, vTransparency);
							if (adjacentCheckResult != 0)
							{
								CreateFace(voxel, Facing.left, vColliderType, i, j, k, voxelRotation, adjacentCheckResult);
							}
							if (vColliderType == ColliderType.none)
							{
								AddCubeMesh(i, j, k, solid: false);
							}
						}
						else if (!CheckAllAdjacent(i, j, k))
						{
							CreateCustomMesh(voxel, i, j, k, voxelType.VMesh, voxelRotation, voxelType.VColliderMesh);
						}
					}
					i = 0;
				}
				j = 0;
			}
			UpdateMesh(chunk.GetComponent<MeshFilter>().mesh, chunk.GetComponent<MeshCollider>());
		}

		private AdjacentCheckResult CheckAdjacent(ushort voxel, int x, int y, int z, Direction direction, Transparency transparency)
		{
			switch (direction)
			{
			case Direction.down:
				y--;
				break;
			case Direction.up:
				y++;
				break;
			case Direction.left:
				x--;
				break;
			case Direction.right:
				x++;
				break;
			case Direction.back:
				z--;
				break;
			case Direction.forward:
				z++;
				break;
			}
			ushort voxel2 = data.GetVoxel(x, y, z);
			if (voxel2 == ushort.MaxValue)
			{
				if (direction == Direction.up)
				{
					return AdjacentCheckResult.RENDER;
				}
				return AdjacentCheckResult.DONT_RENDER;
			}
			Vector3 position = chunk.transform.position + new Vector3(x, y, z);
			Voxel voxelType = Engine.GetVoxelType(voxel2, position);
			Transparency vTransparency = voxelType.VTransparency;
			if (transparency == Transparency.overlay && voxel2 == Engine.usefulIDs.iceBlockID)
			{
				return AdjacentCheckResult.RENDER;
			}
			switch (transparency)
			{
			case Transparency.overlay:
				if (vTransparency != Transparency.transparent)
				{
					return AdjacentCheckResult.DONT_RENDER;
				}
				return AdjacentCheckResult.RENDER;
			case Transparency.transparent:
				if (vTransparency == Transparency.transparent)
				{
					return AdjacentCheckResult.DONT_RENDER;
				}
				return AdjacentCheckResult.RENDER;
			default:
				if (vTransparency == Transparency.solid)
				{
					return AdjacentCheckResult.DONT_RENDER;
				}
				if (transparency == vTransparency && vTransparency == Transparency.semiTransparent && voxel == voxel2 && (voxel != Engine.usefulIDs.blueprintID || voxelType.VMesh == null))
				{
					return AdjacentCheckResult.DONT_RENDER;
				}
				return AdjacentCheckResult.RENDER;
			}
		}

		public bool CheckAllAdjacent(int x, int y, int z)
		{
			for (int i = 0; i < 6; i++)
			{
				Index adjacentIndex = data.GetAdjacentIndex(x, y, z, (Direction)i);
				Vector3 position = chunk.transform.position + adjacentIndex.ToVector3();
				if (Engine.GetVoxelType(data.GetVoxel(adjacentIndex), position).VTransparency != 0)
				{
					return false;
				}
			}
			return true;
		}

		private void CreateFace(ushort voxel, Facing facing, ColliderType colliderType, int x, int y, int z, byte rotation, AdjacentCheckResult adjacentCheckResult)
		{
			Vector3 position = chunk.transform.position + new Vector3(x, y, z);
			Voxel voxelType = Engine.GetVoxelType(voxel, position);
			List<int> list = Faces[voxelType.VSubmeshIndex];
			int materialTextureSize = voxelType.GetMaterialTextureSize();
			bool flag = colliderType == ColliderType.cube && adjacentCheckResult == AdjacentCheckResult.RENDER;
			switch (facing)
			{
			case Facing.forward:
			{
				if (!flag)
				{
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y + 0.5001f, (float)z + 0.5f));
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y + 0.5001f, (float)z + 0.5f));
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y - 0.5001f, (float)z + 0.5f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y - 0.5001f, (float)z + 0.5f));
					break;
				}
				Vector3 item17 = new Vector3((float)x + 0.5001f, (float)y + 0.5001f, (float)z + 0.5f);
				Vector3 item18 = new Vector3((float)x - 0.5001f, (float)y + 0.5001f, (float)z + 0.5f);
				Vector3 item19 = new Vector3((float)x - 0.5001f, (float)y - 0.5001f, (float)z + 0.5f);
				Vector3 item20 = new Vector3((float)x + 0.5001f, (float)y - 0.5001f, (float)z + 0.5f);
				Vertices.Add(item17);
				SolidColliderVertices.Add(item17);
				Vertices.Add(item18);
				SolidColliderVertices.Add(item18);
				Vertices.Add(item19);
				SolidColliderVertices.Add(item19);
				Vertices.Add(item20);
				SolidColliderVertices.Add(item20);
				break;
			}
			case Facing.up:
			{
				if (!flag)
				{
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y + 0.5f, (float)z + 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y + 0.5f, (float)z + 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y + 0.5f, (float)z - 0.5001f));
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y + 0.5f, (float)z - 0.5001f));
					break;
				}
				Vector3 item9 = new Vector3((float)x - 0.5001f, (float)y + 0.5f, (float)z + 0.5001f);
				Vector3 item10 = new Vector3((float)x + 0.5001f, (float)y + 0.5f, (float)z + 0.5001f);
				Vector3 item11 = new Vector3((float)x + 0.5001f, (float)y + 0.5f, (float)z - 0.5001f);
				Vector3 item12 = new Vector3((float)x - 0.5001f, (float)y + 0.5f, (float)z - 0.5001f);
				Vertices.Add(item9);
				SolidColliderVertices.Add(item9);
				Vertices.Add(item10);
				SolidColliderVertices.Add(item10);
				Vertices.Add(item11);
				SolidColliderVertices.Add(item11);
				Vertices.Add(item12);
				SolidColliderVertices.Add(item12);
				break;
			}
			case Facing.right:
			{
				if (!flag)
				{
					Vertices.Add(new Vector3((float)x + 0.5f, (float)y + 0.5001f, (float)z - 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5f, (float)y + 0.5001f, (float)z + 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5f, (float)y - 0.5001f, (float)z + 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5f, (float)y - 0.5001f, (float)z - 0.5001f));
					break;
				}
				Vector3 item21 = new Vector3((float)x + 0.5f, (float)y + 0.5001f, (float)z - 0.5001f);
				Vector3 item22 = new Vector3((float)x + 0.5f, (float)y + 0.5001f, (float)z + 0.5001f);
				Vector3 item23 = new Vector3((float)x + 0.5f, (float)y - 0.5001f, (float)z + 0.5001f);
				Vector3 item24 = new Vector3((float)x + 0.5f, (float)y - 0.5001f, (float)z - 0.5001f);
				Vertices.Add(item21);
				SolidColliderVertices.Add(item21);
				Vertices.Add(item22);
				SolidColliderVertices.Add(item22);
				Vertices.Add(item23);
				SolidColliderVertices.Add(item23);
				Vertices.Add(item24);
				SolidColliderVertices.Add(item24);
				break;
			}
			case Facing.back:
			{
				if (!flag)
				{
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y + 0.5001f, (float)z - 0.5f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y + 0.5001f, (float)z - 0.5f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y - 0.5001f, (float)z - 0.5f));
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y - 0.5001f, (float)z - 0.5f));
					break;
				}
				Vector3 item5 = new Vector3((float)x - 0.5001f, (float)y + 0.5001f, (float)z - 0.5f);
				Vector3 item6 = new Vector3((float)x + 0.5001f, (float)y + 0.5001f, (float)z - 0.5f);
				Vector3 item7 = new Vector3((float)x + 0.5001f, (float)y - 0.5001f, (float)z - 0.5f);
				Vector3 item8 = new Vector3((float)x - 0.5001f, (float)y - 0.5001f, (float)z - 0.5f);
				Vertices.Add(item5);
				SolidColliderVertices.Add(item5);
				Vertices.Add(item6);
				SolidColliderVertices.Add(item6);
				Vertices.Add(item7);
				SolidColliderVertices.Add(item7);
				Vertices.Add(item8);
				SolidColliderVertices.Add(item8);
				break;
			}
			case Facing.down:
			{
				if (!flag)
				{
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y - 0.5f, (float)z - 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y - 0.5f, (float)z - 0.5001f));
					Vertices.Add(new Vector3((float)x + 0.5001f, (float)y - 0.5f, (float)z + 0.5001f));
					Vertices.Add(new Vector3((float)x - 0.5001f, (float)y - 0.5f, (float)z + 0.5001f));
					break;
				}
				Vector3 item13 = new Vector3((float)x - 0.5001f, (float)y - 0.5f, (float)z - 0.5001f);
				Vector3 item14 = new Vector3((float)x + 0.5001f, (float)y - 0.5f, (float)z - 0.5001f);
				Vector3 item15 = new Vector3((float)x + 0.5001f, (float)y - 0.5f, (float)z + 0.5001f);
				Vector3 item16 = new Vector3((float)x - 0.5001f, (float)y - 0.5f, (float)z + 0.5001f);
				Vertices.Add(item13);
				SolidColliderVertices.Add(item13);
				Vertices.Add(item14);
				SolidColliderVertices.Add(item14);
				Vertices.Add(item15);
				SolidColliderVertices.Add(item15);
				Vertices.Add(item16);
				SolidColliderVertices.Add(item16);
				break;
			}
			case Facing.left:
			{
				if (!flag)
				{
					Vertices.Add(new Vector3((float)x - 0.5f, (float)y + 0.5001f, (float)z + 0.5001f));
					Vertices.Add(new Vector3((float)x - 0.5f, (float)y + 0.5001f, (float)z - 0.5001f));
					Vertices.Add(new Vector3((float)x - 0.5f, (float)y - 0.5001f, (float)z - 0.5001f));
					Vertices.Add(new Vector3((float)x - 0.5f, (float)y - 0.5001f, (float)z + 0.5001f));
					break;
				}
				Vector3 item = new Vector3((float)x - 0.5f, (float)y + 0.5001f, (float)z + 0.5001f);
				Vector3 item2 = new Vector3((float)x - 0.5f, (float)y + 0.5001f, (float)z - 0.5001f);
				Vector3 item3 = new Vector3((float)x - 0.5f, (float)y - 0.5001f, (float)z - 0.5001f);
				Vector3 item4 = new Vector3((float)x - 0.5f, (float)y - 0.5001f, (float)z + 0.5001f);
				Vertices.Add(item);
				SolidColliderVertices.Add(item);
				Vertices.Add(item2);
				SolidColliderVertices.Add(item2);
				Vertices.Add(item3);
				SolidColliderVertices.Add(item3);
				Vertices.Add(item4);
				SolidColliderVertices.Add(item4);
				break;
			}
			}
			float textureUnit = Engine.TextureUnit;
			Vector2 textureOffset = Engine.GetTextureOffset(voxelType, (int)RotationsUtility.RotateFacing(facing, rotation));
			float pad = textureUnit * Engine.TexturePadding;
			AssignTopUVs(UVs, pad, textureUnit, textureOffset, (facing == Facing.up || facing == Facing.down) ? rotation : 0);
			list.Add(FaceCount);
			list.Add(FaceCount + 1);
			list.Add(FaceCount + 3);
			list.Add(FaceCount + 1);
			list.Add(FaceCount + 2);
			list.Add(FaceCount + 3);
			if (flag)
			{
				SolidColliderFaces.Add(SolidFaceCount);
				SolidColliderFaces.Add(SolidFaceCount + 1);
				SolidColliderFaces.Add(SolidFaceCount + 3);
				SolidColliderFaces.Add(SolidFaceCount + 1);
				SolidColliderFaces.Add(SolidFaceCount + 2);
				SolidColliderFaces.Add(SolidFaceCount + 3);
			}
			FaceCount += 4;
			if (flag)
			{
				SolidFaceCount += 4;
			}
			if (Vertices.Count > 65530)
			{
				CreateNewMeshObject();
			}
		}

		public static void AssignTopUVs(List<Vector2> uvs, float pad, float tUnit, Vector2 tOffset, int rotation)
		{
			uvs.Add(new Vector2(tUnit * tOffset.x + pad, tUnit * tOffset.y + tUnit - pad));
			uvs.Add(new Vector2(tUnit * tOffset.x + tUnit - pad, tUnit * tOffset.y + tUnit - pad));
			uvs.Add(new Vector2(tUnit * tOffset.x + tUnit - pad, tUnit * tOffset.y + pad));
			uvs.Add(new Vector2(tUnit * tOffset.x + pad, tUnit * tOffset.y + pad));
			if (rotation != 0)
			{
				Vector2[] arr = new Vector2[4]
				{
					uvs[uvs.Count - 4],
					uvs[uvs.Count - 3],
					uvs[uvs.Count - 2],
					uvs[uvs.Count - 1]
				};
				arr = ShiftRight(arr, rotation);
				uvs[uvs.Count - 4] = arr[0];
				uvs[uvs.Count - 3] = arr[1];
				uvs[uvs.Count - 2] = arr[2];
				uvs[uvs.Count - 1] = arr[3];
			}
		}

		public Vector2[] GetTopUVs(float pad, float tUnit, Vector2 tOffset, int rotation)
		{
			Vector2[] arr = new Vector2[4]
			{
				new Vector2(tUnit * tOffset.x + pad, tUnit * tOffset.y + tUnit - pad),
				new Vector2(tUnit * tOffset.x + tUnit - pad, tUnit * tOffset.y + tUnit - pad),
				new Vector2(tUnit * tOffset.x + tUnit - pad, tUnit * tOffset.y + pad),
				new Vector2(tUnit * tOffset.x + pad, tUnit * tOffset.y + pad)
			};
			return ShiftRight(arr, rotation);
		}

		public static Vector2[] ShiftRight(Vector2[] arr, int shitf)
		{
			if (shitf == 0)
			{
				return arr;
			}
			shitf %= arr.Length;
			Vector2[] array = new Vector2[shitf];
			Array.Copy(arr, arr.Length - shitf, array, 0, shitf);
			Array.Copy(arr, 0, arr, shitf, arr.Length - shitf);
			Array.Copy(array, arr, shitf);
			return arr;
		}

		private void AddRotatedVertices(List<Vector3> placeToAdd, Vector3[] arrayToAdd, MeshRotation meshRot, Vector3 blockOffSet)
		{
			switch (meshRot)
			{
			case MeshRotation.back:
				foreach (Vector3 v3 in arrayToAdd)
				{
					placeToAdd.Add(v3.Rotate180() + blockOffSet);
				}
				break;
			case MeshRotation.right:
				foreach (Vector3 v in arrayToAdd)
				{
					placeToAdd.Add(v.Rotate90right() + blockOffSet);
				}
				break;
			case MeshRotation.left:
				foreach (Vector3 v2 in arrayToAdd)
				{
					placeToAdd.Add(v2.Rotate90left() + blockOffSet);
				}
				break;
			default:
				foreach (Vector3 a in arrayToAdd)
				{
					placeToAdd.Add(a + blockOffSet);
				}
				break;
			}
		}

		private void AddAdditionalCustomMeshObject(int x, int y, int z, Mesh mesh, int submeshIndex, MeshRotation rotation, bool withCollider)
		{
			if (additionalCustomMeshPrefab == null)
			{
				additionalCustomMeshPrefab = Resources.Load<GameObject>("additionalCustomMesh");
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(additionalCustomMeshPrefab, meshRenderer.transform);
			gameObject.name = "custom mesh " + mesh.name;
			gameObject.transform.position = meshRenderer.transform.position + new Vector3(x, y, z);
			switch (rotation)
			{
			case MeshRotation.back:
				gameObject.transform.Rotate(0f, 180f, 0f);
				break;
			case MeshRotation.right:
				gameObject.transform.Rotate(0f, 90f, 0f);
				break;
			case MeshRotation.left:
				gameObject.transform.Rotate(0f, -90f, 0f);
				break;
			}
			gameObject.GetComponent<MeshRenderer>().sharedMaterial = materialsCache[submeshIndex];
			gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
			if (withCollider)
			{
				MeshCollider component = gameObject.GetComponent<MeshCollider>();
				component.enabled = true;
				component.sharedMesh = mesh;
			}
		}

		private void CreateCustomMesh(ushort voxel, int x, int y, int z, Mesh mesh, byte rotation, Mesh colliderMesh = null)
		{
			if (mesh == null)
			{
				UnityEngine.Debug.LogError("Uniblocks: The voxel id " + voxel + " uses a custom mesh, but no mesh has been assigned!");
				return;
			}
			Vector3 position = chunk.transform.position + new Vector3(x, y, z);
			Voxel voxelType = Engine.GetVoxelType(voxel, position);
			List<int> list = Faces[voxelType.VSubmeshIndex];
			bool flag = mesh.vertexCount > 200;
			if (Vertices.Count + mesh.vertexCount > 65534)
			{
				CreateNewMeshObject();
			}
			Vector3 blockOffSet = new Vector3(x, y, z);
			MeshRotation meshRotation = RotationsUtility.RotateMeshRotation(rotation, voxelType.VRotation);
			if (!flag)
			{
				AddRotatedVertices(Vertices, mesh.vertices, meshRotation, blockOffSet);
			}
			Vector2 textureOffset = Engine.GetTextureOffset(voxelType, 0);
			if (voxelType.moveUvsByVTexture)
			{
				Vector2[] uv = mesh.uv;
				for (int i = 0; i < uv.Length; i++)
				{
					Vector2 vector = uv[i];
					UVs.Add(new Vector2((vector.x + textureOffset.x) / 32f, (vector.y + textureOffset.y) / 32f));
				}
			}
			else if (!flag)
			{
				Vector2[] uv2 = mesh.uv;
				foreach (Vector2 item in uv2)
				{
					UVs.Add(item);
				}
			}
			if (!flag)
			{
				int[] triangles = mesh.triangles;
				foreach (int num in triangles)
				{
					list.Add(FaceCount + num);
				}
				FaceCount += mesh.vertexCount;
			}
			switch (voxelType.VColliderType)
			{
			case ColliderType.cube:
				AddCubeMesh(x, y, z, solid: true, voxelType.VColliderHeight);
				if (flag)
				{
					AddAdditionalCustomMeshObject(x, y, z, mesh, voxelType.VSubmeshIndex, meshRotation, withCollider: false);
				}
				return;
			case ColliderType.mesh:
			{
				if (colliderMesh == null)
				{
					colliderMesh = mesh;
				}
				if (flag)
				{
					AddAdditionalCustomMeshObject(x, y, z, mesh, voxelType.VSubmeshIndex, meshRotation, withCollider: false);
				}
				AddRotatedVertices(SolidColliderVertices, colliderMesh.vertices, meshRotation, blockOffSet);
				int[] triangles2 = colliderMesh.triangles;
				foreach (int num2 in triangles2)
				{
					SolidColliderFaces.Add(SolidFaceCount + num2);
				}
				SolidFaceCount += colliderMesh.vertexCount;
				return;
			}
			}
			if (voxel != 0)
			{
				AddCubeMesh(x, y, z, solid: false, voxelType.VColliderHeight);
				if (flag)
				{
					AddAdditionalCustomMeshObject(x, y, z, mesh, voxelType.VSubmeshIndex, meshRotation, withCollider: false);
				}
			}
		}

		private void AddCubeMesh(int x, int y, int z, bool solid, float colliderHeight = 1f)
		{
			if (solid)
			{
				Vector3[] array = cubeVertices;
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 a = array[i];
					if (a.y > 0f)
					{
						SolidColliderVertices.Add(a + new Vector3(x, (float)y - (1f - colliderHeight), z));
					}
					else
					{
						SolidColliderVertices.Add(a + new Vector3(x, y, z));
					}
				}
				int[] array2 = cubeTriangles;
				foreach (int num in array2)
				{
					SolidColliderFaces.Add(SolidFaceCount + num);
				}
				SolidFaceCount += cubeVertices.Length;
			}
			else
			{
				Vector3[] array3 = cubeVertices;
				foreach (Vector3 a2 in array3)
				{
					NoCollideVertices.Add(a2 + new Vector3(x, y, z));
				}
				int[] array4 = cubeTriangles;
				foreach (int num2 in array4)
				{
					NoCollideFaces.Add(NoCollideFaceCount + num2);
				}
				NoCollideFaceCount += cubeVertices.Length;
			}
		}

		private void UpdateMesh(Mesh mesh, MeshCollider collider = null)
		{
			mesh.Clear();
			mesh.SetVertices(Vertices);
			mesh.subMeshCount = materialsLength;
			int num = 0;
			for (int i = 0; i < materialsLength; i++)
			{
				if (Faces[i].Count > 0)
				{
					num++;
				}
			}
			int num2 = 0;
			Material[] array = new Material[num];
			for (int j = 0; j < materialsLength; j++)
			{
				if (Faces[j].Count > 0)
				{
					mesh.SetTriangles(Faces[j], num2);
					array[num2] = materialsCache[j];
					num2++;
				}
			}
			meshRenderer.sharedMaterials = array;
			mesh.SetUVs(0, UVs);
			mesh.RecalculateNormals();
			Mesh mesh2 = new Mesh();
			mesh2.SetVertices(SolidColliderVertices);
			mesh2.SetTriangles(SolidColliderFaces, 0);
			mesh2.RecalculateNormals();
			collider.sharedMesh = null;
			collider.sharedMesh = mesh2;
			Vertices.Clear();
			UVs.Clear();
			foreach (List<int> face in Faces)
			{
				face.Clear();
			}
			SolidColliderVertices.Clear();
			SolidColliderFaces.Clear();
			NoCollideVertices.Clear();
			NoCollideFaces.Clear();
			FaceCount = 0;
			SolidFaceCount = 0;
			NoCollideFaceCount = 0;
		}

		private void CreateNewMeshObject()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(chunk.MeshContainer, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = chunk.transform;
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			component.mesh = new Mesh();
			gameObject.GetComponent<Renderer>().sharedMaterials = materialsCache;
			UpdateMesh(component.mesh, gameObject.GetComponent<MeshCollider>());
		}
	}
}
