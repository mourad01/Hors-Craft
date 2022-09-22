// DecompilerFi decompiler from Assembly-CSharp.dll class: ReturnButton
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReturnButton : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate
		{
			Manager.Get<StateMachineManager>().PopState();
		});
	}
}
