// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.PlayerBetterMovement
using System.Collections;
using UnityEngine;

namespace Uniblocks
{
	public class PlayerBetterMovement : PlayerMovement
	{
		private const string ANIMATOR_MODE_CHANGE_KEY = "locomotionChange";

		private const string ANIMATOR_MODE_TYPE_KEY = "locomotionType";

		private const string ANIMATOR_LOCOMOTION_BLEND_VALUE_KEY = "locomotionIdleActiveBlend";

		private Mode currentAnimationMode;

		public override void ChangeMoveMode(Mode newMode)
		{
			base.ChangeMoveMode(newMode);
			if (!(playerAnimator == null))
			{
				playerAnimator.SetTrigger("locomotionChange");
				playerAnimator.SetInteger("locomotionType", (int)newMode);
				StartCoroutine(CheckAnimation(newMode));
			}
		}

		private IEnumerator CheckAnimation(Mode newMode)
		{
			yield return null;
			playerAnimator.SetInteger("locomotionType", (int)newMode);
			playerAnimator.SetTrigger("locomotionChange");
		}

		protected override void UpdateAnimatorState()
		{
			if (playerAnimator.gameObject.activeInHierarchy)
			{
				float bigger = GetBigger(playerInputX, playerInputY);
				playerAnimator.SetFloat("locomotionIdleActiveBlend", bigger);
				playerAnimator.SetFloat("testPlayerInputX", playerInputX);
				playerAnimator.SetFloat("testPlayerInputY", playerInputY);
			}
		}

		protected void OnEnable()
		{
			ChangeMoveMode(mode);
		}

		protected override void UnderwaterStatusWillChange()
		{
			playerAnimator.SetTrigger("locomotionChange");
			if (base.underwater)
			{
				playerAnimator.SetInteger("locomotionType", (mode == Mode.FLYING) ? 1 : 0);
			}
			else
			{
				playerAnimator.SetInteger("locomotionType", 2);
			}
		}

		private float GetBigger(float valA, float valB)
		{
			return (!(Mathf.Abs(valA) > Mathf.Abs(valB))) ? Mathf.Abs(valB) : Mathf.Abs(valA);
		}

		private bool IsEqual(float a, float b)
		{
			if (a >= b - Mathf.Epsilon && a <= b + Mathf.Epsilon)
			{
				return true;
			}
			return false;
		}
	}
}
