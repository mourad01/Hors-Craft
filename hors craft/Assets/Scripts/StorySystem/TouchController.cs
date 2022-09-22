// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.TouchController
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StorySystem
{
	public class TouchController : MonoBehaviourSingleton<TouchController>
	{
		private List<TouchInfo> currentTouchInfos = new List<TouchInfo>();

		public static bool isInstance;

		public Action<TouchInfo> newTouchInfo;

		private Camera mainCamera
		{
			[CompilerGenerated]
			get
			{
				return Camera.main;
			}
		}

		private void Update()
		{
			for (int num = currentTouchInfos.Count - 1; num >= 0; num--)
			{
				if (TouchExist(currentTouchInfos[num].fingerId))
				{
					currentTouchInfos[num].TryAddPoint(GetTouchPosition(currentTouchInfos[num].fingerId));
				}
				else
				{
					currentTouchInfos[num].EndTouch();
					currentTouchInfos.RemoveAt(num);
				}
			}
			FindNewTouches();
		}

		private void FindNewTouches()
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				if (Input.touches[i].phase == TouchPhase.Began)
				{
					AddNewToch(UnityEngine.Input.mousePosition, Input.touches[i].fingerId);
				}
			}
		}

		private void AddNewToch(Vector2 startPoint, int fingerId)
		{
			TouchInfo touchInfo = new TouchInfo(UnityEngine.Input.mousePosition, fingerId);
			currentTouchInfos.Add(touchInfo);
			newTouchInfo(touchInfo);
		}

		private Vector2 GetTouchPosition(int fingerId)
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				if (Input.touches[i].fingerId == fingerId)
				{
					return Input.touches[i].position;
				}
			}
			return Vector2.zero;
		}

		public static bool TouchExist(int fingerId)
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				if (Input.touches[i].fingerId == fingerId && Input.touches[i].phase != TouchPhase.Ended)
				{
					return true;
				}
			}
			return false;
		}
	}
}
