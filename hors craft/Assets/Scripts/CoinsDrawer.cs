// DecompilerFi decompiler from Assembly-CSharp.dll class: CoinsDrawer
using Common.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoinsDrawer : MonoBehaviour
{
	public Text addedGold;

	public Text mainText;

	private float timeShowing = 1.5f;

	private float currentShowTime;

	private int currencyChange;

	public void OnCurrencyChange(int changeValue)
	{
		int probableCurrency = Manager.Get<AbstractSoftCurrencyManager>().GetProbableCurrency();
		if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Showing"))
		{
			currencyChange += changeValue;
			currentShowTime = 0f;
		}
		else
		{
			currencyChange = changeValue;
			if (base.transform.gameObject.activeInHierarchy)
			{
				StartCoroutine(ShowWindow());
			}
		}
		mainText.text = probableCurrency.ToString();
		addedGold.text = $"+ {currencyChange.ToString()}";
	}

	private IEnumerator ShowWindow()
	{
		currentShowTime = 0f;
		GetComponent<Animator>().SetTrigger("Show");
		while (timeShowing > currentShowTime)
		{
			yield return new WaitForSeconds(0.1f);
			currentShowTime += 0.1f;
		}
		GetComponent<Animator>().SetTrigger("Hide");
	}
}
