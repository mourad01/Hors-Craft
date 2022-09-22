// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.ImageFileLoader
using System.IO;
using UnityEngine;

namespace Common.Utils
{
	public class ImageFileLoader
	{
		public static Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight)
		{
			Texture2D texture2D = LoadTexture(filePath);
			if (texture2D == null)
			{
				return null;
			}
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), pixelsPerUnit, 0u, spriteType);
		}

		public static Texture2D LoadTexture(string filePath)
		{
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				Texture2D texture2D = new Texture2D(2, 2);
				if (texture2D.LoadImage(data))
				{
					return texture2D;
				}
			}
			return null;
		}
	}
}
