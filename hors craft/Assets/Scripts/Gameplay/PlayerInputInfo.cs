// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.PlayerInputInfo
using GameUI;
using UnityEngine;

namespace Gameplay
{
	public class PlayerInputInfo
	{
		public static SimpleRepeatButton inputArea;

		public static CurrentInputInfo inputInfo => ConstructInputInfo();

		private static CurrentInputInfo ConstructInputInfo()
		{
			if (inputArea != null && inputArea.pressed)
			{
				CurrentInputInfo currentInputInfo = new CurrentInputInfo();
				if (UnityEngine.Input.touchCount > 0)
				{
					Touch? touch = FindCurrentTouch(inputArea.fingerIds[inputArea.fingerIds.Count - 1]);
					if (touch.HasValue)
					{
						currentInputInfo.phase = touch.Value.phase;
						currentInputInfo.position = touch.Value.position;
					}
				}
				else
				{
					if (Input.GetMouseButtonDown(0))
					{
						currentInputInfo.phase = TouchPhase.Began;
					}
					else if (Input.GetMouseButton(0))
					{
						currentInputInfo.phase = TouchPhase.Moved;
					}
					else if (Input.GetMouseButtonUp(0))
					{
						currentInputInfo.phase = TouchPhase.Ended;
					}
					currentInputInfo.position = UnityEngine.Input.mousePosition;
				}
				return currentInputInfo;
			}
			CurrentInputInfo currentInputInfo2 = new CurrentInputInfo();
			currentInputInfo2.phase = TouchPhase.Canceled;
			currentInputInfo2.position = Vector3.zero;
			return currentInputInfo2;
		}

		private static Touch? FindCurrentTouch(int id)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (UnityEngine.Input.GetTouch(i).fingerId == id)
				{
					UnityEngine.Debug.LogError(i);
					return UnityEngine.Input.GetTouch(i);
				}
			}
			return null;
		}
	}
}
