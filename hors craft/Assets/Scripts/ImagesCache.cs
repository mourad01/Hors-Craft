// DecompilerFi decompiler from Assembly-CSharp.dll class: ImagesCache
using System.Collections.Generic;
using UnityEngine;

public class ImagesCache
{
	private Dictionary<string, Sprite> _cache;

	private Dictionary<string, Sprite> cache
	{
		get
		{
			if (_cache == null)
			{
				_cache = new Dictionary<string, Sprite>();
			}
			return _cache;
		}
	}

	public bool IsThereImage(string id)
	{
		return cache.ContainsKey(id);
	}

	public void AddImage(string id, Texture texture)
	{
		cache[id] = Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
	}

	public void AddImage(string id, Sprite texture)
	{
		cache[id] = texture;
	}

	public void RemoveImage(string id)
	{
		cache.Remove(id);
	}

	public Sprite GetImage(string id)
	{
		return cache[id];
	}

	public bool TryToGetImage(string id, out Sprite texture)
	{
		return cache.TryGetValue(id, out texture);
	}
}
