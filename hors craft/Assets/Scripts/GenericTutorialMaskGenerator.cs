// DecompilerFi decompiler from Assembly-CSharp.dll class: GenericTutorialMaskGenerator
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

public static class GenericTutorialMaskGenerator
{
	private const float SCALING = 4f;

	private const float MIN_SIZE = 384f;

	private static float multiplier = 1f;

	private static Sprite darkPixelSprite;

	private static Sprite whitePixelSprite;

	private static Vector2 maskSize
	{
		get
		{
			Vector2 a = default(Vector2);
			a.x = Mathf.RoundToInt((float)Screen.width / 4f);
			a.y = Mathf.RoundToInt((float)Screen.height / 4f);
			float num = Mathf.Min(a.x, a.y);
			multiplier = 384f / num;
			multiplier = Mathf.Max(1f, multiplier);
			return a * multiplier;
		}
	}

	public static List<Pair<Sprite, Rect>> CreateMask(Rect targetRect, GenericTutorial.Shape shape, Color darkColor, Sprite customSprite = null)
	{
		InitPixelTextures(darkColor);
		Vector2 maskSize = GenericTutorialMaskGenerator.maskSize;
		int width = (int)maskSize.x;
		int height = (int)maskSize.y;
		targetRect.size /= 4f;
		targetRect.size *= multiplier;
		targetRect.position /= 4f;
		targetRect.position *= multiplier;
		List<Pair<Sprite, Rect>> list = new List<Pair<Sprite, Rect>>();
		switch (shape)
		{
		default:
			list.Add(CreateRectTexture(targetRect, width, height, darkColor));
			break;
		case GenericTutorial.Shape.CIRCLE:
			list.Add(CreateCircleTexture(targetRect, width, height, darkColor));
			break;
		case GenericTutorial.Shape.CUSTOM:
			list = CreateCustomTextures(targetRect, width, height, darkColor, customSprite);
			break;
		case GenericTutorial.Shape.WHOLE_SCREEN_DARK:
			list.Add(CreateOneColorTexture(width, height, darkColor));
			break;
		case GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT:
			list.Add(CreateOneColorTexture(width, height, new Color(0f, 0f, 0f, 0f)));
			break;
		}
		return list;
	}

	private static void InitPixelTextures(Color darkColor)
	{
		if (darkPixelSprite == null)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, darkColor);
			texture2D.Apply();
			darkPixelSprite = Sprite.Create(texture2D, new Rect(0f, 0f, 1f, 1f), new Vector2(0f, 0f), 100f);
		}
		if (whitePixelSprite == null)
		{
			Texture2D texture2D2 = new Texture2D(1, 1);
			texture2D2.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
			texture2D2.Apply();
			whitePixelSprite = Sprite.Create(texture2D2, new Rect(0f, 0f, 1f, 1f), new Vector2(0f, 0f), 100f);
		}
	}

	private static Pair<Sprite, Rect> CreateRectTexture(Rect targetRect, int width, int height, Color darkColor)
	{
		Texture2D texture2D = new Texture2D(width, height);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (!targetRect.Contains(new Vector2(i, j)))
				{
					texture2D.SetPixel(i, j, darkColor);
					continue;
				}
				float a = Mathf.Min(Mathf.Abs((float)i - targetRect.x), Mathf.Abs((float)i - targetRect.x - targetRect.width));
				float b = Mathf.Min(Mathf.Abs((float)j - targetRect.y), Mathf.Abs((float)j - targetRect.y - targetRect.height));
				float num = Mathf.Min(a, b);
				float num2 = 0.1f * Mathf.Min(targetRect.width, targetRect.height);
				float value = num / num2;
				Color color = darkColor;
				float num3 = color.a = Easing.Ease(EaseType.InQuart, color.a, 0f, value);
				texture2D.SetPixel(i, j, color);
			}
		}
		texture2D.Apply();
		return new Pair<Sprite, Rect>(Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f), new Rect(0f, 0f, texture2D.width, texture2D.height));
	}

	private static Pair<Sprite, Rect> CreateCircleTexture(Rect targetRect, int width, int height, Color darkColor)
	{
		Texture2D texture2D = new Texture2D(width, height);
		Vector2 center = targetRect.center;
		float num = Mathf.Max(targetRect.width, targetRect.height) / 2f;
		float num2 = num * num;
		float num3 = num * 40f;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				float num4 = Mathf.Pow((float)i - center.x, 2f);
				float num5 = Mathf.Pow((float)j - center.y, 2f);
				float num6 = num4 + num5 - num2;
				if (num6 < 0f)
				{
					float value = (0f - num6) / num3;
					Color color = darkColor;
					float num7 = color.a = Easing.Ease(EaseType.InQuart, color.a, 0f, value);
					texture2D.SetPixel(i, j, color);
				}
				else
				{
					texture2D.SetPixel(i, j, darkColor);
				}
			}
		}
		texture2D.Apply();
		return new Pair<Sprite, Rect>(Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f), new Rect(0f, 0f, texture2D.width, texture2D.height));
	}

	private static Texture2D CreateCustomTexture(Rect targetRect, int width, int height, Color darkColor, Sprite customSprite)
	{
		Texture2D texture2D = new Texture2D(width, height);
		Texture2D texture = customSprite.texture;
		Vector2 vector = new Vector2(texture.width, texture.height);
		float num = vector.y / vector.x;
		Vector2 center = targetRect.center;
		if (targetRect.width * num > targetRect.height)
		{
			targetRect.height = targetRect.width * num;
		}
		else
		{
			targetRect.width = targetRect.height / num;
		}
		targetRect.size *= 1.3f;
		targetRect.center = center;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (!targetRect.Contains(new Vector2(j, i)))
				{
					texture2D.SetPixel(j, i, darkColor);
					continue;
				}
				Vector2 vector2 = default(Vector2);
				vector2.x = ((float)j - targetRect.x) / targetRect.width;
				vector2.y = ((float)i - targetRect.y) / targetRect.height;
				Vector2 vector3 = vector2;
				Color pixelBilinear = texture.GetPixelBilinear(vector3.x, vector3.y);
				texture2D.SetPixel(j, i, pixelBilinear);
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	private static List<Pair<Sprite, Rect>> CreateCustomTextures(Rect targetRect, int width, int height, Color darkColor, Sprite customSprite)
	{
		Texture2D texture = customSprite.texture;
		Vector2 vector = new Vector2(texture.width, texture.height);
		float num = vector.y / vector.x;
		Vector2 center = targetRect.center;
		if (targetRect.width * num > targetRect.height)
		{
			targetRect.height = targetRect.width * num;
		}
		else
		{
			targetRect.width = targetRect.height / num;
		}
		targetRect.size *= 1.3f;
		targetRect.center = center;
		Vector2 sizeDelta = (Manager.Get<CanvasManager>().canvas.transform as RectTransform).sizeDelta;
		float num2 = sizeDelta.x / (float)width;
		float num3 = sizeDelta.y / (float)height;
		targetRect.width *= num2;
		targetRect.height *= num3;
		center.x *= num2;
		center.y *= num3;
		targetRect.center = center;
		List<Pair<Sprite, Rect>> list = new List<Pair<Sprite, Rect>>();
		list.Add(new Pair<Sprite, Rect>(customSprite, targetRect));
		list.Add(new Pair<Sprite, Rect>(darkPixelSprite, new Rect(0f, Mathf.Min(targetRect.yMin, 0f), sizeDelta.x, Mathf.Max(targetRect.yMin, 1f))));
		list.Add(new Pair<Sprite, Rect>(darkPixelSprite, new Rect(Mathf.Min(targetRect.xMin, 0f), targetRect.yMin, Mathf.Max(targetRect.xMin, 1f), targetRect.height)));
		list.Add(new Pair<Sprite, Rect>(darkPixelSprite, new Rect(targetRect.xMax, targetRect.yMin, Mathf.Max(sizeDelta.x - targetRect.xMax, 1f), targetRect.height)));
		list.Add(new Pair<Sprite, Rect>(darkPixelSprite, new Rect(0f, targetRect.yMax, sizeDelta.x, Mathf.Max(sizeDelta.y - targetRect.yMax, 1f))));
		return list;
	}

	private static Pair<Sprite, Rect> CreateOneColorTexture(int width, int height, Color color)
	{
		Sprite first;
		if (color == darkPixelSprite.texture.GetPixel(0, 0))
		{
			first = darkPixelSprite;
		}
		else if (color == whitePixelSprite.texture.GetPixel(0, 0))
		{
			first = whitePixelSprite;
		}
		else
		{
			Texture2D texture2D = new Texture2D(1, 1);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			first = Sprite.Create(texture2D, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
		}
		return new Pair<Sprite, Rect>(first, new Rect(0f, 0f, width, height));
	}
}
