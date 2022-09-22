// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.TouchPoint
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StorySystem
{
	public class TouchPoint : MonoBehaviour
	{
		public float radius = 2f;

		public Action onDone;

		public bool debugAction;

		protected Vector3 worldPosition
		{
			get
			{
				Vector3 position = base.transform.position;
				position.z = 0f;
				return position;
			}
		}

		protected virtual Color gizmosColor
		{
			[CompilerGenerated]
			get
			{
				return Color.yellow;
			}
		}

		protected virtual void Start()
		{
			TouchController get = MonoBehaviourSingleton<TouchController>.get;
			get.newTouchInfo = (Action<TouchInfo>)Delegate.Remove(get.newTouchInfo, new Action<TouchInfo>(NewTouch));
			TouchController get2 = MonoBehaviourSingleton<TouchController>.get;
			get2.newTouchInfo = (Action<TouchInfo>)Delegate.Combine(get2.newTouchInfo, new Action<TouchInfo>(NewTouch));
		}

		protected void NewTouch(TouchInfo info)
		{
			if (Vector3.Distance(info.LastWorldPoint(), worldPosition) < radius)
			{
				PointTouched(info);
			}
		}

		protected virtual void PointTouched(TouchInfo info)
		{
			if (onDone != null)
			{
				onDone();
			}
			if (debugAction)
			{
				DebugAction();
			}
		}

		protected virtual void DebugAction()
		{
			UnityEngine.Debug.LogError($"Taped at {worldPosition} point with radious {radius}");
			UnityEngine.Object.Destroy(this);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = gizmosColor;
			GizmosDrawer();
		}

		protected virtual void GizmosDrawer()
		{
			Gizmos.DrawWireSphere(worldPosition, radius);
		}

		private void OnDisable()
		{
			if (TouchController.isInstance)
			{
				TouchController get = MonoBehaviourSingleton<TouchController>.get;
				get.newTouchInfo = (Action<TouchInfo>)Delegate.Remove(get.newTouchInfo, new Action<TouchInfo>(NewTouch));
			}
		}
	}
}
