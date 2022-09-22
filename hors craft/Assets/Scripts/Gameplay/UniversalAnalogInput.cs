// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.UniversalAnalogInput
using Common.Behaviours;
using GameUI;
using UnityEngine;

namespace Gameplay
{
	public class UniversalAnalogInput
	{
		private struct CurrentInputInfo
		{
			public TouchPhase phase;

			public Vector3 position;
		}

		private Vector2 analogPosition;

		private AnalogController analog;

		private SimpleRepeatButton analogStick;

		private bool touching;

		public bool IsStickPressed => analogStick.pressed;

		public UniversalAnalogInput(AnalogController analog, SimpleRepeatButton stick)
		{
			Init(analog, stick);
		}

		public void Init(AnalogController analog, SimpleRepeatButton stick)
		{
			this.analog = analog;
			analogStick = stick;
			touching = false;
		}

		public bool IsInit()
		{
			return analog != null && analogStick != null;
		}

		public bool IsTouching()
		{
			return touching;
		}

		public Vector2 CalculatePosition()
		{
			if (!analogStick.pressed)
			{
				analog.SetGaugeCenterPositon();
				Reset();
			}
			else
			{
				UpdateTouch();
			}
			return analogPosition;
		}

		public Vector2 GetStickAnchor()
		{
			return analogStick.GetComponent<RectTransform>().anchorMax;
		}

		public bool IsInDeadZone(float deadZoneSize)
		{
			return CalculatePosition().sqrMagnitude < deadZoneSize;
		}

		private void Reset()
		{
			touching = false;
			analogPosition = Vector2.zero;
		}

		private void UpdateTouch()
		{
			if (!touching)
			{
				if (analogStick.pressed)
				{
					PerformTouchStart();
					touching = true;
				}
			}
			else if (analogStick.pressed)
			{
				PerformTouchContinue();
			}
			else
			{
				touching = false;
			}
		}

		private bool IsCorrectTouchDown()
		{
			return analogStick.pressed;
		}

		private bool IsCorrectTouchUp()
		{
			return Input.GetMouseButtonUp(0);
		}

		private void PerformTouchStart()
		{
			analog.UpdateAnalogControllerPosition();
		}

		private void PerformTouchContinue()
		{
			CurrentInputInfo currentInputInfo = ConstructInputInfo();
			Vector2 fingerOffset = (Vector2)currentInputInfo.position - analog.center;
			analog.SetFingerOffset(fingerOffset);
			analogPosition = 1.25f * (2f * analog.gaugeTransform.anchorMin - Vector2.one);
		}

		private CurrentInputInfo ConstructInputInfo()
		{
			if (analogStick != null && analogStick.pressed)
			{
				CurrentInputInfo result = default(CurrentInputInfo);
				if (UnityEngine.Input.touchCount > 0)
				{
					Touch? touch = FindCurrentTouch();
					if (touch.HasValue)
					{
						result.phase = touch.Value.phase;
						result.position = touch.Value.position;
					}
				}
				else
				{
					if (MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper)
					{
						if (Input.GetMouseButtonDown(2))
						{
							result.phase = TouchPhase.Began;
						}
						else if (Input.GetMouseButton(2))
						{
							result.phase = TouchPhase.Moved;
						}
					}
					else if (Input.GetMouseButtonDown(0))
					{
						result.phase = TouchPhase.Began;
					}
					else if (Input.GetMouseButton(0))
					{
						result.phase = TouchPhase.Moved;
					}
					result.position = UnityEngine.Input.mousePosition;
				}
				return result;
			}
			CurrentInputInfo result2 = default(CurrentInputInfo);
			result2.phase = TouchPhase.Canceled;
			result2.position = Vector3.zero;
			return result2;
		}

		private Touch? FindCurrentTouch()
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (UnityEngine.Input.GetTouch(i).fingerId == analogStick.fingerId)
				{
					return UnityEngine.Input.GetTouch(i);
				}
			}
			return null;
		}
	}
}
