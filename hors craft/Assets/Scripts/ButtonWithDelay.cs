// DecompilerFi decompiler from Assembly-CSharp.dll class: ButtonWithDelay
using Common.Managers;
using States;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithDelay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Button button;

	public int index;

	public float delay = 1f;

	public void OnPointerClick(PointerEventData data)
	{
		UnityEngine.Debug.Log("Clicked!");
		AdventureQuestState adventureQuestState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(AdventureQuestState)) as AdventureQuestState;
		if (adventureQuestState != null)
		{
			adventureQuestState.ChoosedAnswer(index);
		}
		StartCoroutine(Wait(2f));
	}

	private IEnumerator Wait(float time)
	{
		while (time > 0f)
		{
			time -= Time.unscaledDeltaTime;
			yield return null;
		}
		button.onClick.Invoke();
	}
}
