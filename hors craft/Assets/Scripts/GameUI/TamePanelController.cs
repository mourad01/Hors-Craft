// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.TamePanelController
using com.ootii.Cameras;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class TamePanelController : MonoBehaviour
	{
		public Image tameProgress;

		public Image heart;

		public Sprite loveHeart;

		public Sprite hateHeart;

		private TameContext tameContext;

		private void Awake()
		{
			tameContext = new TameContext();
		}

		public void Enable(bool enable)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_FRONT_OF_TAMEABLE);
			MonoBehaviourSingleton<GameplayFacts>.get.SetContext(Fact.IN_FRONT_OF_TAMEABLE, tameContext, enable);
		}

		private void OnDisable()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_FRONT_OF_TAMEABLE);
		}

		public void EnableOnlyTameButton(bool enable)
		{
			tameContext.tameButtonAllowed = enable;
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.IN_FRONT_OF_TAMEABLE);
		}

		public void ShowAndPosition(Transform overWhat, Pettable pettable)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			base.transform.position = CameraController.instance.MainCamera.WorldToScreenPoint(overWhat.TransformPoint(pettable.relativeTamePanelPos));
			tameContext.pettable = pettable;
			tameContext.tameButtonAllowed = true;
			Enable(enable: true);
		}

		public void Refresh(Pettable pettable)
		{
			Refresh(pettable.tameValue, pettable.tameMaxValue, pettable.tamed);
		}

		public void Refresh(float value, float maxValue, bool tamed = false)
		{
			tameProgress.fillAmount = value / maxValue;
			heart.sprite = ((!tamed) ? hateHeart : loveHeart);
		}

		public void SetCustomSprite(Sprite sprite)
		{
			heart.sprite = sprite;
		}

		public void SetBarColor(Color color)
		{
			ColorController component = tameProgress.gameObject.GetComponent<ColorController>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			tameProgress.color = color;
		}
	}
}
