// DecompilerFi decompiler from Assembly-CSharp.dll class: MeshData
using System;
using System.Collections.Generic;
using System.Threading;
using Uniblocks;
using UnityEngine;

[Serializable]
public class MeshData
{
	public struct MeshDataVoxelEvents
	{
		public ushort voxelID;

		public int x0;

		public int x1;

		public int x2;
	}

	public Chunk chunk;

	public ChunkData chunkData;

	public Vector3 chunkPosition;

	public int SideLength;

	public int SquaredSideLength;

	public int[] mask;

	public int[] maskIndices;

	public int materialsLength;

	public List<Vector3> Vertices;

	public List<Color> Colors;

	public List<List<int>> Faces;

	public List<Vector2> UVs;

	public List<Vector2> UVs2;

	public List<int> StichedFaces;

	public int FaceCount;

	public List<Vector3> SolidColliderVertices;

	public List<int> SolidColliderFaces;

	public int SolidFaceCount;

	public List<Vector3> NoCollideVertices;

	public List<int> NoCollideFaces;

	public int NoCollideFaceCount;

	public bool triggerVoxelEvents;

	public Dictionary<int, ushort> customblocks = new Dictionary<int, ushort>();

	public HashSet<int> visited = new HashSet<int>();

	public MeshRenderer meshRenderer;

	public volatile bool isFree = true;

	public int[] x;

	public int[] q;

	public int[] y;

	public List<MeshDataVoxelEvents> eventsData = new List<MeshDataVoxelEvents>();

	public byte[] voxelVerticesAO;

	public Vector2[] verticesUVs;

	public List<Vector4> Tangents;

	public int[] rectIndices;

	public bool hasChildMeshes;

	public int[] currentVoxelMaskIndices;

	public int[] indices;

	public Vector3[] verticesFace;

	private bool ambient;

	private bool normals;

	private bool tangents;

	public void Init(int materialsLength, bool ambientOcclusion = false, bool edgeNormals = false, bool tangents = false)
	{
		ambient = ambientOcclusion;
		normals = edgeNormals;
		this.tangents = tangents;
		SideLength = ChunkData.SideLength;
		SquaredSideLength = ChunkData.SquaredSideLength;
		Vertices = new List<Vector3>(8192);
		Colors = new List<Color>(8192);
		SolidColliderVertices = new List<Vector3>(8192);
		SolidColliderFaces = new List<int>(16384);
		UVs = new List<Vector2>(8192);
		UVs2 = new List<Vector2>(8192);
		StichedFaces = new List<int>(1200);
		mask = new int[SquaredSideLength];
		maskIndices = new int[SquaredSideLength];
		x = new int[3];
		q = new int[3];
		y = new int[3];
		Faces = new List<List<int>>(materialsLength);
		for (int i = 0; i < materialsLength; i++)
		{
			Faces.Add(new List<int>(32768));
		}
		if (mask == null || mask.Length != SquaredSideLength)
		{
			mask = new int[SquaredSideLength];
			maskIndices = new int[SquaredSideLength];
		}
		if (ambientOcclusion)
		{
			voxelVerticesAO = new byte[(int)Mathf.Pow(SideLength + 2, 3f)];
			if (edgeNormals)
			{
				verticesUVs = new Vector2[8];
				rectIndices = new int[4];
			}
		}
		if (tangents)
		{
			Tangents = new List<Vector4>(8192);
		}
		currentVoxelMaskIndices = new int[3];
		indices = new int[3];
		verticesFace = new Vector3[4];
		hasChildMeshes = false;
	}

	public void PrepareData(ChunkData chunk, bool triggerVoxelEvents)
	{
		chunkData = chunk;
		chunkPosition = ChunkData.IndexToPosition(chunk.ChunkIndex);
		SideLength = ChunkData.SideLength;
		SquaredSideLength = SideLength * SideLength;
		this.triggerVoxelEvents = triggerVoxelEvents;
		customblocks.Clear();
		visited.Clear();
		hasChildMeshes = false;
	}

	public void Clear()
	{
		ThreadPool.QueueUserWorkItem(ThreadClear, null);
	}

	private void ThreadClear(object info)
	{
		chunk = null;
		meshRenderer = null;
		eventsData.Clear();
		visited.Clear();
		customblocks.Clear();
		Vertices.Clear();
		Colors.Clear();
		SolidColliderVertices.Clear();
		UVs.Clear();
		UVs2.Clear();
		for (int i = 0; i < Faces.Count; i++)
		{
			Faces[i].Clear();
		}
		StichedFaces.Clear();
		SolidColliderFaces.Clear();
		FaceCount = 0;
		SolidFaceCount = 0;
		if (ambient)
		{
			voxelVerticesAO = new byte[(int)Mathf.Pow(SideLength + 2, 3f)];
			if (normals)
			{
				verticesUVs = new Vector2[8];
				rectIndices = new int[4];
			}
		}
		if (tangents)
		{
			Tangents.Clear();
		}
		hasChildMeshes = false;
		currentVoxelMaskIndices[0] = 0;
		currentVoxelMaskIndices[1] = 0;
		currentVoxelMaskIndices[2] = 0;
		indices[0] = 0;
		indices[1] = 0;
		indices[2] = 0;
		isFree = true;
	}
}
