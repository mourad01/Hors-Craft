// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.DragPoints
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StorySystem
{
	public class DragPoints : TouchPoint
	{
		public Transform endpointTransform;

		[Header("Define the top one or bottom one")]
		public Vector3 endPoint;

		protected override Color gizmosColor
		{
			[CompilerGenerated]
			get
			{
				return Color.black;
			}
		}

		protected override void PointTouched(TouchInfo info)
		{
			info.onEnded = (Action<TouchInfo>)Delegate.Remove(info.onEnded, new Action<TouchInfo>(TouchEnded));
			info.onEnded = (Action<TouchInfo>)Delegate.Combine(info.onEnded, new Action<TouchInfo>(TouchEnded));
		}

		protected override void Start()
		{
			base.Start();
			CalcEndPoint();
		}

		private void TouchEnded(TouchInfo touch)
		{
			if (base.gameObject == null)
			{
				touch.onEnded = (Action<TouchInfo>)Delegate.Remove(touch.onEnded, new Action<TouchInfo>(TouchEnded));
			}
			else if (Vector3.Distance(touch.LastWorldPoint(), endPoint) < radius)
			{
				if (onDone != null)
				{
					onDone();
				}
				if (debugAction)
				{
					DebugAction();
				}
				touch.onEnded = (Action<TouchInfo>)Delegate.Remove(touch.onEnded, new Action<TouchInfo>(TouchEnded));
			}
		}

		protected override void DebugAction()
		{
			UnityEngine.Debug.LogError($"Draged from {base.worldPosition} to {endPoint}");
		}

		protected void CalcEndPoint()
		{
			if (endpointTransform != null)
			{
				endPoint = endpointTransform.position;
			}
			endPoint.z = 0f;
		}

		protected override void GizmosDrawer()
		{
			CalcEndPoint();
			Gizmos.DrawWireSphere(base.worldPosition, radius);
			Gizmos.DrawLine(base.worldPosition, endPoint);
			Gizmos.DrawWireSphere(endPoint, radius);
		}
	}
}
