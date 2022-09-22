// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingMob
using System.Collections;
using UnityEngine;

namespace DragMinigame
{
	public class DragRacingMob : MonoBehaviour
	{
		private Animator animator;

		[SerializeField]
		private int animationsCount;

		public void Init()
		{
			StartCoroutine(initAnimationCO());
		}

		private IEnumerator initAnimationCO()
		{
			animator = GetComponent<Animator>();
			int animIndex = UnityEngine.Random.Range(0, animationsCount);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 2f));
			animator.SetBool(animIndex.ToString(), value: true);
		}

		private int FindRandomSkin(float random)
		{
			float num = random * SkinList.instance.sumOfWeights;
			float num2 = 0f;
			int num3 = -1;
			while (num3 < SkinList.instance.possibleSkins.Count - 1 && num2 <= num)
			{
				num3++;
				num2 += SkinList.instance.genderProbabilities[SkinList.instance.possibleSkins[num3].gender];
			}
			if (num3 < SkinList.instance.possibleSkins.Count - 1)
			{
				return num3;
			}
			return SkinList.instance.possibleSkins.Count - 1;
		}

		public void SetSkin(int index)
		{
			PlayerGraphic component = GetComponent<PlayerGraphic>();
			if (!(component == null))
			{
				component.SetWholeBodyl(index);
			}
		}
	}
}
