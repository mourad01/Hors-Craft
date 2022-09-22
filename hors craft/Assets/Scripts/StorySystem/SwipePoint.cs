// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.SwipePoint
using CommonAttribute;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StorySystem
{
	public class SwipePoint : TouchPoint
	{
		public float minDistance = 1f;

		public float directionAngle;

		public float torelanceAngle = 40f;

		[ReadOnly]
		public Vector2 direction;

		protected override Color gizmosColor
		{
			[CompilerGenerated]
			get
			{
				return Color.red;
			}
		}

		protected override void PointTouched(TouchInfo info)
		{
			info.onMoved = (Action<TouchInfo>)Delegate.Remove(info.onMoved, new Action<TouchInfo>(TouchMoved));
			info.onMoved = (Action<TouchInfo>)Delegate.Combine(info.onMoved, new Action<TouchInfo>(TouchMoved));
		}

		private void TouchMoved(TouchInfo touch)
		{
			if (base.gameObject == null)
			{
				touch.onMoved = (Action<TouchInfo>)Delegate.Remove(touch.onMoved, new Action<TouchInfo>(TouchMoved));
			}
			else
			{
				if (!(Vector3.Distance(touch.LastWorldPoint(), touch.FirstWorldPoint()) >= minDistance))
				{
					return;
				}
				Vector3 normalized = (touch.LastWorldPoint() - touch.FirstWorldPoint()).normalized;
				float f = Vector3.Dot(direction, normalized);
				float num = Mathf.Acos(f);
				float num2 = num * 57.29578f;
				if (num2 >= directionAngle - torelanceAngle && num2 <= directionAngle + torelanceAngle)
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
				touch.onMoved = (Action<TouchInfo>)Delegate.Remove(touch.onMoved, new Action<TouchInfo>(TouchMoved));
			}
		}

		protected override void DebugAction()
		{
			UnityEngine.Debug.LogError($"Swipe at {base.worldPosition} point with radious {radius}");
		}

		[ContextMenu("calcDir")]
		public void CalcDirection()
		{
			direction = new Vector2(Mathf.Cos(directionAngle * ((float)Math.PI / 180f)), Mathf.Sin(directionAngle * ((float)Math.PI / 180f)));
		}

		protected override void GizmosDrawer()
		{
			CalcDirection();
			float f = (directionAngle + torelanceAngle) * ((float)Math.PI / 180f);
			Vector3 vector = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
			Gizmos.DrawWireSphere(base.worldPosition, radius);
			Gizmos.DrawLine(base.transform.position, base.transform.position + new Vector3(direction.x * minDistance, direction.y * minDistance, 0f));
			Gizmos.DrawLine(base.transform.position, base.transform.position + new Vector3(vector.x * minDistance, vector.y * minDistance, 0f));
			f = (directionAngle - torelanceAngle) * ((float)Math.PI / 180f);
			vector = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
			Gizmos.DrawLine(base.transform.position, base.transform.position + new Vector3(vector.x * minDistance, vector.y * minDistance, 0f));
		}
	}
}
