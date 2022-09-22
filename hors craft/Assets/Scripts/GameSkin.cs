// DecompilerFi decompiler from Assembly-CSharp.dll class: GameSkin
using System;
using Uniblocks;
using UnityEngine;

[Serializable]
public class GameSkin
{
	public int levelId;

	public Texture2D tileSet;

	public Skin skin;

	public void InjectTileSetIntoEngine()
	{
		Engine.SetTileset(tileSet);
	}
}
