// DecompilerFi decompiler from Assembly-CSharp.dll class: SetTimer
using Common.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SetTimer : MonoBehaviour
{
	private bool running;

	public Text timer;

	private void OnEnable()
	{
		StartCoroutine(UpdateTime());
	}

	public void OnDisable()
	{
		running = false;
		StopAllCoroutines();
	}

	private IEnumerator UpdateTime()
	{
		if (running || !Manager.Contains<DynamicOfferPackManager>())
		{
			yield break;
		}
		DynamicOfferPackManager offerManager = Manager.Get<DynamicOfferPackManager>();
		if (offerManager == null || timer == null)
		{
			yield break;
		}
		running = true;
		while (true)
		{
			if (running)
			{
				if (!offerManager.ShouldShowPackButton())
				{
					break;
				}
				timer.text = offerManager.GetFormattedTimeToEnd();
				yield return new WaitForSeconds(1f);
				continue;
			}
			yield break;
		}
		MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.OFFERPACK_ENABLED);
	}
}
