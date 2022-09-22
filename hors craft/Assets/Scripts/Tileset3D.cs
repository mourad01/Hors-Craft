// DecompilerFi decompiler from Assembly-CSharp.dll class: Tileset3D
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class Tileset3D : MonoBehaviour
{
	public Texture2D tileset;

	public List<Sprite> sprites3dList = new List<Sprite>();

	public Dictionary<string, Sprite> sprites3dDictionary;

	public void Init()
	{
		sprites3dDictionary = new Dictionary<string, Sprite>();
		foreach (Sprite sprites3d in sprites3dList)
		{
			sprites3dDictionary.Add(sprites3d.name, sprites3d);
		}
	}

	public Sprite GetSprite(Voxel voxel)
	{
		if (sprites3dDictionary == null)
		{
			Init();
		}
		sprites3dDictionary.TryGetValue(voxel.name, out Sprite value);
		return value;
	}
}
