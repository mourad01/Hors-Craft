// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.CrosspromoIconDragger
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TsgCommon.Crosspromo
{
	public class CrosspromoIconDragger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public CrosspromoClickCallback onClick;

		[SerializeField]
		private RuntimeAnimatorController[] possibleAnimators;

		private RectTransform rectTransform;

		private Animator animator;

		private bool pointerDown;

		private bool treatLikeAClick;

		private int fingerId;

		private float downTime;

		private float upTime;

		private Vector2 basePercentPos;

		private Vector2 destPercentPos;

		private const float MOVE_AFTER_DURATION = 0.25f;

		private const float SMOOTH_MOVE_FACTOR = 0.15f;

		private const float SMOOTH_RETURN_FACTOR = 0.2f;

		private const float MOVE_UPDATE_INTERVALS = 0.01f;

		private const float RETURN_AFTER_DURATION = 1f;

		private const float CLICK_CALLBACK_DELAY = 0.5f;

		private float lastUpdateTime;

		private void Start()
		{
			rectTransform = (base.transform as RectTransform);
			basePercentPos = rectTransform.anchorMin;
			animator = GetComponentInChildren<Animator>(includeInactive: true);
			pointerDown = false;
			treatLikeAClick = false;
			fingerId = -1;
		}

		public void SetAnimator(int animatorIndex)
		{
			if (animatorIndex >= 0 && possibleAnimators.Length > 0)
			{
				GetComponentInChildren<Animator>().runtimeAnimatorController = possibleAnimators[Mathf.Clamp(animatorIndex, 0, possibleAnimators.Length - 1)];
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Vector2 localPoint = GetFingerPercentPos(eventData.pointerId);
			localPoint.Scale(new Vector2(Screen.width, Screen.height));
			RectTransform component = base.transform.GetChild(0).GetComponent<RectTransform>();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(component, eventData.position, eventData.pressEventCamera, out localPoint);
			float num = Mathf.Abs(localPoint.x);
			Vector2 offsetMax = component.offsetMax;
			if (num > offsetMax.x)
			{
				float num2 = Mathf.Abs(localPoint.y);
				Vector2 offsetMax2 = component.offsetMax;
				if (num2 > offsetMax2.y)
				{
					return;
				}
			}
			pointerDown = true;
			fingerId = eventData.pointerId;
			downTime = Time.realtimeSinceStartup;
			lastUpdateTime = downTime;
			animator.SetBool("hold", value: true);
			treatLikeAClick = true;
			destPercentPos = rectTransform.anchorMin;
		}

		private void OnGUI()
		{
			if (pointerDown)
			{
				treatLikeAClick = (Time.realtimeSinceStartup < downTime + 0.25f);
				UpdatePointerPos();
			}
			bool flag = Time.realtimeSinceStartup > upTime + 1f;
			if (pointerDown || !flag)
			{
				UpdateSmoothMove();
			}
			else if (Time.realtimeSinceStartup > upTime + 1f)
			{
				UpdateSmoothReturn();
			}
		}

		private void UpdatePointerPos()
		{
			destPercentPos = GetFingerPercentPos(fingerId);
		}

		private Vector2 GetFingerPercentPos(int fingerId)
		{
			Vector2 result = rectTransform.anchorMin;
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				Touch touch = UnityEngine.Input.GetTouch(i);
				if (touch.fingerId == fingerId)
				{
					Vector2 position = touch.position;
					float x = position.x / (float)Screen.width;
					Vector2 position2 = touch.position;
					result = new Vector2(x, position2.y / (float)Screen.height);
					break;
				}
			}
			return result;
		}

		private void UpdateSmoothMove()
		{
			if (Time.realtimeSinceStartup - lastUpdateTime > 0.01f)
			{
				lastUpdateTime = Time.realtimeSinceStartup;
				Vector2 vector = Vector2.Lerp(rectTransform.anchorMin, destPercentPos, 0.15f);
				RectTransform obj = rectTransform;
				Vector2 vector2 = vector;
				rectTransform.anchorMax = vector2;
				obj.anchorMin = vector2;
			}
		}

		private void UpdateSmoothReturn()
		{
			if (Time.realtimeSinceStartup - lastUpdateTime > 0.01f)
			{
				lastUpdateTime = Time.realtimeSinceStartup;
				Vector2 vector = Vector2.Lerp(rectTransform.anchorMin, basePercentPos, 0.2f);
				RectTransform obj = rectTransform;
				Vector2 vector2 = vector;
				rectTransform.anchorMax = vector2;
				obj.anchorMin = vector2;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (pointerDown)
			{
				pointerDown = false;
				fingerId = -1;
				upTime = Time.realtimeSinceStartup;
				if (treatLikeAClick)
				{
					animator.SetTrigger("open");
					StartCoroutine(TriggerClickAfter(0.5f));
				}
				else
				{
					animator.SetBool("hold", value: false);
				}
			}
		}

		private IEnumerator TriggerClickAfter(float realtimeDelay)
		{
			float delayEndTime = Time.realtimeSinceStartup + realtimeDelay;
			while (Time.realtimeSinceStartup < delayEndTime)
			{
				yield return null;
			}
			if (onClick != null)
			{
				onClick();
			}
		}
	}
}
