// DecompilerFi decompiler from Assembly-CSharp.dll class: RectSelectionUtil
using UnityEngine;

public static class RectSelectionUtil
{
	private static Texture2D _whiteTexture;

	public static Texture2D WhiteTexture
	{
		get
		{
			if (_whiteTexture != null)
			{
				return _whiteTexture;
			}
			_whiteTexture = new Texture2D(1, 1);
			_whiteTexture.SetPixel(0, 0, Color.white);
			_whiteTexture.Apply();
			return _whiteTexture;
		}
	}

	public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
	{
		screenPosition1.y = (float)Screen.height - screenPosition1.y;
		screenPosition2.y = (float)Screen.height - screenPosition2.y;
		Vector3 vector = Vector3.Min(screenPosition1, screenPosition2);
		Vector3 vector2 = Vector3.Max(screenPosition1, screenPosition2);
		return Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
	}

	public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
	{
		Vector3 lhs = camera.ScreenToViewportPoint(screenPosition1);
		Vector3 rhs = camera.ScreenToViewportPoint(screenPosition2);
		Vector3 min = Vector3.Min(lhs, rhs);
		Vector3 max = Vector3.Max(lhs, rhs);
		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;
		Bounds result = default(Bounds);
		result.SetMinMax(min, max);
		return result;
	}

	public static void DrawScreenRect(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.DrawTexture(rect, WhiteTexture);
		GUI.color = Color.white;
	}

	public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
	{
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
		DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
		DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
	}
}
