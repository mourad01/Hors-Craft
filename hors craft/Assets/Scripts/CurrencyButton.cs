// DecompilerFi decompiler from Assembly-CSharp.dll class: CurrencyButton
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CurrencyButton : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(CurrencyShopState)))
			{
				Manager.Get<StateMachineManager>().PushState<CurrencyShopState>(new CurrencyShopParameter(noMoneyResponse: false));
			}
		});
	}
}
