// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.AnimatedRepeatButton
using UnityEngine;

namespace GameUI
{
	[RequireComponent(typeof(Animator))]
	public class AnimatedRepeatButton : SimpleRepeatButton
	{
		private Animator animator;

		private bool wasLastPressed;

		private void Start()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (!wasLastPressed && base.pressed)
			{
				StartPress();
			}
			else if (wasLastPressed && !base.pressed)
			{
				EndPress();
			}
			wasLastPressed = base.pressed;
		}

		private void StartPress()
		{
			animator.SetBool("pressed", value: true);
		}

		private void EndPress()
		{
			animator.SetBool("pressed", value: false);
		}
	}
}
