// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.TranslationsSwapperController
using Common.Managers;
using UnityEngine;

namespace Common.Behaviours
{
	public class TranslationsSwapperController : MonoBehaviour
	{
		private const float THIRD_TOUCH_DELTA_TO_APPLY = 25f;

		private Vector2? thirdTouchPosition;

		private int lastThirdTouchFingerId;

		private void Update()
		{
			if (Application.isEditor || MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper)
			{
				UpdateDebugMode();
			}
		}

		private void UpdateDebugMode()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.T))
			{
				if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
				{
					Previous();
				}
				else if (UnityEngine.Input.GetKey(KeyCode.RightControl))
				{
					Next();
				}
			}
			if (UnityEngine.Input.touchCount == 3)
			{
				Touch touch = UnityEngine.Input.GetTouch(2);
				if (lastThirdTouchFingerId == touch.fingerId)
				{
					return;
				}
				if (thirdTouchPosition.HasValue)
				{
					Vector2 vector = touch.position - thirdTouchPosition.Value;
					if (vector.y < -25f)
					{
						Previous();
						lastThirdTouchFingerId = touch.fingerId;
					}
					else if (vector.y > 25f)
					{
						Next();
						lastThirdTouchFingerId = touch.fingerId;
					}
				}
				thirdTouchPosition = touch.position;
			}
			else
			{
				thirdTouchPosition = null;
				lastThirdTouchFingerId = -1;
			}
		}

		private void Previous()
		{
			GetComponent<TranslationsManager>().DownloadPreviousLanguage();
		}

		private void Next()
		{
			GetComponent<TranslationsManager>().DownloadNextLanguage();
		}
	}
}
