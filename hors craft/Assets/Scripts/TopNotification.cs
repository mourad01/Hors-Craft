// DecompilerFi decompiler from Assembly-CSharp.dll class: TopNotification
using Common.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopNotification : MonoBehaviour
{
	public class ShowInformation
	{
		public string information;

		public float timeToHide;

		public bool setOnTop;

		public ShowInformation()
		{
		}

		public ShowInformation(string text, float timeToHide = 1.5f, bool setAtTop = true)
		{
			information = text;
			this.timeToHide = timeToHide;
			setOnTop = setAtTop;
		}
	}

	public Transform parentTransform;

	public bool showing;

	public float hideAnimationTime = 0.18f;

	protected Queue<ShowInformation> queue = new Queue<ShowInformation>();

	public void Show(ShowInformation information)
	{
		if (showing)
		{
			queue.Enqueue(information);
			return;
		}
		base.gameObject.SetActive(value: true);
		showing = true;
		CancelInvoke();
		PrepareToShow(information.setOnTop);
		FireTriggrer("Show");
		SetElement(information);
		StartCoroutine(Hide(information.timeToHide));
	}

	public virtual void PrepareToShow(bool setAtTop)
	{
		if (parentTransform != null)
		{
			UnityEngine.Debug.Log("Parent world/local: " + parentTransform.position + " " + parentTransform.localPosition);
			UnityEngine.Debug.Log("Child  world/local: " + base.transform.position + " " + base.transform.localPosition);
		}
		base.gameObject.SetActive(value: true);
		if (setAtTop)
		{
			if (parentTransform != null)
			{
				parentTransform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
				parentTransform.SetAsLastSibling();
			}
			else
			{
				base.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
				base.transform.SetAsLastSibling();
			}
		}
		if (parentTransform != null)
		{
			UnityEngine.Debug.Log("Parent world/local: " + parentTransform.position + " " + parentTransform.localPosition);
			UnityEngine.Debug.Log("Child  world/local: " + base.transform.position + " " + base.transform.localPosition);
		}
	}

	public virtual void SetElement(ShowInformation information)
	{
		GetComponentInChildren<Text>().text = information.information;
	}

	public void FireTriggrer(string trigger)
	{
		Animator[] componentsInChildren = GetComponentsInChildren<Animator>();
		Animator[] array = componentsInChildren;
		foreach (Animator animator in array)
		{
			animator.SetTrigger(trigger);
		}
	}

	public IEnumerator Hide(float howLongShow)
	{
		yield return new WaitForSecondsRealtime(howLongShow);
		FireTriggrer("Hide");
		StartCoroutine(TryToGetNext());
	}

	protected IEnumerator TryToGetNext()
	{
		yield return new WaitForSecondsRealtime(hideAnimationTime);
		showing = false;
		base.gameObject.SetActive(value: false);
		if (queue.Count > 0)
		{
			ShowInformation information = queue.Dequeue();
			Show(information);
		}
	}

	public virtual void HideImmediately()
	{
		CancelInvoke();
		showing = false;
		base.gameObject.SetActive(value: false);
		queue = new Queue<ShowInformation>();
	}
}
