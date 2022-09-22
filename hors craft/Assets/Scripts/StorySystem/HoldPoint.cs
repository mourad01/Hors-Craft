// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.HoldPoint
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StorySystem
{
	public class HoldPoint : TouchPoint
	{
		public float holdTime = 1f;

		protected override Color gizmosColor
		{
			[CompilerGenerated]
			get
			{
				return Color.magenta;
			}
		}

		/*protected override void PointTouched(TouchInfo info)
		{
			CheckHold(info);
		}*/

		[DebuggerStepThrough]
		/*[AsyncStateMachine(typeof(_003CCheckHold_003Ec__async0))]
		private void CheckHold(TouchInfo info)
		{
			_003CCheckHold_003Ec__async0 stateMachine = default(_003CCheckHold_003Ec__async0);
			stateMachine.info = info;
			stateMachine._0024this = this;
			stateMachine._0024builder = AsyncVoidMethodBuilder.Create();
			stateMachine._0024builder.Start(ref stateMachine);
		}*/

		protected override void DebugAction()
		{
			UnityEngine.Debug.LogError($"Hold at {base.worldPosition} point with radious {radius}");
		}

		protected override void GizmosDrawer()
		{
			Gizmos.DrawWireSphere(base.worldPosition, radius);
		}
	}
}
