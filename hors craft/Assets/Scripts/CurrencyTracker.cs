// DecompilerFi decompiler from Assembly-CSharp.dll class: CurrencyTracker
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyTracker : MonoBehaviour
{
	public Text currencyText;

	private void Update()
	{
		currencyText.text = Manager.Get<AbstractSoftCurrencyManager>().GetProbableCurrency().ToString();
	}
}
