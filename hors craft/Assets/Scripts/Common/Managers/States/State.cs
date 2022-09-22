// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.State
using UnityEngine;

namespace Common.Managers.States
{
	public abstract class State : MonoBehaviour
	{
		public abstract void StartState(StartParameter startParameter);

		public abstract void UpdateState();

		public abstract void FreezeState();

		public abstract void UnfreezeState();

		public abstract void FinishState();

		public virtual void PrepareState()
		{
		}
	}
}
