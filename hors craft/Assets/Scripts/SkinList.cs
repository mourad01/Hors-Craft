// DecompilerFi decompiler from Assembly-CSharp.dll class: SkinList
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinList : MonoBehaviourSingleton<SkinList>
{
	public class SpriteP
	{
		public Sprite sprite;

		public Skin skin;

		public SpriteP(Sprite sprite, Skin skin)
		{
			this.sprite = sprite;
			this.skin = skin;
		}
	}

	[SerializeField]
	public List<Skin> possibleSkins;

	public Dictionary<Skin.Gender, float> genderProbabilities;

	public float sumOfWeights;

	public Dictionary<BodyPart, Dictionary<string, SpriteP>> clothesSprites;

	public Dictionary<int, Material> clothesMaterials = new Dictionary<int, Material>();

	private static SkinList _instance;

	public static SkinList customPlayerSkinList;

	public Dictionary<BodyPart, Dictionary<string, SpriteP>> shopSprites
	{
		get
		{
			if (clothesSprites == null || possibleSkins.Count != clothesSprites[BodyPart.Head].Count)
			{
				CreateSprites();
			}
			return clothesSprites;
		}
	}

	public static SkinList instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<SkinList>();
			}
			return _instance;
		}
	}

	public List<Sprite> GetNextToUnlockSprites()
	{
		List<Sprite> list = new List<Sprite>();
		List<Skin> list2 = possibleSkins.FindAll((Skin x) => 1 == CustomizationClothesTabFragment.AdsNeededForItem(x.id));
		foreach (Skin item in list2)
		{
			list.Add(shopSprites[BodyPart.Head][item.texture.name].sprite);
			list.Add(shopSprites[BodyPart.Body][item.texture.name].sprite);
			list.Add(shopSprites[BodyPart.Legs][item.texture.name].sprite);
		}
		return list;
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public override void Init()
	{
		CreateTemporaryDictionaries();
		RestoreIds();
	}

	public void RestoreIds()
	{
		int num = 0;
		foreach (Skin possibleSkin in possibleSkins)
		{
			possibleSkin.id = num;
			num++;
		}
	}

	private void CreateTemporaryDictionaries()
	{
		clothesMaterials = new Dictionary<int, Material>();
		CreateSprites();
	}

	public Sprite getSprite(BodyPart part, int index)
	{
		if (clothesSprites == null)
		{
			CreateSprites();
		}
		if (!clothesSprites[part].ContainsKey(possibleSkins[index].texture.name) || clothesSprites[part][possibleSkins[index].texture.name].sprite == null)
		{
			CreateSprite(possibleSkins[index]);
		}
		return clothesSprites[part][possibleSkins[index].texture.name].sprite;
	}

	private void CreateSprite(Skin skin)
	{
		clothesSprites[BodyPart.Head][skin.texture.name] = new SpriteP(Sprite.Create(skin.texture, new Rect(8f, 48f, 8f, 8f), Vector2.zero, 100f, 0u, SpriteMeshType.FullRect), skin);
		clothesSprites[BodyPart.Body][skin.texture.name] = new SpriteP(Sprite.Create(CreateBodyTexture(skin.texture), new Rect(0f, 0f, 16f, 12f), Vector2.zero, 100f, 0u, SpriteMeshType.FullRect), skin);
		clothesSprites[BodyPart.Legs][skin.texture.name] = new SpriteP(Sprite.Create(CreateLegsTexture(skin.texture), new Rect(0f, 0f, 8f, 13f), Vector2.zero, 100f, 0u, SpriteMeshType.FullRect), skin);
	}

	private void CreateSprites()
	{
		clothesSprites = new Dictionary<BodyPart, Dictionary<string, SpriteP>>();
		IEnumerator enumerator = Enum.GetValues(BodyPart.Body.GetType()).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BodyPart key = (BodyPart)enumerator.Current;
				clothesSprites[key] = new Dictionary<string, SpriteP>();
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		foreach (Skin possibleSkin in possibleSkins)
		{
			CreateSprite(possibleSkin);
		}
	}

	public void AddPossibleSkin(Skin skin)
	{
		possibleSkins.Add(new Skin(skin, possibleSkins.Count));
	}

	public void AddPossibleSkin(List<Skin> skins)
	{
		for (int i = 0; i < skins.Count; i++)
		{
			AddPossibleSkin(skins[i]);
		}
	}

	private Texture2D CreateLegsTexture(Texture2D mainTexture)
	{
		Texture2D texture2D = new Texture2D(8, 13, TextureFormat.ARGB32, mipChain: false);
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		Color[] pixels = mainTexture.GetPixels(4, 32, 4, 12);
		texture2D.SetPixels(0, 0, 4, 12, pixels);
		pixels = mainTexture.GetPixels(20, 0, 4, 12);
		texture2D.SetPixels(4, 0, 4, 12, pixels);
		pixels = mainTexture.GetPixels(20, 32, 8, 1);
		texture2D.SetPixels(0, 12, 8, 1, pixels);
		for (int i = 0; i < 10; i++)
		{
			texture2D.SetPixel(3, i, Color.clear);
			texture2D.SetPixel(4, i, Color.clear);
		}
		texture2D.Apply();
		return texture2D;
	}

	private Texture2D CreateBodyTexture(Texture2D mainTexture)
	{
		Texture2D texture2D = new Texture2D(16, 12, TextureFormat.ARGB32, mipChain: false);
		texture2D.filterMode = FilterMode.Point;
		Color[] pixels = mainTexture.GetPixels(20, 32, 8, 12);
		texture2D.SetPixels(4, 0, 8, 12, pixels);
		pixels = mainTexture.GetPixels(44, 32, 4, 12);
		texture2D.SetPixels(0, 0, 4, 12, pixels);
		pixels = mainTexture.GetPixels(36, 0, 4, 12);
		texture2D.SetPixels(12, 0, 4, 12, pixels);
		texture2D.SetPixel(0, 11, Color.clear);
		texture2D.SetPixel(1, 11, Color.clear);
		texture2D.SetPixel(0, 10, Color.clear);
		texture2D.SetPixel(15, 11, Color.clear);
		texture2D.SetPixel(14, 11, Color.clear);
		texture2D.SetPixel(15, 10, Color.clear);
		for (int i = 0; i < 6; i++)
		{
			texture2D.SetPixel(3, i, Color.clear);
			texture2D.SetPixel(12, i, Color.clear);
		}
		for (int j = 0; j < 4; j++)
		{
			texture2D.SetPixel(j + 6, 11, Color.clear);
		}
		texture2D.Apply();
		return texture2D;
	}

	private void CreateNewMaterial(int index, Texture texture)
	{
		clothesMaterials[index] = new Material(Shader.Find("Diffuse"));
		clothesMaterials[index].SetTexture("_MainTex", texture);
	}

	public Material GetClothes(int index)
	{
		index %= possibleSkins.Count;
		if (!clothesMaterials.ContainsKey(index))
		{
			CreateNewMaterial(index, possibleSkins[index].texture);
		}
		return clothesMaterials[index];
	}

	public void SetGenderProbabilities(Dictionary<Skin.Gender, float> genderProbabilities)
	{
		this.genderProbabilities = genderProbabilities;
		sumOfWeights = 0f;
		foreach (Skin possibleSkin in possibleSkins)
		{
			sumOfWeights += genderProbabilities[possibleSkin.gender];
		}
	}
}
