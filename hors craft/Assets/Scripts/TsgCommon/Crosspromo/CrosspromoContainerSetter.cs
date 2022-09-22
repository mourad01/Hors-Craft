// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.CrosspromoContainerSetter
using UnityEngine;
using UnityEngine.UI;

namespace TsgCommon.Crosspromo
{
	public class CrosspromoContainerSetter : MonoBehaviour
	{
		public Image bubble;

		public Image bgImage;

		public CurveText curveText;

		public LetterSpacing letterSpacing;

		private void Start()
		{
			CrosspromoContainerSetup containerSetup = MonoBehaviourSingleton<CrosspromoController>.get.containerSetup;
			AssignMissingReferences();
			bgImage.gameObject.SetActive(value: false);
			Sprite sprite = (!(MonoBehaviourSingleton<CrosspromoController>.get.containerSprite != null)) ? MonoBehaviourSingleton<MultiCrosspromoController>.get.containerSprite : MonoBehaviourSingleton<CrosspromoController>.get.containerSprite;
			Sprite sprite2 = (!(MonoBehaviourSingleton<CrosspromoController>.get.backgroundSprite != null)) ? MonoBehaviourSingleton<MultiCrosspromoController>.get.backgroundSprite : MonoBehaviourSingleton<CrosspromoController>.get.backgroundSprite;
			bubble.sprite = sprite;
			float num = sprite.texture.width;
			float num2 = sprite.texture.height;
			float x = num / 1024f;
			float y = num2 / 1024f;
			bubble.rectTransform.localScale = new Vector3(x, y, 1f);
			if (sprite2 != null)
			{
				bgImage.gameObject.SetActive(value: true);
				bgImage.sprite = sprite2;
				bgImage.rectTransform.localScale = bubble.rectTransform.localScale;
			}
			RectTransform component = curveText.GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			Vector3 localPosition = component.localPosition;
			rectTransform.anchoredPosition = new Vector3(localPosition.x, containerSetup.textyPos, 0f);
			curveText.enabled = containerSetup.useCurve;
			curveText.eulerRadius = containerSetup.textEulerRadius;
			if (!curveText.enabled)
			{
				curveText.GetComponent<Text>().text = "SEE OTHER\nGAMES";
			}
			letterSpacing.spacing = containerSetup.letterSpacing;
			Quaternion localRotation = default(Quaternion);
			localRotation.eulerAngles = new Vector3(0f, 0f, containerSetup.animatorZrotation);
			base.transform.FindChildRecursively("Animator").GetComponent<RectTransform>().localRotation = localRotation;
		}

		private void AssignMissingReferences()
		{
			if (bubble == null)
			{
				bubble = base.transform.FindChildRecursively("Bubble").GetComponent<Image>();
			}
			if (bgImage == null)
			{
				bgImage = base.transform.FindChildRecursively("Background").GetComponent<Image>();
			}
			if (curveText == null)
			{
				curveText = GetComponentInChildren<CurveText>();
			}
			if (letterSpacing == null)
			{
				letterSpacing = GetComponentInChildren<LetterSpacing>();
			}
		}

		public void SetSprites()
		{
		}
	}
}
