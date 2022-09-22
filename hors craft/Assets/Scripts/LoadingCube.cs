// DecompilerFi decompiler from Assembly-CSharp.dll class: LoadingCube
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class LoadingCube : MonoBehaviour
{
	public static LoadingCube instance;

	[SerializeField]
	private Voxel templateBlock;

	[SerializeField]
	public Texture2D tileset;

	[SerializeField]
	private GameObject customVisualizationPrefab;

	[SerializeField]
	private GameObject cubeToGenerate;

	private Mesh mesh;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void InitCube(Voxel loadingBlock)
	{
		if (customVisualizationPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(customVisualizationPrefab, base.transform.GetChild(0));
			gameObject.transform.localRotation = Quaternion.identity;
			cubeToGenerate.SetActive(value: false);
			return;
		}
		if (loadingBlock != null)
		{
			templateBlock = loadingBlock;
		}
		mesh = GetComponentInChildren<MeshFilter>().mesh;
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		Vector3[] vertices = new Vector3[24]
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
		int[] triangles = new int[36]
		{
			0,
			2,
			3,
			0,
			3,
			1,
			8,
			10,
			11,
			8,
			11,
			9,
			4,
			6,
			7,
			4,
			7,
			5,
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
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		SetTexture();
		meshRenderer.gameObject.SetActive(value: true);
	}

	private void SetTexture()
	{
		float num = 0.03125f;
		List<Vector2> list = new List<Vector2>();
		Vector2 textureOffset = Engine.GetTextureOffset(templateBlock, 5);
		list.Add(new Vector2(textureOffset.x * num, textureOffset.y * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, textureOffset.y * num));
		list.Add(new Vector2(textureOffset.x * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, (textureOffset.y + 1f) * num));
		textureOffset = Engine.GetTextureOffset(templateBlock, 4);
		list.Add(new Vector2((textureOffset.x + 1f) * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2(textureOffset.x * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, textureOffset.y * num));
		list.Add(new Vector2(textureOffset.x * num, textureOffset.y * num));
		textureOffset = Engine.GetTextureOffset(templateBlock, 0);
		list.Add(new Vector2((textureOffset.x + 1f) * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2(textureOffset.x * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, textureOffset.y * num));
		list.Add(new Vector2(textureOffset.x * num, textureOffset.y * num));
		textureOffset = Engine.GetTextureOffset(templateBlock, 1);
		list.Add(new Vector2((textureOffset.x + 1f) * num, textureOffset.y * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2(textureOffset.x * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2(textureOffset.x * num, textureOffset.y * num));
		textureOffset = Engine.GetTextureOffset(templateBlock, 3);
		list.Add(new Vector2(textureOffset.x * num, textureOffset.y * num));
		list.Add(new Vector2(textureOffset.x * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, textureOffset.y * num));
		textureOffset = Engine.GetTextureOffset(templateBlock, 2);
		list.Add(new Vector2(textureOffset.x * num, textureOffset.y * num));
		list.Add(new Vector2(textureOffset.x * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, (textureOffset.y + 1f) * num));
		list.Add(new Vector2((textureOffset.x + 1f) * num, textureOffset.y * num));
		mesh.uv = list.ToArray();
		meshRenderer.material = templateBlock.VMaterial;
		meshRenderer.material.SetTexture("_MainTex", tileset);
		if (meshRenderer.material.shader.name.Contains("CutOff + TwoSide"))
		{
			meshRenderer.material.shader = Shader.Find("_X/Diffuse + Two Side");
		}
		if (meshRenderer.material.shader.name.Contains("CutOff"))
		{
			meshRenderer.material.shader = Shader.Find("_X/Diffuse");
		}
	}
}
