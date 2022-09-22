// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.MeshCombiner
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
	public class MeshCombiner
	{
		public static Mesh CustomCombine(CustomCombineInstance[] combine, bool useOrginTransform, bool generateUV2 = false)
		{
			PrepareCombine(combine, useOrginTransform);
			Material[] array = FindMaterials(combine);
			List<Mesh> list = new List<Mesh>();
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				int[] materialIndex;
				CustomCombineInstance[] array3 = FindCombines(combine, material, out materialIndex);
				if (array3 != null)
				{
					Mesh item = CreateSubmesh(array3, materialIndex);
					list.Add(item);
				}
			}
			CombineInstance[] array4 = new CombineInstance[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				CombineInstance combineInstance = default(CombineInstance);
				combineInstance.mesh = list[j];
				combineInstance.subMeshIndex = 0;
				combineInstance.transform = Matrix4x4.identity;
				CombineInstance combineInstance2 = combineInstance;
				array4[j] = combineInstance2;
			}
			Mesh mesh = UnityCombine(array4, mergeSubMeshes: false);
			mesh.RecalculateNormals();
			if (!generateUV2 || mesh.vertexCount > 0)
			{
			}
			return mesh;
		}

		private static void PrepareCombine(CustomCombineInstance[] combine, bool useOrginTransform)
		{
			for (int i = 0; i < combine.Length; i++)
			{
				if (useOrginTransform)
				{
					combine[i].combineInstance.transform = combine[i].orginTransform.localToWorldMatrix;
					continue;
				}
				Vector3 position = combine[i].orginTransform.root.position;
				combine[i].orginTransform.root.position = Vector3.zero;
				Vector3 eulerAngles = combine[i].orginTransform.root.eulerAngles;
				combine[i].orginTransform.root.eulerAngles = Vector3.zero;
				combine[i].combineInstance.transform = combine[i].orginTransform.localToWorldMatrix;
				combine[i].orginTransform.root.position = position;
				combine[i].orginTransform.root.eulerAngles = eulerAngles;
			}
		}

		private static Mesh CreateSubmesh(CustomCombineInstance[] combines, int[] submeshIndex)
		{
			MeshExtraData meshExtraData = new MeshExtraData();
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < combines.Length; i++)
			{
				int[] triangles = combines[i].combineInstance.mesh.GetTriangles(submeshIndex[i]);
				Vector3[] vertices = combines[i].combineInstance.mesh.vertices;
				List<int> list3 = new List<int>();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				meshExtraData.SetProcessingMesh(combines[i].combineInstance.mesh);
				for (int j = 0; j < triangles.Length; j++)
				{
					if (!list3.Contains(triangles[j]))
					{
						list3.Add(triangles[j]);
						dictionary.Add(triangles[j], list.Count);
						Vector3 point = vertices[triangles[j]];
						Vector3 item = combines[i].combineInstance.transform.MultiplyPoint(point);
						list.Add(item);
						meshExtraData.AddItemAt(triangles[j]);
					}
				}
				for (int k = 0; k < triangles.Length; k++)
				{
					list2.Add(dictionary[triangles[k]]);
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = list.ToArray();
			mesh.triangles = list2.ToArray();
			meshExtraData.ApplyData(mesh);
			return mesh;
		}

		public static Material[] FindMaterials(CustomCombineInstance[] combine)
		{
			List<Material> list = new List<Material>();
			for (int i = 0; i < combine.Length; i++)
			{
				for (int j = 0; j < combine[i].meshRenderer.sharedMaterials.Length; j++)
				{
					if (!list.Contains(combine[i].meshRenderer.sharedMaterials[j]))
					{
						list.Add(combine[i].meshRenderer.sharedMaterials[j]);
					}
				}
			}
			return list.ToArray();
		}

		private static CustomCombineInstance[] FindCombines(CustomCombineInstance[] combine, Material material, out int[] materialIndex)
		{
			List<CustomCombineInstance> list = new List<CustomCombineInstance>(combine.Length);
			List<int> list2 = new List<int>(combine.Length);
			for (int i = 0; i < combine.Length; i++)
			{
				for (int j = 0; j < combine[i].meshRenderer.sharedMaterials.Length; j++)
				{
					if (!(combine[i].meshRenderer.sharedMaterials[j] != material))
					{
						list.Add(combine[i]);
						list2.Add(j);
					}
				}
			}
			materialIndex = list2.ToArray();
			return list.ToArray();
		}

		public static Mesh UnityCombine(CombineInstance[] combine, bool mergeSubMeshes = true, bool useMatrices = true)
		{
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(combine, mergeSubMeshes, useMatrices);
			return mesh;
		}
	}
}
