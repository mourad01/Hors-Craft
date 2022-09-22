// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Gameplay.UniversalAnalogInput
using Common.Behaviours;
using Common.GameUI;
using UnityEngine;

namespace Common.Gameplay
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

		private bool _touching;

		private bool touching
		{
			get
			{
				return _touching;
			}
			set
			{
				if (_touching && !value)
				{
					analog.OnTouchEnd();
				}
				_touching = value;
			}
		}

		public bool IsStickPressed => analogStick.pressed;

		public UniversalAnalogInput(AnalogController analog, SimpleRepeatButton stick)
		{
			this.analog = analog;
			analogStick = stick;
			touching = false;
		}

		public bool IsTouching()
		{
			return touching;
		}

		public bool IsAnalogNull()
		{
			return analog == null;
		}

		public Vector2 CalculatePosition()
		{
			if (IsAnalogNull())
			{
				return Vector2.zero;
			}
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
			AnalogController analogController = analog;
			CurrentInputInfo currentInputInfo = ConstructInputInfo();
			analogController.OnTouchStart(currentInputInfo.position);
		}

		private void PerformTouchContinue()
		{
			CurrentInputInfo currentInputInfo = ConstructInputInfo();
			analog.OnTouchContinue();
			Vector2 fingerOffset = (Vector2)currentInputInfo.position - analog.center;
			analog.SetFingerOffset(fingerOffset);
			analogPosition = 1.25f * (2f * analog.gaugeTransform.anchorMin - Vector2.one);
		}

		private CurrentInputInfo ConstructInputInfo()
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

		private Touch? FindCurrentTouch()
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (analogStick.fingerIds.Contains(UnityEngine.Input.GetTouch(i).fingerId))
				{
					return UnityEngine.Input.GetTouch(i);
				}
			}
			return null;
		}
	}
}
