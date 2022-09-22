// DecompilerFi decompiler from Assembly-CSharp.dll class: VoxelSprite
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class VoxelSprite
{
	private static Texture2D _tileset;

	private static Dictionary<ushort, Sprite> spriteCollection;

	private static Texture2D tileset
	{
		get
		{
			if (_tileset == null)
			{
				_tileset = Engine.GetTilesetTexture();
			}
			return _tileset;
		}
	}

	public static void ClearAllSprites()
	{
		_tileset = null;
		spriteCollection = null;
	}

	public static Vector2 GetVoxelUVsOffset(Voxel voxel, int arrayIndex)
	{
		if (tileset == null || voxel.VTexture.Length <= arrayIndex)
		{
			return Vector2.zero;
		}
		int num = tileset.height / 32;
		Vector2 vector = voxel.VTexture[arrayIndex];
		float y = Mathf.FloorToInt(vector.y);
		vector = new Vector2(Mathf.FloorToInt(vector.x), y);
		return new Vector2(Mathf.FloorToInt(vector.x) * num, Mathf.FloorToInt(vector.y) * num);
	}

	public static Sprite GetVoxelSprite(ushort voxelId)
	{
		return GetVoxelSprite(Engine.GetVoxelType(voxelId));
	}

	public static Sprite GetVoxelSprite(Voxel voxel)
	{
		if (spriteCollection == null)
		{
			spriteCollection = new Dictionary<ushort, Sprite>();
		}
		if (voxel == null)
		{
			return null;
		}
		if (voxel.useCustomSprite)
		{
			return voxel.voxelSprite;
		}
		if (voxel.GetComponent<VoxelDoorOpenClose>() != null)
		{
			return GenerateDoorSprite(voxel);
		}
		if (Application.isPlaying && Manager.Get<ModelManager>().blocksUnlocking.Is3dBlocksViewEnabled() && Engine.ChunkManagerInstance.tileset3d != null)
		{
			Sprite sprite = Engine.ChunkManagerInstance.tileset3d.GetComponent<Tileset3D>().GetSprite(voxel);
			if (sprite != null)
			{
				return sprite;
			}
			return GenerateNormalBlock(voxel);
		}
		return GenerateNormalBlock(voxel);
	}

	private static Sprite GenerateDoorSprite(Voxel voxel)
	{
		if (!spriteCollection.ContainsKey(voxel.GetUniqueID()))
		{
			if (tileset == null)
			{
				return null;
			}
			int num = tileset.height / 32;
			Rect rect = new Rect(GetVoxelUVsOffset(voxel, 0), Vector2.one * num);
			Rect rect2 = new Rect(GetVoxelUVsOffset(voxel, 1), Vector2.one * num);
			Texture2D texture2D = new Texture2D(num, num * 2);
			Color[] pixels = tileset.GetPixels((int)rect.x, (int)rect.y, num, num);
			Color[] pixels2 = tileset.GetPixels((int)rect2.x, (int)rect2.y, num, num);
			texture2D.SetPixels(0, num, num, num, pixels);
			texture2D.SetPixels(0, 0, num, num, pixels2);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, num, num * 2), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			spriteCollection.Add(voxel.GetUniqueID(), value);
		}
		return spriteCollection[voxel.GetUniqueID()];
	}

	private static Sprite GenerateNormalBlock(Voxel voxel)
	{
		if (!spriteCollection.ContainsKey(voxel.GetUniqueID()))
		{
			if (tileset == null)
			{
				return null;
			}
			if (voxel.VTexture == null)
			{
				return voxel.voxelSprite;
			}
			Rect rect = new Rect(GetVoxelUVsOffset(voxel, (voxel.VTexture.Length == 6) ? 5 : 0), Vector2.one * (tileset.height / 32));
			Sprite value = Sprite.Create(tileset, rect, new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			spriteCollection.Add(voxel.GetUniqueID(), value);
		}
		return spriteCollection[voxel.GetUniqueID()];
	}

	public static Texture2D EditorGetVoxelSprite(Voxel voxel, Texture2D texture)
	{
		int num = texture.height / 32;
		Vector2 vector = voxel.VTexture[(voxel.VTexture.Length == 6) ? 5 : 0];
		vector = new Vector2(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
		Vector2 position = new Vector2(Mathf.FloorToInt(vector.x) * num, Mathf.FloorToInt(vector.y) * num);
		Rect rect = new Rect(position, Vector2.one * num);
		if (!voxel.GetComponent<VoxelDoorOpenClose>())
		{
			Texture2D texture2D = new Texture2D(num, num);
			Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, num, num);
			texture2D.SetPixels(pixels);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			return texture2D;
		}
		Vector2 vector2 = voxel.VTexture[1];
		vector2 = new Vector2(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
		Vector2 position2 = new Vector2(Mathf.FloorToInt(vector2.x) * num, Mathf.FloorToInt(vector2.y) * num);
		Rect rect2 = new Rect(position2, Vector2.one * num);
		Texture2D texture2D2 = new Texture2D(num, num * 2);
		Color[] pixels2 = texture.GetPixels((int)rect.x, (int)rect.y, num, num);
		Color[] pixels3 = texture.GetPixels((int)rect2.x, (int)rect2.y, num, num);
		texture2D2.SetPixels(0, num, num, num, pixels2);
		texture2D2.SetPixels(0, 0, num, num, pixels3);
		texture2D2.filterMode = FilterMode.Point;
		texture2D2.Apply();
		return texture2D2;
	}
}
