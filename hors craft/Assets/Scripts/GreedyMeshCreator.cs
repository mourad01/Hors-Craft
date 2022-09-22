// DecompilerFi decompiler from Assembly-CSharp.dll class: GreedyMeshCreator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Uniblocks;
using UnityEngine;

public class GreedyMeshCreator : MonoBehaviour, IMeshCreator
{
	public int maxDataAmountMod;

	public static int workerThreadsCount;

	public static int completionWorkThreadsCount;

	public static Dictionary<ushort, CustomMeshData> customMeshDict;

	private Queue<MeshData> readyData = new Queue<MeshData>();

	private Queue<Action> queuedGenerations = new Queue<Action>();

	private MeshData[] dataArray;

	private int maxDataAmount = 10;

	private int maxMeshGenerationPerFrame = 10;

	private Index playerStartingChunkIndex;

	private bool firstBuild;

	private Vector3[] cubeVertices;

	private int[] cubeTriangles;

	private const int MAX_MATERIALS = 20;

	private int blueprintIndex = 1118;

	private readonly int[] permutation = new int[3]
	{
		1,
		0,
		2
	};

	private readonly int[,] perms = new int[3, 2]
	{
		{
			2,
			1
		},
		{
			0,
			2
		},
		{
			0,
			1
		}
	};

	private readonly float[] ambientOcclusionValues = new float[5]
	{
		1f,
		0.7f,
		0.5f,
		0.4f,
		0.4f
	};

	public bool ambientOcclusion = true;

	public bool edgeNormals = true;

	public bool useThreading = true;

	public bool tangents = true;

	public Material[] materialsCache;

	public static Material blackMaterial;

	public bool meshCollider = true;

	private int[][][] maskHelper;

	private readonly HashSet<int> addedCustoms = new HashSet<int>();

	private int sidePower = 4;

	private int sideSquaredPower = 8;

	private GameObject chunkPrefab;

	public int maxVerticesInChunkCustomMesh = 210;

	private GameObject additionalCustomMeshPrefab;

	private int materialsLength;

	private static int[] verticesLengthPowers;

	private void Start()
	{
		maxDataAmount += maxDataAmountMod;
		maxMeshGenerationPerFrame += maxDataAmountMod;
		firstBuild = true;
		if (ambientOcclusion)
		{
			InitAOVariables();
		}
		InitCustomMeshDict();
		sidePower = fastLog2(ChunkData.SideLength);
		sideSquaredPower = fastLog2(ChunkData.SquaredSideLength);
		chunkPrefab = Engine.ChunkManagerInstance.ChunkObject;
	}

	private int fastLog2(int powerOfTwo)
	{
		int num = 1;
		int num2 = 0;
		while ((powerOfTwo & num) == 0 && num2 < 32)
		{
			num <<= 1;
			num2++;
		}
		return num2;
	}

	public void InitMeshDataArray()
	{
		dataArray = new MeshData[maxDataAmount];
		for (int i = 0; i < maxDataAmount; i++)
		{
			dataArray[i] = new MeshData();
			dataArray[i].Init(materialsLength, ambientOcclusion, edgeNormals, tangents);
		}
	}

	private void InitCustomMeshDict()
	{
		customMeshDict = new Dictionary<ushort, CustomMeshData>();
		CustomMeshData customMeshData = new CustomMeshData();
		Voxel[] blocks = Engine.Blocks;
		foreach (Voxel voxel in blocks)
		{
			if (!(voxel.VMesh != null) || !voxel.VCustomMesh)
			{
				continue;
			}
			Mesh vMesh = voxel.VMesh;
			customMeshData.vertices = vMesh.vertices;
			if (customMeshData.vertices.Length > maxVerticesInChunkCustomMesh)
			{
				continue;
			}
			customMeshData.triangles = vMesh.triangles;
			customMeshData.uvs = vMesh.uv;
			customMeshData.uvs2 = new Vector2[customMeshData.uvs.Length];
			float textureUnit = Engine.TextureUnit;
			Vector2 vector = Engine.GetTextureOffset(voxel, 0) * textureUnit;
			if (voxel.moveUvsByVTexture)
			{
				for (int j = 0; j < customMeshData.uvs.Length; j++)
				{
					Vector2 vector2 = customMeshData.uvs[j];
					customMeshData.uvs[j] = new Vector2(vector2.x * textureUnit + vector.x, vector2.y * textureUnit + vector.y);
				}
			}
			for (int k = 0; k < customMeshData.uvs.Length; k++)
			{
				customMeshData.uvs2[k] = vector;
			}
			customMeshData.tangents = new Vector4[customMeshData.vertices.Length];
			Vector4 vector3 = new Vector4(-1f, 0f, 0f, -1f);
			for (int l = 0; l < customMeshData.tangents.Length; l++)
			{
				customMeshData.tangents[l] = vector3;
			}
			customMeshData.colors = vMesh.colors;
			if (customMeshData.colors == null || customMeshData.colors.Length == 0)
			{
				customMeshData.colors = new Color[customMeshData.vertices.Length];
				Color white = Color.white;
				for (int m = 0; m < customMeshData.colors.Length; m++)
				{
					customMeshData.colors[m] = white;
				}
			}
			customMeshDict.Add(voxel.GetUniqueID(), customMeshData);
			customMeshData = new CustomMeshData();
		}
	}

	private void Update()
	{
		HandleQueuedGenerations();
		lock (readyData)
		{
			if (readyData.Count > 0)
			{
				HandleFirstBuild();
				HandleReadyDataProcessing();
			}
		}
	}

	private void HandleReadyDataProcessing()
	{
		for (int i = 0; i < maxMeshGenerationPerFrame; i++)
		{
			if (readyData.Count <= 0)
			{
				break;
			}
			MeshData data = readyData.Dequeue();
			ProcessMeshData(data);
		}
	}

	private void HandleQueuedGenerations()
	{
		lock (queuedGenerations)
		{
			for (int i = 0; i < maxMeshGenerationPerFrame; i++)
			{
				if (queuedGenerations.Count <= 0)
				{
					break;
				}
				Action action = queuedGenerations.Dequeue();
				action();
			}
		}
	}

	private void HandleFirstBuild()
	{
		if (firstBuild)
		{
			playerStartingChunkIndex = Engine.PositionToIndex(PlayerGraphic.GetControlledPlayerInstance().transform.position);
			readyData = new Queue<MeshData>(from d in readyData
				orderby IsChunkUnderPlayer(d.chunkData.ChunkIndex)
				select d);
			firstBuild = false;
		}
	}

	private MeshData GetFreeData()
	{
		MeshData meshData = dataArray.FirstOrDefault((MeshData d) => d.isFree);
		if (meshData != null)
		{
			meshData.isFree = false;
		}
		return meshData;
	}

	public void LogSizes()
	{
	}

	public void Init()
	{
		blueprintIndex = Engine.usefulIDs.blueprintID;
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
		if (!blackMaterial)
		{
			blackMaterial = new Material(Shader.Find("Unlit/Color"));
			blackMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
			blackMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public void InitSubmeshArrays(Material[] sharedMaterials, Chunk chunk)
	{
		materialsCache = new List<Material>(sharedMaterials).ToArray();
		materialsLength = sharedMaterials.Length;
		HackShaderChange(chunk);
		InitMeshDataArray();
	}

	private void HackShaderChange(Chunk chunk)
	{
		for (int i = 0; i < materialsCache.Length; i++)
		{
			if (Array.IndexOf(chunk.tilesetMaterials, materialsCache[i]) >= 0)
			{
				if (materialsCache[i].shader.name == "_X/Diffuse [NoBatch]")
				{
					Material material = new Material(materialsCache[i]);
					material.shader = Shader.Find("_X/Greedy Diffuse [NoBatch]");
					materialsCache[i] = material;
				}
				else if (materialsCache[i].shader.name == "_X/Diffuse + CutOff [NoBatch]")
				{
					Material material2 = new Material(materialsCache[i]);
					material2.shader = Shader.Find("_X/Greedy Diffuse + CutOff [NoBatch]");
					materialsCache[i] = material2;
				}
				else if (materialsCache[i].shader.name == "_X/Diffuse + Color [NoBatch]")
				{
					Material material3 = new Material(materialsCache[i]);
					material3.shader = Shader.Find("_X/Greedy Diffuse + Color [NoBatch]");
					materialsCache[i] = material3;
				}
				else if (materialsCache[i].shader.name == "_X/Diffuse + Transparent")
				{
					Material material4 = new Material(materialsCache[i]);
					material4.shader = Shader.Find("_X/Greedy Diffuse + Transparent");
					materialsCache[i] = material4;
				}
			}
		}
	}

	public void Generate(ChunkData chunk, bool triggerVoxelEvents = true)
	{
		MeshData freeData = GetFreeData();
		if (freeData == null)
		{
			lock (queuedGenerations)
			{
				queuedGenerations.Enqueue(delegate
				{
					Generate(chunk, triggerVoxelEvents);
				});
			}
			return;
		}
		if (chunk == null)
		{
			freeData.Clear();
			return;
		}
		freeData.PrepareData(chunk, triggerVoxelEvents);
		if (useThreading && !chunk.rebuildOnMainThread)
		{
			ThreadPool.QueueUserWorkItem(RebuildMesh, freeData);
		}
		else
		{
			RebuildMesh(freeData);
		}
	}

	private void CalculateAmbientOcclusionInVertices(MeshData data)
	{
		data.x[0] = -1;
		data.x[1] = -1;
		while (data.x[1] < verticesLengthPowers[1])
		{
			data.x[2] = -1;
			while (data.x[2] < verticesLengthPowers[1])
			{
				CalculateAmbientWithChecks(data);
				data.x[2]++;
			}
			data.x[1]++;
		}
		data.x[0] = 0;
		while (data.x[0] < data.SideLength)
		{
			data.x[1] = -1;
			data.x[2] = -1;
			while (data.x[2] < verticesLengthPowers[1])
			{
				CalculateAmbientWithChecks(data);
				data.x[2]++;
			}
			data.x[1] = 0;
			while (data.x[1] < data.SideLength)
			{
				data.x[2] = -1;
				CalculateAmbientWithChecks(data);
				data.x[2] = 0;
				while (data.x[2] < data.SideLength)
				{
					CalculateAmbientWithoutChecks(data);
					data.x[2]++;
				}
				data.x[2] = data.SideLength;
				CalculateAmbientWithChecks(data);
				data.x[1]++;
			}
			data.x[1] = data.SideLength;
			data.x[2] = -1;
			while (data.x[2] < verticesLengthPowers[1])
			{
				CalculateAmbientWithChecks(data);
				data.x[2]++;
			}
			data.x[0]++;
		}
		data.x[0] = data.SideLength;
		data.x[1] = -1;
		while (data.x[1] < verticesLengthPowers[1])
		{
			data.x[2] = -1;
			while (data.x[2] < verticesLengthPowers[1])
			{
				CalculateAmbientWithChecks(data);
				data.x[2]++;
			}
			data.x[1]++;
		}
	}

	private void CalculateAmbientWithChecks(MeshData data)
	{
		ushort voxel = data.chunkData.GetVoxel(data.x[0], data.x[1], data.x[2]);
		switch (voxel)
		{
		case 0:
		case 12:
			return;
		default:
			if (Engine.GetVoxelType(voxel).VTransparency != 0)
			{
				return;
			}
			break;
		case 1:
		case 2:
		case 5:
			break;
		}
		byte b = 1;
		for (int i = data.x[0]; i < data.x[0] + 2; i++)
		{
			for (int j = data.x[1]; j < data.x[1] + 2; j++)
			{
				for (int k = data.x[2]; k < data.x[2] + 2; k++)
				{
					if (i >= 0 && k >= 0 && j >= 0 && i <= data.SideLength && k <= data.SideLength && j <= data.SideLength)
					{
						data.voxelVerticesAO[i * verticesLengthPowers[2] + j * verticesLengthPowers[1] + k] += b;
					}
					b = (byte)(b * 2);
				}
			}
		}
	}

	private void CalculateAmbientWithoutChecks(MeshData data)
	{
		ushort voxelSimple = data.chunkData.GetVoxelSimple(data.x[0], data.x[1], data.x[2]);
		switch (voxelSimple)
		{
		case 0:
		case 12:
			return;
		default:
			if (Engine.GetVoxelType(voxelSimple).VTransparency != 0)
			{
				return;
			}
			break;
		case 1:
		case 2:
		case 5:
			break;
		}
		int num = data.x[0] * verticesLengthPowers[2] + data.x[1] * verticesLengthPowers[1] + data.x[2];
		data.voxelVerticesAO[num]++;
		data.voxelVerticesAO[num + 1] += 2;
		data.voxelVerticesAO[num + verticesLengthPowers[1]] += 4;
		data.voxelVerticesAO[num + verticesLengthPowers[1] + 1] += 8;
		data.voxelVerticesAO[num + verticesLengthPowers[2]] += 16;
		data.voxelVerticesAO[num + verticesLengthPowers[2] + 1] += 32;
		data.voxelVerticesAO[num + verticesLengthPowers[2] + verticesLengthPowers[1]] += 64;
		data.voxelVerticesAO[num + verticesLengthPowers[2] + verticesLengthPowers[1] + 1] += 128;
	}

	private void RebuildMesh(object dataObj)
	{
		MeshData meshData = (MeshData)dataObj;
		if (ambientOcclusion)
		{
			CalculateAmbientOcclusionInVertices(meshData);
		}
		List<Rect> list = new List<Rect>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		for (int i = 0; i < 3; i++)
		{
			int num = (i + 1) % 3;
			int num2 = (i + 2) % 3;
			meshData.x[0] = (meshData.x[1] = (meshData.x[2] = 0));
			meshData.q[0] = (meshData.q[1] = (meshData.q[2] = 0));
			meshData.y[0] = (meshData.y[1] = (meshData.y[2] = 0));
			meshData.q[i] = 1;
			meshData.x[i] = -1;
			while (meshData.x[i] < meshData.SideLength)
			{
				int num3 = 0;
				meshData.x[num2] = 0;
				while (meshData.x[num2] < meshData.SideLength)
				{
					meshData.x[num] = 0;
					while (meshData.x[num] < meshData.SideLength)
					{
						int num4 = meshData.x[0] + meshData.SideLength * meshData.x[1] + meshData.SquaredSideLength * meshData.x[2];
						meshData.y[0] = meshData.x[0] + meshData.q[0];
						meshData.y[1] = meshData.x[1] + meshData.q[1];
						meshData.y[2] = meshData.x[2] + meshData.q[2];
						ushort num5 = (meshData.x[i] < 0) ? meshData.chunkData.GetVoxel(meshData.x[0], meshData.x[1], meshData.x[2]) : meshData.chunkData.VoxelData[meshData.x[2] * meshData.SquaredSideLength + meshData.x[1] * meshData.SideLength + meshData.x[0]];
						ushort num6 = (meshData.y[i] >= meshData.SideLength) ? meshData.chunkData.GetVoxel(meshData.y[0], meshData.y[1], meshData.y[2]) : meshData.chunkData.VoxelData[meshData.y[2] * meshData.SquaredSideLength + meshData.y[1] * meshData.SideLength + meshData.y[0]];
						if ((num5 == 0 && num6 == 0) || (num5 == 5 && num6 == 5) || (num5 == 12 && num6 == 12))
						{
							meshData.mask[num3] = 0;
							meshData.maskIndices[num3] = num4;
						}
						else
						{
							Voxel voxel;
							if (num5 != blueprintIndex)
							{
								voxel = ((num5 < Engine.Blocks.Length) ? Engine.Blocks[num5] : Engine.Blocks[0]);
							}
							else
							{
								Vector3 chunkPosition = meshData.chunkPosition;
								chunkPosition.x += meshData.x[0];
								chunkPosition.y += meshData.x[1];
								chunkPosition.z += meshData.x[2];
								voxel = Engine.GetVoxelType(num5, chunkPosition);
							}
							Voxel voxel2;
							if (num6 != blueprintIndex)
							{
								voxel2 = ((num6 < Engine.Blocks.Length) ? Engine.Blocks[num6] : Engine.Blocks[0]);
							}
							else
							{
								Vector3 chunkPosition2 = meshData.chunkPosition;
								chunkPosition2.x += meshData.y[0];
								chunkPosition2.y += meshData.y[1];
								chunkPosition2.z += meshData.y[2];
								voxel2 = Engine.GetVoxelType(num6, chunkPosition2);
							}
							Transparency vTransparency = voxel.VTransparency;
							bool vCustomMesh = voxel.VCustomMesh;
							bool hasStartBehaviour = voxel.hasStartBehaviour;
							if (meshData.x[i] >= 0 && DrawThisVoxelsFace(num5, vTransparency, vCustomMesh, num6, voxel2.VTransparency, voxel2.VCustomMesh, meshData.q[1] == 1))
							{
								meshData.mask[num3] = num5;
								meshData.maskIndices[num3] = num4;
							}
							else if (meshData.x[i] + 1 < meshData.SideLength && DrawThisVoxelsFace(num6, voxel2.VTransparency, voxel2.VCustomMesh, num5, vTransparency, vCustomMesh, fromUp: false))
							{
								meshData.mask[num3] = -num6;
								meshData.maskIndices[num3] = num4 + meshData.q[0] + meshData.SideLength * meshData.q[1] + meshData.SquaredSideLength * meshData.q[2];
							}
							else
							{
								meshData.mask[num3] = 0;
								meshData.maskIndices[num3] = num4;
							}
							if ((num6 == ushort.MaxValue && meshData.q[1] != 1) || num5 == ushort.MaxValue)
							{
								meshData.mask[num3] = 0;
							}
							if (meshData.x[i] >= 0)
							{
								if (vCustomMesh && !meshData.customblocks.ContainsKey(num4))
								{
									meshData.customblocks.Add(num4, num5);
								}
								if (hasStartBehaviour && !meshData.visited.Contains(num4))
								{
									meshData.visited.Add(num4);
									if (meshData.triggerVoxelEvents)
									{
										MeshData.MeshDataVoxelEvents item = default(MeshData.MeshDataVoxelEvents);
										item.voxelID = num5;
										item.x0 = meshData.x[0];
										item.x1 = meshData.x[1];
										item.x2 = meshData.x[2];
										meshData.eventsData.Add(item);
									}
								}
							}
						}
						meshData.x[num]++;
						num3++;
					}
					meshData.x[num2]++;
				}
				meshData.x[i]++;
				num3 = 0;
				list.Clear();
				list2.Clear();
				list3.Clear();
				float[] array = new float[3];
				float[] array2 = new float[3];
				meshData.x[num2] = 0;
				while (meshData.x[num2] < meshData.SideLength)
				{
					meshData.x[num] = 0;
					while (meshData.x[num] < meshData.SideLength)
					{
						int num7 = meshData.mask[num3];
						if (num7 != 0)
						{
							int w = 1;
							int h = 1;
							CalculateRect(meshData, num7, i, num, num2, num3, out w, out h);
							array[0] = 0f;
							array[1] = 0f;
							array[2] = 0f;
							array2[0] = 0f;
							array2[1] = 0f;
							array2[2] = 0f;
							int num8 = (num7 > 0) ? 1 : (-1);
							int num9 = permutation[i] * 2;
							if (num8 > 0)
							{
								array2[num2] = h;
								array[num] = w;
							}
							else
							{
								num7 = -num7;
								num9++;
								array[num2] = h;
								array2[num] = w;
							}
							list2.Add(meshData.Vertices.Count);
							list.Add(new Rect(meshData.x[num], meshData.x[num2], w, h));
							list3.Add(num8);
							int num10 = meshData.maskIndices[num3];
							CreateFace(pos: new Vector3(num10 % meshData.SideLength, (num10 >> sidePower) % meshData.SideLength, num10 >> sideSquaredPower), data: meshData, voxel: (ushort)num7, x: meshData.x, du: array, dv: array2, u: perms[i, 0], v: perms[i, 1], facing: num9, dir: num8, dim: i);
							for (int j = 0; j < h; j++)
							{
								for (int k = 0; k < w; k++)
								{
									meshData.mask[num3 + k + j * meshData.SideLength] = 0;
								}
							}
							meshData.x[num] += w;
							num3 += w;
						}
						else
						{
							meshData.x[num]++;
							num3++;
						}
					}
					meshData.x[num2]++;
				}
				int count = list.Count;
				for (int l = 0; l < count; l++)
				{
					Rect rect = list[l];
					for (int m = 0; m < count; m++)
					{
						if (l == m || list3[l] != list3[m])
						{
							continue;
						}
						Rect rect2 = list[m];
						if (rect.xMax == rect2.xMin && rect.yMax > rect2.yMin && rect2.yMax > rect.yMin)
						{
							if (list3[l] == 1)
							{
								meshData.StichedFaces.Add(list2[l] + 1);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[l] + 2);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[m] + 3);
								meshData.StichedFaces.Add(list2[l] + 2);
							}
							else
							{
								meshData.StichedFaces.Add(list2[l] + 3);
								meshData.StichedFaces.Add(list2[l] + 2);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[l] + 2);
								meshData.StichedFaces.Add(list2[m] + 1);
								meshData.StichedFaces.Add(list2[m]);
							}
						}
						else if (rect.yMax == rect2.yMin && rect.xMax > rect2.xMin && rect2.xMax > rect.xMin)
						{
							if (list3[l] == 1)
							{
								meshData.StichedFaces.Add(list2[l] + 2);
								meshData.StichedFaces.Add(list2[m] + 1);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[l] + 3);
								meshData.StichedFaces.Add(list2[l] + 2);
							}
							else
							{
								meshData.StichedFaces.Add(list2[l] + 1);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[l] + 2);
								meshData.StichedFaces.Add(list2[m]);
								meshData.StichedFaces.Add(list2[m] + 3);
								meshData.StichedFaces.Add(list2[l] + 2);
							}
						}
					}
				}
			}
		}
		lock (readyData)
		{
			readyData.Enqueue(meshData);
		}
	}

	private void CalculateRect(MeshData data, int c, int dim, int u, int v, int index, out int w, out int h)
	{
		if (ambientOcclusion)
		{
			if (edgeNormals)
			{
				CalculateRectWithAmbientAndNormals(data, c, dim, u, v, index, out w, out h);
			}
			else
			{
				CalculateRectWithAmbient(data, c, dim, u, v, index, out w, out h);
			}
		}
		else
		{
			CalculateRectWithoutAmbient(data, c, dim, u, v, index, out w, out h);
		}
	}

	private void CalculateRectWithoutAmbient(MeshData data, int c, int dim, int u, int v, int index, out int w, out int h)
	{
		w = 1;
		h = 1;
		while (data.x[u] + w < data.SideLength && SameVoxels(c, data.mask[index + w]))
		{
			w++;
		}
		int num = 0;
		while (data.x[v] + h < data.SideLength && SameVoxels(c, data.mask[index + num + h * data.SideLength]))
		{
			num++;
			if (num == w)
			{
				num = 0;
				h++;
			}
		}
	}

	private bool SameVoxels(int a, int b)
	{
		return a == b && a != blueprintIndex;
	}

	private void CalculateRectWithAmbient(MeshData data, int c, int dim, int u, int v, int index, out int w, out int h)
	{
		w = 1;
		h = 1;
		int dir = (c > 0) ? 1 : (-1);
		int num = verticesLengthPowers[2 - u];
		int num2 = verticesLengthPowers[2 - v];
		int num3 = verticesLengthPowers[2 - dim];
		int[] array = new int[4];
		array[0] = data.x[dim] * num3 + data.x[u] * num + data.x[v] * num2;
		array[1] = array[0] + num;
		array[2] = array[0] + num2;
		array[3] = array[0] + num + num2;
		bool flag = SameAmbient(data, array[0], array[1], dim, dir) && SameAmbient(data, array[2], array[3], dim, dir);
		if (flag)
		{
			array[1] += num;
			array[3] += num;
			flag = (SameAmbient(data, array[0], array[1], dim, dir) && SameAmbient(data, array[2], array[3], dim, dir));
		}
		while (flag && data.x[u] + w < data.SideLength && SameVoxels(c, data.mask[index + w]))
		{
			w++;
			array[1] += num;
			array[3] += num;
			flag = (SameAmbient(data, array[0], array[1], dim, dir) && SameAmbient(data, array[2], array[3], dim, dir));
		}
		array[1] = array[0] + num;
		array[3] = array[0] + num + num2;
		if (!SameAmbient(data, array[0], array[2], dim, dir) || !SameAmbient(data, array[1], array[3], dim, dir) || (w != 1 && !SameAmbient(data, array[2], array[3], dim, dir)))
		{
			return;
		}
		array[2] += num2;
		array[3] += num2;
		int num4 = array[3];
		flag = (SameAmbient(data, array[0], array[2], dim, dir) && SameAmbient(data, array[1], array[3], dim, dir) && (w == 1 || SameAmbient(data, array[2], array[3], dim, dir)));
		int num5 = 0;
		while (flag && data.x[v] + h < data.SideLength && SameVoxels(c, data.mask[index + num5 + h * data.SideLength]))
		{
			num5++;
			if (num5 == w)
			{
				num5 = 0;
				h++;
				array[2] += num2;
				array[3] += num2;
				num4 = array[3];
				flag = (SameAmbient(data, array[0], array[2], dim, dir) && SameAmbient(data, array[1], array[3], dim, dir) && (w == 1 || SameAmbient(data, array[2], array[3], dim, dir)));
			}
			else
			{
				num4 += num;
				flag = SameAmbient(data, array[2], num4, dim, dir);
			}
		}
	}

	private bool SameAmbient(MeshData data, int vertexIndexOne, int vertexIndexTwo, int dim, int dir)
	{
		byte b = data.voxelVerticesAO[vertexIndexOne];
		byte b2 = data.voxelVerticesAO[vertexIndexTwo];
		if (dir == 1)
		{
			switch (dim)
			{
			case 0:
				return (b & 1) + ((b & 2) >> 1) + ((b & 4) >> 2) + ((b & 8) >> 3) == (b2 & 1) + ((b2 & 2) >> 1) + ((b2 & 4) >> 2) + ((b2 & 8) >> 3);
			case 1:
				return (b & 1) + ((b & 2) >> 1) + ((b & 0x10) >> 4) + ((b & 0x20) >> 5) == (b2 & 1) + ((b2 & 2) >> 1) + ((b2 & 0x10) >> 4) + ((b2 & 0x20) >> 5);
			default:
				return (b & 1) + ((b & 0x10) >> 4) + ((b & 4) >> 2) + ((b & 0x40) >> 6) == (b2 & 1) + ((b2 & 0x10) >> 4) + ((b2 & 4) >> 2) + ((b2 & 0x40) >> 6);
			}
		}
		switch (dim)
		{
		case 0:
			return ((b & 0x10) >> 4) + ((b & 0x20) >> 5) + ((b & 0x40) >> 6) + ((b & 0x80) >> 7) == ((b2 & 0x10) >> 4) + ((b2 & 0x20) >> 5) + ((b2 & 0x40) >> 6) + ((b2 & 0x80) >> 7);
		case 1:
			return ((b & 4) >> 2) + ((b & 8) >> 3) + ((b & 0x40) >> 6) + ((b & 0x80) >> 7) == ((b2 & 4) >> 2) + ((b2 & 8) >> 3) + ((b2 & 0x40) >> 6) + ((b2 & 0x80) >> 7);
		default:
			return ((b & 2) >> 1) + ((b & 0x20) >> 5) + ((b & 8) >> 3) + ((b & 0x80) >> 7) == ((b2 & 2) >> 1) + ((b2 & 0x20) >> 5) + ((b2 & 8) >> 3) + ((b2 & 0x80) >> 7);
		}
	}

	private void CalculateRectWithAmbientAndNormals(MeshData data, int c, int dim, int u, int v, int index, out int w, out int h)
	{
		w = 1;
		h = 1;
		int dir = (c > 0) ? 1 : (-1);
		int num = verticesLengthPowers[2 - u];
		int num2 = verticesLengthPowers[2 - v];
		int num3 = verticesLengthPowers[2 - dim];
		data.rectIndices[0] = data.x[dim] * num3 + data.x[u] * num + data.x[v] * num2;
		data.rectIndices[1] = data.rectIndices[0] + num;
		data.rectIndices[2] = data.rectIndices[0] + num + num2;
		data.rectIndices[3] = data.rectIndices[0] + num2;
		data.verticesUVs[0] = GetUVs(data.voxelVerticesAO[data.rectIndices[0]], dim, dir, perms[dim, 0], perms[dim, 1], 0, data);
		data.verticesUVs[1] = GetUVs(data.voxelVerticesAO[data.rectIndices[1]], dim, dir, perms[dim, 0], perms[dim, 1], 1, data);
		data.verticesUVs[2] = GetUVs(data.voxelVerticesAO[data.rectIndices[2]], dim, dir, perms[dim, 0], perms[dim, 1], 2, data);
		data.verticesUVs[3] = GetUVs(data.voxelVerticesAO[data.rectIndices[3]], dim, dir, perms[dim, 0], perms[dim, 1], 3, data);
		data.verticesUVs[4] = data.verticesUVs[0];
		data.verticesUVs[5] = data.verticesUVs[1];
		data.verticesUVs[6] = data.verticesUVs[2];
		data.verticesUVs[7] = data.verticesUVs[3];
		bool flag;
		if (SameAmbientAndNormals(data, dim, dir, 0, 1, calculateFirstNormal: false, calculateSecondNormal: false) && SameAmbientAndNormals(data, dim, dir, 2, 3, calculateFirstNormal: false, calculateSecondNormal: false))
		{
			data.rectIndices[1] += num;
			data.rectIndices[2] += num;
			for (flag = (SameAmbientAndNormals(data, dim, dir, 0, 1, calculateFirstNormal: false, calculateSecondNormal: true) && SameAmbientAndNormals(data, dim, dir, 2, 3, calculateFirstNormal: true, calculateSecondNormal: false)); flag && data.x[u] + w < data.SideLength && SameVoxels(c, data.mask[index + w]); flag = (SameAmbientAndNormals(data, dim, dir, 0, 1, calculateFirstNormal: false, calculateSecondNormal: true) && SameAmbientAndNormals(data, dim, dir, 2, 3, calculateFirstNormal: true, calculateSecondNormal: false)))
			{
				w++;
				data.verticesUVs[5] = data.verticesUVs[1];
				data.rectIndices[1] += num;
				data.rectIndices[2] += num;
			}
		}
		data.rectIndices[1] = data.rectIndices[0] + num;
		data.rectIndices[2] = data.rectIndices[0] + num + num2;
		data.verticesUVs[1] = data.verticesUVs[5];
		data.verticesUVs[2] = data.verticesUVs[6];
		if (!SameAmbientAndNormals(data, dim, dir, 0, 3, calculateFirstNormal: false, calculateSecondNormal: false) || !SameAmbientAndNormals(data, dim, dir, 1, 2, calculateFirstNormal: false, calculateSecondNormal: false) || (w != 1 && !SameAmbientAndNormals(data, dim, dir, 2, 3, calculateFirstNormal: false, calculateSecondNormal: false)))
		{
			return;
		}
		data.rectIndices[2] += num2;
		data.rectIndices[3] += num2;
		int num4 = data.rectIndices[2];
		flag = (SameAmbientAndNormals(data, dim, dir, 0, 3, calculateFirstNormal: false, calculateSecondNormal: true) && SameAmbientAndNormals(data, dim, dir, 1, 2, calculateFirstNormal: false, calculateSecondNormal: true) && (w == 1 || SameAmbientAndNormals(data, dim, dir, 2, 3, calculateFirstNormal: false, calculateSecondNormal: false)));
		Vector2 vector = data.verticesUVs[2];
		int num5 = 0;
		while (flag && data.x[v] + h < data.SideLength && SameVoxels(c, data.mask[index + num5 + h * data.SideLength]))
		{
			num5++;
			if (num5 == w)
			{
				num5 = 0;
				h++;
				data.verticesUVs[6] = vector;
				data.verticesUVs[7] = data.verticesUVs[3];
				data.rectIndices[2] += num2;
				data.rectIndices[3] += num2;
				num4 = data.rectIndices[2];
				flag = (SameAmbientAndNormals(data, dim, dir, 0, 3, calculateFirstNormal: false, calculateSecondNormal: true) && SameAmbientAndNormals(data, dim, dir, 1, 2, calculateFirstNormal: false, calculateSecondNormal: true) && (w == 1 || SameAmbientAndNormals(data, dim, dir, 2, 3, calculateFirstNormal: false, calculateSecondNormal: false)));
			}
			else
			{
				num4 += num;
				vector = GetUVs(data.voxelVerticesAO[num4], dim, dir, perms[dim, 0], perms[dim, 1], 2, data);
				flag = (SameAmbient(data, data.rectIndices[2], num4, dim, dir) && data.verticesUVs[2].Equals(vector, 0, 0));
			}
		}
	}

	private bool SameAmbientAndNormals(MeshData data, int dim, int dir, int i1, int i2, bool calculateFirstNormal, bool calculateSecondNormal)
	{
		if (SameAmbient(data, data.rectIndices[i1], data.rectIndices[i2], dim, dir))
		{
			if (calculateFirstNormal)
			{
				data.verticesUVs[i1] = GetUVs(data.voxelVerticesAO[data.rectIndices[i1]], dim, dir, perms[dim, 0], perms[dim, 1], i1, data);
			}
			if (calculateSecondNormal)
			{
				data.verticesUVs[i2] = GetUVs(data.voxelVerticesAO[data.rectIndices[i2]], dim, dir, perms[dim, 0], perms[dim, 1], i2, data);
			}
			return data.verticesUVs[i1].Equals(data.verticesUVs[i2], 0, 0);
		}
		return false;
	}

	private Vector2 GetUVs(byte mask, int dim, int dir, int u, int v, int i, MeshData data)
	{
		if (dir == -1)
		{
			switch (i)
			{
			case 1:
				i = 3;
				break;
			case 3:
				i = 1;
				break;
			}
		}
		Vector2 result = default(Vector2);
		int[] currentVoxelMaskIndices = data.currentVoxelMaskIndices;
		currentVoxelMaskIndices[0] = 0;
		currentVoxelMaskIndices[1] = 0;
		currentVoxelMaskIndices[2] = 0;
		currentVoxelMaskIndices[dim] = ((dir != 1) ? 1 : 0);
		bool flag;
		bool flag2;
		if ((dir == 1 && (dim == 0 || dim == 1)) || (dir == -1 && dim == 2))
		{
			flag = (i == 2 || i == 3);
			flag2 = (i == 1 || i == 2);
		}
		else
		{
			flag = (i == 1 || i == 2);
			flag2 = (i == 2 || i == 3);
		}
		currentVoxelMaskIndices[u] = ((!flag) ? 1 : 0);
		currentVoxelMaskIndices[v] = ((!flag2) ? 1 : 0);
		int[] indices = data.indices;
		indices[0] = currentVoxelMaskIndices[0];
		indices[1] = currentVoxelMaskIndices[1];
		indices[2] = currentVoxelMaskIndices[2];
		indices[u] = 1 - indices[u];
		int num = maskHelper[indices[0]][indices[1]][indices[2]];
		if (((mask >> num) & 1) == 1)
		{
			result[0] = 0.5f;
		}
		else
		{
			result[0] = ((!flag) ? 0f : 1f);
		}
		indices[u] = currentVoxelMaskIndices[u];
		indices[v] = 1 - indices[v];
		num = maskHelper[indices[0]][indices[1]][indices[2]];
		if (((mask >> num) & 1) == 1)
		{
			result[1] = 0.5f;
		}
		else
		{
			result[1] = ((!flag2) ? 0f : 1f);
		}
		if ((dim == 2 && dir == -1) || (dim == 1 && dir == 1))
		{
			result[0] = 1f - result[0];
			result[1] = 1f - result[1];
		}
		else if ((dim == 2 && dir == 1) || (dim == 0 && dir == -1) || (dim == 1 && dir == -1))
		{
			result[0] = 1f - result[0];
		}
		return result;
	}

	private bool IsChunkUnderPlayer(Index chunkindex)
	{
		return chunkindex.y <= playerStartingChunkIndex.y && chunkindex.x == playerStartingChunkIndex.x && chunkindex.z == playerStartingChunkIndex.z;
	}

	private void ProcessMeshData(MeshData data)
	{
		if (data == null)
		{
			UnityEngine.Debug.LogError("greedy mesh creator data null");
			return;
		}
		data.chunk = null;
		ChunkManager.Chunks.TryGetValue(data.chunkData.ChunkIndex, out data.chunk);
		if (data.chunk != null)
		{
			int num = data.chunk.transform.childCount - 1;
			Transform transform = null;
			for (int num2 = num; num2 >= 0; num2--)
			{
				transform = data.chunk.transform.GetChild(num2);
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
		addedCustoms.Clear();
		foreach (KeyValuePair<int, ushort> customblock in data.customblocks)
		{
			int num3 = customblock.Key % data.SideLength;
			int num4 = (customblock.Key >> sidePower) % data.SideLength;
			int num5 = customblock.Key >> sideSquaredPower;
			Vector3 position = ChunkData.IndexToPosition(data.chunkData.ChunkIndex) + new Vector3(num3, num4, num5);
			Voxel voxelType = Engine.GetVoxelType(customblock.Value, position);
			byte voxelRotation = data.chunkData.GetVoxelRotation(num3, num4, num5);
			if (CreateCustomMesh(data, voxelType, num3, num4, num5, voxelRotation, isSafeToAdditionalMesh: false))
			{
				addedCustoms.Add(customblock.Key);
			}
		}
		if (data.Vertices.Count < 1 && data.SolidColliderVertices.Count < 1 && addedCustoms.Count == data.customblocks.Count)
		{
			if (data.chunk != null)
			{
				ChunkManager.chunkPool.Release(data.chunk);
			}
			data.Clear();
			return;
		}
		if (data.chunk == null)
		{
			data.chunk = ChunkManager.chunkPool.Take().Init(data.chunkData);
		}
		if (data.chunk == null)
		{
			data.Clear();
			return;
		}
		data.meshRenderer = data.chunk.GetComponent<MeshRenderer>();
		if (data.eventsData.Count > 0)
		{
			foreach (MeshData.MeshDataVoxelEvents eventsDatum in data.eventsData)
			{
				MeshData.MeshDataVoxelEvents current2 = eventsDatum;
				VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(current2.voxelID);
				if (instanceForVoxelId != null)
				{
					instanceForVoxelId.OnBlockRebuilded(data.chunkData, current2.x0, current2.x1, current2.x2);
				}
			}
		}
		foreach (KeyValuePair<int, ushort> customblock2 in data.customblocks)
		{
			if (!addedCustoms.Contains(customblock2.Key))
			{
				int num6 = customblock2.Key % data.SideLength;
				int num7 = (customblock2.Key >> sidePower) % data.SideLength;
				int num8 = customblock2.Key >> sideSquaredPower;
				Vector3 position2 = ChunkData.IndexToPosition(data.chunkData.ChunkIndex) + new Vector3(num6, num7, num8);
				Voxel voxelType2 = Engine.GetVoxelType(customblock2.Value, position2);
				byte voxelRotation2 = data.chunkData.GetVoxelRotation(num6, num7, num8);
				if (CreateCustomMesh(data, voxelType2, num6, num7, num8, voxelRotation2, isSafeToAdditionalMesh: true))
				{
					addedCustoms.Add(customblock2.Key);
				}
			}
		}
		if (data.Vertices.Count > 0 || data.SolidColliderVertices.Count > 0)
		{
			data.chunk.GetComponent<MeshRenderer>().enabled = true;
			data.chunk.GetComponent<MeshCollider>().enabled = true;
			object mesh = data.chunk.GetComponent<MeshFilter>().sharedMesh;
			if (mesh == null)
			{
				Mesh mesh2 = new Mesh();
				data.chunk.GetComponent<MeshFilter>().sharedMesh = mesh2;
				mesh = mesh2;
			}
			UpdateMesh(data, (Mesh)mesh, data.chunk.GetComponent<MeshCollider>());
			data.chunk.StartSpawnAnimation();
		}
		else if (data.chunk.transform.childCount > 0)
		{
			data.chunk.GetComponent<MeshRenderer>().enabled = false;
			data.chunk.GetComponent<MeshCollider>().enabled = false;
		}
		else
		{
			data.chunk.GetComponent<MeshRenderer>().enabled = false;
			data.chunk.GetComponent<MeshCollider>().enabled = false;
			ChunkManager.chunkPool.Release(data.chunk);
		}
		data.Clear();
	}

	private bool DrawThisVoxelsFace(ushort aVoxel, Transparency aTransp, bool aCustomMesh, ushort bVoxel, Transparency bTransp, bool bCustomMesh, bool fromUp)
	{
		if (aTransp == Transparency.transparent || bTransp == Transparency.solid || aCustomMesh)
		{
			return false;
		}
		bool flag = aTransp == bTransp && bTransp == Transparency.semiTransparent && aVoxel == bVoxel && (aVoxel != Engine.usefulIDs.blueprintID || !bCustomMesh);
		return (aTransp != Transparency.overlay || (bTransp == Transparency.transparent && fromUp) || (bVoxel == Engine.usefulIDs.iceBlockID && !fromUp)) && !flag;
	}

	private void CreateFace(MeshData data, ushort voxel, int[] x, float[] du, float[] dv, int u, int v, int facing, int dir, Vector3 pos, int dim)
	{
		Vector3 position = data.chunkPosition + pos;
		Voxel voxelType = Engine.GetVoxelType(voxel, position);
		List<int> list = data.Faces[voxelType.VSubmeshIndex];
		Vector3[] verticesFace = data.verticesFace;
		verticesFace[0] = new Vector3(x[0], x[1], x[2]);
		verticesFace[1] = new Vector3((float)x[0] + du[0], (float)x[1] + du[1], (float)x[2] + du[2]);
		verticesFace[2] = new Vector3((float)x[0] + du[0] + dv[0], (float)x[1] + du[1] + dv[1], (float)x[2] + du[2] + dv[2]);
		verticesFace[3] = new Vector3((float)x[0] + dv[0], (float)x[1] + dv[1], (float)x[2] + dv[2]);
		Color item = (voxel == 9 || voxel == 67 || voxel == 68) ? Color.black : Color.white;
		for (int i = 0; i < verticesFace.Length; i++)
		{
			if (ambientOcclusion)
			{
				int num = (int)(verticesFace[i].x * (float)verticesLengthPowers[2] + verticesFace[i].y * (float)verticesLengthPowers[1] + verticesFace[i].z);
				byte b = data.voxelVerticesAO[num];
				int num2 = 0;
				if (dir == 1)
				{
					switch (dim)
					{
					case 0:
						num2 = (b & 1) + ((b & 2) >> 1) + ((b & 4) >> 2) + ((b & 8) >> 3);
						break;
					case 1:
						num2 = (b & 1) + ((b & 2) >> 1) + ((b & 0x10) >> 4) + ((b & 0x20) >> 5);
						break;
					default:
						num2 = (b & 1) + ((b & 0x10) >> 4) + ((b & 4) >> 2) + ((b & 0x40) >> 6);
						break;
					}
				}
				else
				{
					switch (dim)
					{
					case 0:
						num2 = ((b & 0x10) >> 4) + ((b & 0x20) >> 5) + ((b & 0x40) >> 6) + ((b & 0x80) >> 7);
						break;
					case 1:
						num2 = ((b & 4) >> 2) + ((b & 8) >> 3) + ((b & 0x40) >> 6) + ((b & 0x80) >> 7);
						break;
					default:
						num2 = ((b & 2) >> 1) + ((b & 0x20) >> 5) + ((b & 8) >> 3) + ((b & 0x80) >> 7);
						break;
					}
				}
				float num3 = item.a = ambientOcclusionValues[num2];
				if (edgeNormals)
				{
					Vector2 vector = data.verticesUVs[i + 4];
					if (dir == -1)
					{
						switch (i)
						{
						case 1:
							vector = data.verticesUVs[7];
							break;
						case 3:
							vector = data.verticesUVs[5];
							break;
						}
					}
					item.g = vector.x;
					item.b = vector.y;
				}
				else
				{
					item.g = 0.5f;
					item.b = 0.5f;
				}
			}
			data.Colors.Add(item);
			verticesFace[i] -= Vector3.one * 0.5f;
			data.Vertices.Add(verticesFace[i]);
		}
		if (tangents)
		{
			Vector4 zero = Vector4.zero;
			zero.w = -1f;
			if (dim == 0)
			{
				zero.z = dir;
			}
			else
			{
				zero.x = -1f;
			}
			for (int j = 0; j < verticesFace.Length; j++)
			{
				data.Tangents.Add(zero);
			}
		}
		float textureUnit = Engine.TextureUnit;
		Vector2 item2 = Engine.GetTextureOffset(voxelType, facing) * textureUnit;
		data.UVs.Add(new Vector2(item2.x, item2.y));
		data.UVs.Add(new Vector2(item2.x + textureUnit * du[u], item2.y + textureUnit * du[v]));
		data.UVs.Add(new Vector2(item2.x + textureUnit * (du[u] + dv[u]), item2.y + textureUnit * (du[v] + dv[v])));
		data.UVs.Add(new Vector2(item2.x + textureUnit * dv[u], item2.y + textureUnit * dv[v]));
		data.UVs2.Add(item2);
		data.UVs2.Add(item2);
		data.UVs2.Add(item2);
		data.UVs2.Add(item2);
		list.Add(data.FaceCount);
		list.Add(data.FaceCount + 1);
		list.Add(data.FaceCount + 3);
		list.Add(data.FaceCount + 1);
		list.Add(data.FaceCount + 2);
		list.Add(data.FaceCount + 3);
		if (voxelType.VColliderType == ColliderType.cube)
		{
			data.SolidColliderVertices.Add(verticesFace[0]);
			data.SolidColliderVertices.Add(verticesFace[1]);
			data.SolidColliderVertices.Add(verticesFace[2]);
			data.SolidColliderVertices.Add(verticesFace[3]);
			data.SolidColliderFaces.Add(data.SolidFaceCount);
			data.SolidColliderFaces.Add(data.SolidFaceCount + 1);
			data.SolidColliderFaces.Add(data.SolidFaceCount + 3);
			data.SolidColliderFaces.Add(data.SolidFaceCount + 1);
			data.SolidColliderFaces.Add(data.SolidFaceCount + 2);
			data.SolidColliderFaces.Add(data.SolidFaceCount + 3);
			data.SolidFaceCount += 4;
		}
		data.FaceCount += 4;
		if (data.Vertices.Count > 65530)
		{
			CreateNewMeshObject(data);
		}
	}

	public Vector2[] GetTopUVs(float pad, float tUnit, Vector2 tOffset, int rotation)
	{
		tOffset *= tUnit;
		float[] array = new float[3]
		{
			1f,
			0f,
			0f
		};
		float[] array2 = new float[3]
		{
			0f,
			1f,
			0f
		};
		int num = 0;
		int num2 = 1;
		return new Vector2[8]
		{
			new Vector2(tOffset.x, tOffset.y),
			new Vector2(tOffset.x + tUnit * array[num], tOffset.y + tUnit * array[num2]),
			new Vector2(tOffset.x + tUnit * (array[num] + array2[num]), tOffset.y + tUnit * (array[num2] + array2[num2])),
			new Vector2(tOffset.x + tUnit * array2[num], tOffset.y + tUnit * array2[num2]),
			new Vector2(tOffset.x, tOffset.y),
			new Vector2(tOffset.x, tOffset.y),
			new Vector2(tOffset.x, tOffset.y),
			new Vector2(tOffset.x, tOffset.y)
		};
	}

	private bool CreateCustomMesh(MeshData data, Voxel voxelComponent, int x, int y, int z, byte rotation, bool isSafeToAdditionalMesh)
	{
		Mesh vMesh = voxelComponent.VMesh;
		if (vMesh == null)
		{
			return false;
		}
		if (data.Faces.Count <= voxelComponent.VSubmeshIndex)
		{
			UnityEngine.Debug.LogError("CUSTOM MESH ERROR: " + data.Faces.Count + " " + voxelComponent.VSubmeshIndex);
			return false;
		}
		CustomMeshData value;
		int num = (!customMeshDict.TryGetValue(voxelComponent.GetUniqueID(), out value)) ? vMesh.vertexCount : value.vertices.Length;
		List<int> list = data.Faces[voxelComponent.VSubmeshIndex];
		bool flag = num > maxVerticesInChunkCustomMesh || voxelComponent.GetUniqueID() == blueprintIndex;
		if (data.Vertices.Count + num > 65534)
		{
			CreateNewMeshObject(data);
		}
		if ((flag && !isSafeToAdditionalMesh) || (!flag && isSafeToAdditionalMesh))
		{
			return false;
		}
		Vector3 blockOffSet = new Vector3(x, y, z);
		MeshRotation meshRotation = RotationsUtility.RotateMeshRotation(rotation, voxelComponent.VRotation);
		if (!flag)
		{
			Vector3[] array = (value == null) ? vMesh.vertices : value.vertices;
			AddRotatedVertices(data.Vertices, array, meshRotation, blockOffSet);
			Color[] array2 = (value == null) ? vMesh.colors : value.colors;
			float textureUnit = Engine.TextureUnit;
			Vector2 item = Engine.GetTextureOffset(voxelComponent, 0) * textureUnit;
			Vector4 vector = new Vector4(-1f, 0f, 0f, -1f);
			Color white = Color.white;
			Vector2[] array3 = (value != null) ? null : vMesh.uv;
			bool flag2 = array2 != null && array2.Length > 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (tangents)
				{
					List<Vector4> list2 = data.Tangents;
					Vector4? vector2 = (value != null) ? new Vector4?(value.tangents[i]) : null;
					list2.Add((!vector2.HasValue) ? vector : vector2.Value);
				}
				data.Colors.Add((!flag2) ? white : array2[i]);
				if (value != null)
				{
					data.UVs.Add(value.uvs[i]);
					data.UVs2.Add(value.uvs2[i]);
				}
				else if (voxelComponent.moveUvsByVTexture)
				{
					data.UVs.Add(new Vector2(array3[i].x * textureUnit + item.x, array3[i].y * textureUnit + item.y));
					data.UVs2.Add(item);
				}
				else
				{
					data.UVs.Add(array3[i]);
					data.UVs2.Add(item);
				}
			}
			int[] array4 = (value == null) ? vMesh.triangles : value.triangles;
			int[] array5 = array4;
			foreach (int num2 in array5)
			{
				list.Add(data.FaceCount + num2);
			}
			data.FaceCount += num;
		}
		ColliderType vColliderType = voxelComponent.VColliderType;
		Mesh mesh = voxelComponent.VColliderMesh;
		switch (vColliderType)
		{
		case ColliderType.cube:
			AddCubeMesh(data, x, y, z, voxelComponent.VColliderHeight);
			if (flag)
			{
				AddAdditionalCustomMeshObject(data, x, y, z, vMesh, voxelComponent.VSubmeshIndex, meshRotation, withCollider: false);
			}
			break;
		case ColliderType.mesh:
		{
			if (mesh == null)
			{
				mesh = vMesh;
			}
			if (flag)
			{
				AddAdditionalCustomMeshObject(data, x, y, z, vMesh, voxelComponent.VSubmeshIndex, meshRotation, withCollider: true);
				break;
			}
			AddRotatedVertices(data.SolidColliderVertices, mesh.vertices, meshRotation, blockOffSet);
			int[] triangles = mesh.triangles;
			foreach (int num3 in triangles)
			{
				data.SolidColliderFaces.Add(data.SolidFaceCount + num3);
			}
			data.SolidFaceCount += mesh.vertexCount;
			break;
		}
		default:
			if (voxelComponent.GetUniqueID() != 0 && flag)
			{
				AddAdditionalCustomMeshObject(data, x, y, z, vMesh, voxelComponent.VSubmeshIndex, meshRotation, withCollider: false);
			}
			break;
		}
		return true;
	}

	private void AddAdditionalCustomMeshObject(MeshData data, int x, int y, int z, Mesh mesh, int submeshIndex, MeshRotation rotation, bool withCollider)
	{
		if (additionalCustomMeshPrefab == null)
		{
			additionalCustomMeshPrefab = Resources.Load<GameObject>("additionalCustomMesh");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(additionalCustomMeshPrefab, data.chunk.transform);
		gameObject.name = $"custom mesh {mesh.name}";
		gameObject.transform.position = data.chunk.transform.position + new Vector3(x, y, z);
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

	private void UpdateMesh(MeshData data, Mesh mesh, MeshCollider collider = null)
	{
		mesh.Clear();
		mesh.SetVertices(data.Vertices);
		mesh.SetColors(data.Colors);
		int num = 0;
		for (int i = 0; i < data.Faces.Count; i++)
		{
			if (data.Faces[i].Count > 0)
			{
				num++;
			}
		}
		if (data.StichedFaces.Count > 0)
		{
			num++;
		}
		mesh.subMeshCount = num;
		int num2 = 0;
		Material[] array = new Material[num];
		for (int j = 0; j < data.Faces.Count; j++)
		{
			if (data.Faces[j].Count > 0)
			{
				mesh.SetTriangles(data.Faces[j], num2);
				array[num2] = materialsCache[j];
				num2++;
			}
		}
		if (data.StichedFaces.Count > 0)
		{
			array[num - 1] = blackMaterial;
			mesh.SetTriangles(data.StichedFaces, num2);
		}
		data.meshRenderer.sharedMaterials = array;
		mesh.SetUVs(0, data.UVs);
		mesh.SetUVs(1, data.UVs2);
		mesh.RecalculateNormals();
		if (tangents)
		{
			mesh.SetTangents(data.Tangents);
		}
		if (data.SolidColliderFaces.Count > 0)
		{
			Mesh mesh2 = (!(collider.sharedMesh != null)) ? new Mesh() : collider.sharedMesh;
			mesh2.Clear();
			mesh2.SetVertices(data.SolidColliderVertices);
			mesh2.SetTriangles(data.SolidColliderFaces, 0);
			mesh2.RecalculateNormals();
			collider.sharedMesh = null;
			collider.sharedMesh = mesh2;
		}
		else
		{
			collider.sharedMesh = null;
		}
	}

	private void CreateNewMeshObject(MeshData data)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(chunkPrefab.GetComponent<Chunk>().MeshContainer, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = data.chunk.transform;
		MeshFilter component = gameObject.GetComponent<MeshFilter>();
		component.mesh = new Mesh();
		gameObject.GetComponent<Renderer>().sharedMaterials = materialsCache;
		UpdateMesh(data, component.mesh, gameObject.GetComponent<MeshCollider>());
	}

	private void AddCubeMesh(MeshData data, int x, int y, int z, float colliderHeight = 1f)
	{
		Vector3[] array = cubeVertices;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 a = array[i];
			if (a.y > 0f)
			{
				data.SolidColliderVertices.Add(a + new Vector3(x, (float)y - (1f - colliderHeight), z));
			}
			else
			{
				data.SolidColliderVertices.Add(a + new Vector3(x, y, z));
			}
		}
		int[] array2 = cubeTriangles;
		foreach (int num in array2)
		{
			data.SolidColliderFaces.Add(data.SolidFaceCount + num);
		}
		data.SolidFaceCount += cubeVertices.Length;
	}

	private void InitAOVariables()
	{
		verticesLengthPowers = new int[3];
		verticesLengthPowers[0] = 1;
		verticesLengthPowers[1] = ChunkData.SideLength + 1;
		verticesLengthPowers[2] = verticesLengthPowers[1] * verticesLengthPowers[1];
		if (!edgeNormals)
		{
			return;
		}
		maskHelper = new int[2][][];
		for (int i = 0; i < 2; i++)
		{
			maskHelper[i] = new int[2][];
			for (int j = 0; j < 2; j++)
			{
				maskHelper[i][j] = new int[2];
			}
		}
		maskHelper[0][0][0] = 7;
		maskHelper[0][0][1] = 6;
		maskHelper[0][1][0] = 5;
		maskHelper[0][1][1] = 4;
		maskHelper[1][0][0] = 3;
		maskHelper[1][0][1] = 2;
		maskHelper[1][1][0] = 1;
		maskHelper[1][1][1] = 0;
	}
}
