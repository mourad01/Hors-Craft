// DecompilerFi decompiler from Assembly-CSharp.dll class: UberConfig
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UberConfigs/SingleConfig")]
public class UberConfig : ScriptableObject
{
	public int voxelID;

	public Material uberMaterial;

	public Shader uberShader;

	public Texture2D[] uberTexture;

	public Texture2D uberNormal;

	public Texture2D edgeTexture;

	public float blockSize;

	public Sprite customSprite;

	public Color tint = Color.white;

	internal bool dirtyMaterial;
}
