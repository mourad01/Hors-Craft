// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.MeshExtraData
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
	internal class MeshExtraData
	{
		private List<Vector2> UVs = new List<Vector2>();

		private bool hasUVs;

		private List<Vector2> UV2s = new List<Vector2>();

		private bool hasUV2s;

		private List<Vector2> UV3s = new List<Vector2>();

		private bool hasUV3s;

		private List<Vector2> UV4s = new List<Vector2>();

		private bool hasUV4s;

		private List<Vector3> normals = new List<Vector3>();

		private bool hasNormals;

		private List<Vector4> tangents = new List<Vector4>();

		private bool hasTangents;

		private Vector2[] oldUVs;

		private Vector2[] oldUV2s;

		private Vector2[] oldUV3s;

		private Vector2[] oldUV4s;

		private Vector3[] oldNormals;

		private Vector4[] oldTangents;

		public void SetProcessingMesh(Mesh mesh)
		{
			oldUVs = mesh.uv;
			oldUV2s = mesh.uv2;
			oldUV3s = mesh.uv3;
			oldUV4s = mesh.uv4;
			oldNormals = mesh.normals;
			oldTangents = mesh.tangents;
		}

		public void AddItemAt(int index)
		{
			AddToList(UVs, oldUVs, index, ref hasUVs);
			AddToList(UV2s, oldUV2s, index, ref hasUV2s);
			AddToList(UV3s, oldUV3s, index, ref hasUV3s);
			AddToList(UV4s, oldUV4s, index, ref hasUV4s);
			AddToList(normals, oldNormals, index, ref hasNormals);
			AddToList(tangents, oldTangents, index, ref hasTangents);
		}

		public void ApplyData(Mesh mesh)
		{
			if (hasUVs)
			{
				mesh.uv = UVs.ToArray();
			}
			if (hasUV2s)
			{
				mesh.uv2 = UV2s.ToArray();
			}
			if (hasUV3s)
			{
				mesh.uv3 = UV3s.ToArray();
			}
			if (hasUV4s)
			{
				mesh.uv4 = UV4s.ToArray();
			}
			if (hasTangents)
			{
				mesh.tangents = tangents.ToArray();
			}
			if (hasNormals)
			{
				mesh.normals = normals.ToArray();
				mesh.RecalculateNormals();
			}
			else
			{
				mesh.RecalculateNormals();
			}
		}

		private static void AddToList<T>(List<T> list, T[] array, int index, ref bool addedFlag) where T : new()
		{
			if (array.Length <= index)
			{
				list.Add(new T());
				return;
			}
			list.Add(array[index]);
			addedFlag = true;
		}
	}
}
