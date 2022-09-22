// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OutOfRangeModule
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class OutOfRangeModule : GameplayModule
	{
		public Text outOfRangeText;

		private Coroutine outOfRangeCoroutine;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.SURVIVAL_OUT_OF_RANGE
		};

		public override void Init()
		{
			base.Init();
			outOfRangeText.gameObject.SetActive(value: false);
		}

		public override void OnFactsChanged(HashSet<Fact> facts)
		{
			if (outOfRangeCoroutine != null)
			{
				StopCoroutine(outOfRangeCoroutine);
			}
			outOfRangeCoroutine = StartCoroutine(ShowOutOfRangeEx());
		}

		private IEnumerator ShowOutOfRangeEx()
		{
			outOfRangeText.gameObject.SetActive(value: true);
			Color c = outOfRangeText.color;
			c.a = 0.8f;
			outOfRangeText.color = c;
			yield return new WaitForSeconds(0.3f);
			float time = 0f;
			while (time < 0.2f)
			{
				c.a = 0.8f * Mathf.Clamp01(1f - time / 0.2f);
				outOfRangeText.color = c;
				time += Time.deltaTime;
				yield return null;
			}
			outOfRangeText.gameObject.SetActive(value: false);
		}
	}
}
