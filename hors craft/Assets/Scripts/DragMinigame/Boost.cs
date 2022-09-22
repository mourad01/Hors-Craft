// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.Boost
using UnityEngine;

namespace DragMinigame
{
	public class Boost : MonoBehaviour
	{
		[HideInInspector]
		public float fullBoost = 4f;

		[HideInInspector]
		public float currentBoost;

		public void Init(float boostAmount)
		{
			fullBoost = boostAmount;
			currentBoost = fullBoost;
		}

		public void CalculateBoost()
		{
			currentBoost -= Time.deltaTime;
		}
	}
}
