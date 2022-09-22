// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyChestUI
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyChestUI : MonoBehaviour
{
	public RawImage chestImage;

	public Text timerText;

	public Text animatedText;

	public Button chestButton;

	private bool hideAfterOpen;

	private Type openedAtType;

	public GameObject connectedChest;

	public void Init(Vector2 position, bool hideAfterOpen)
	{
		this.hideAfterOpen = hideAfterOpen;
		base.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform);
		base.transform.localScale = Vector3.one;
		base.transform.SetAsLastSibling();
		TrySetPosition(position);
		SetUpButton();
		openedAtType = Manager.Get<StateMachineManager>().currentState.GetType();
	}

	private void Update()
	{
		string timeForNextChest = Manager.Get<DailyChestManager>().GetTimeForNextChest();
		timeForNextChest = ((!string.IsNullOrEmpty(timeForNextChest)) ? string.Format("{0}: {1}", Manager.Get<TranslationsManager>().GetText("dailychest.timer", "Free chest in"), timeForNextChest) : Manager.Get<TranslationsManager>().GetText("dailychest.freechest", "Free Chest!"));
		timerText.text = timeForNextChest;
		if (openedAtType != Manager.Get<StateMachineManager>().currentState.GetType())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void SetUpButton()
	{
		chestButton.onClick.AddListener(OnOpen);
	}

	private void AnimateOpen()
	{
		animatedText.text = $"{Manager.Get<DailyChestManager>().GetLastReward()}!";
		Animator component = connectedChest.GetComponent<Animator>();
		if (!(component == null))
		{
			component.SetTrigger("Open");
			animatedText.GetComponent<Animator>().SetTrigger("Show");
		}
	}

	private void AnimateInvalid()
	{
	}

	private void OnOpen()
	{
		if (Manager.Get<DailyChestManager>().TryOpenChest())
		{
			AnimateOpen();
		}
		else
		{
			AnimateInvalid();
		}
	}

	public void DestroyAfterAnimation()
	{
		if (hideAfterOpen)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void TrySetPosition(Vector2 position)
	{
		GetComponent<RectTransform>().anchorMax = position;
		GetComponent<RectTransform>().anchorMin = position;
		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
	}
}
