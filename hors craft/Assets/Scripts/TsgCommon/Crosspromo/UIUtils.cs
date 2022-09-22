// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.UIUtils
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	internal static class UIUtils
	{
		public static Vector2 GetPercentPosOnScreen(RectTransform rectTransform)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Vector3 vector = Vector3.zero;
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					vector += array[i];
				}
				vector /= array.Length;
			}
			vector.x /= Screen.width;
			vector.y /= Screen.height;
			return vector;
		}

		public static void FitOnScreen(RectTransform rectTransform, Vector2 percentPosOnScreen, int maxFitTries = 100, float alignToFitPercentStep = 0.01f)
		{
			float[] borders = new float[4]
			{
				0f,
				0f,
				Screen.width,
				Screen.height
			};
			moveTillItFits(rectTransform, borders, percentPosOnScreen, maxFitTries, alignToFitPercentStep);
		}

		public static void FitOnScreen(RectTransform rectTransform, RectTransform screenTransform, Vector2 percentPosOnScreen, int maxFitTries = 100, float alignToFitPercentStep = 0.01f)
		{
			Vector3[] array = new Vector3[4];
			screenTransform.GetWorldCorners(array);
			float[] borders = new float[4]
			{
				array[0].x,
				array[0].y,
				array[2].x,
				array[2].y
			};
			moveTillItFits(rectTransform, borders, percentPosOnScreen, maxFitTries, alignToFitPercentStep);
		}

		private static void moveTillItFits(RectTransform rectTransform, float[] borders, Vector2 percentPosOnScreen, int maxFitTries = 100, float alignToFitPercentStep = 0.01f)
		{
			bool flag = false;
			int num = 0;
			do
			{
				Vector2 vector3 = rectTransform.anchorMin = (rectTransform.anchorMax = percentPosOnScreen);
				rectTransform.anchoredPosition = Vector3.zero;
				Vector3[] array = new Vector3[4];
				rectTransform.GetWorldCorners(array);
				flag = true;
				if (array[0].x < borders[0])
				{
					percentPosOnScreen.x += alignToFitPercentStep;
					flag = false;
				}
				else if (array[0].y < borders[1])
				{
					percentPosOnScreen.y += alignToFitPercentStep;
					flag = false;
				}
				else if (array[2].x >= borders[2])
				{
					percentPosOnScreen.x -= alignToFitPercentStep;
					flag = false;
				}
				else if (array[2].y >= borders[3])
				{
					percentPosOnScreen.y -= alignToFitPercentStep;
					flag = false;
				}
				num++;
			}
			while (!flag && num < maxFitTries);
		}
	}
}
