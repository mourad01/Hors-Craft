// DecompilerFi decompiler from Assembly-CSharp.dll class: PhotoRewardText
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotoRewardText : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(ClainAnimationCO());
	}

	private IEnumerator ClainAnimationCO()
	{
		Text text = GetComponent<Text>();
		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.localScale = Vector3.zero;
		Color color4 = text.color;
		float newAlpha = 0f;
		float animTime = 2f;
		float startTime = Time.realtimeSinceStartup;
		float duration = 0f;
		bool fadeOut = false;
		while (duration < animTime)
		{
			float delta = Time.realtimeSinceStartup - startTime;
			startTime = Time.realtimeSinceStartup;
			if (newAlpha >= 1f && !fadeOut)
			{
				fadeOut = true;
			}
			float newDelta = (!fadeOut) ? (delta * 2f / animTime) : ((0f - delta) * 2f / animTime);
			newAlpha += newDelta;
			Text text2 = text;
			Color color = text.color;
			float r = color.r;
			Color color2 = text.color;
			float g = color2.g;
			Color color3 = text.color;
			text2.color = new Color(r, g, color3.b, newAlpha);
			rectTransform.localScale += Vector3.one * (delta / animTime);
			duration += delta;
			yield return null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
