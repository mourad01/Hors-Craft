// DecompilerFi decompiler from Assembly-CSharp.dll class: SpriteFromResources
using UnityEngine;
using UnityEngine.UI;

public class SpriteFromResources : MonoBehaviour
{
	public string pathToResource;

	public Image image;

	public Color color = Color.white;

	private Sprite sprite;

	private void Awake()
	{
		sprite = Resources.Load<Sprite>(pathToResource);
		image.sprite = sprite;
		image.color = color;
	}

	private void OnDestroy()
	{
		image.sprite = null;
		Resources.UnloadAsset(sprite);
		Resources.UnloadUnusedAssets();
	}
}
