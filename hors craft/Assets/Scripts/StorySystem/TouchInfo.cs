// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.TouchInfo
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StorySystem
{
	public class TouchInfo
	{
		public int fingerId;

		private TouchPhase _touchPhase;

		public List<Vector2> path = new List<Vector2>();

		public float minDistance = 0.1f;

		public Action<TouchInfo> onMoved;

		public Action<TouchInfo> onEnded;

		public TouchPhase touchPhase
		{
			get
			{
				return _touchPhase;
			}
			private set
			{
				switch (value)
				{
				case TouchPhase.Moved:
					if (onMoved != null)
					{
						onMoved(this);
					}
					break;
				case TouchPhase.Ended:
					if (onEnded != null)
					{
						onEnded(this);
					}
					break;
				}
				_touchPhase = value;
			}
		}

		public TouchInfo(Vector2 startPoint, int fingerId)
		{
			this.fingerId = fingerId;
			path.Add(startPoint);
			touchPhase = TouchPhase.Began;
		}

		public bool TryAddPoint(Vector2 point)
		{
			if (Vector2.Distance(path.Last(), point) > minDistance)
			{
				path.Add(point);
				touchPhase = TouchPhase.Moved;
				return true;
			}
			return false;
		}

		public void EndTouch()
		{
			touchPhase = TouchPhase.Ended;
		}

		public Vector3 WorldPoint(int index, Camera camera = null)
		{
			if (path.Count <= index)
			{
				return Vector3.zero;
			}
			if (camera == null)
			{
				camera = Camera.main;
			}
			Vector3 result = camera.ScreenToWorldPoint(path[index]);
			result.z = 0f;
			return result;
		}

		public Vector3 LastWorldPoint(Camera camera = null)
		{
			return WorldPoint(path.Count - 1, camera);
		}

		public Vector3 FirstWorldPoint(Camera camera = null)
		{
			return WorldPoint(0, camera);
		}
	}
}
