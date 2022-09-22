// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.OverrideAnimatorSpeed
using UnityEngine;

namespace Common.Behaviours
{
	public class OverrideAnimatorSpeed : MonoBehaviour
	{
		public float speed = 1f;

		private void Awake()
		{
			Animator component = GetComponent<Animator>();
			if (component != null)
			{
				component.speed = speed;
			}
		}
	}
}
