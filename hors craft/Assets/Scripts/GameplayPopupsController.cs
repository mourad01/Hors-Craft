// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplayPopupsController
using Common.Managers;
using Common.Managers.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayPopupsController : MonoBehaviour
{
	[Serializable]
	public abstract class InitialPopup
	{
		public State stateToPush;

		public List<InitialPopupRequirements> requirements = new List<InitialPopupRequirements>();

		public bool useHasShownKey = true;

		public string hasShownKey;

		public InitialPopupExecution customExecution;

		public virtual bool ShouldDispose()
		{
			return requirements.Count > 0 && requirements.Any((InitialPopupRequirements r) => !r.CanBeShown());
		}

		public virtual bool IsReady(float timePassed)
		{
			return requirements.All((InitialPopupRequirements r) => r.CanBeShown());
		}
	}

	[Serializable]
	public class ActivationTimePopup : InitialPopup
	{
		public float activationTime;

		public bool IsReady(float timePassed)
		{
			return base.IsReady(timePassed) && activationTime <= timePassed;
		}
	}

	[Serializable]
	public class QueuePopup : InitialPopup
	{
		public float delay;

		public bool IsReady(float timePassed)
		{
			return base.IsReady(timePassed) && delay < 0f;
		}
	}

	[Serializable]
	public class NotDisposablePopup : ActivationTimePopup
	{
		public bool ShouldDispose()
		{
			return false;
		}
	}

	private float startTime;

	public static List<InitialPopup> initialPopups
	{
		get;
		private set;
	}

	public static Stack<InitialPopup> popupHistory
	{
		get;
		private set;
	}

	public bool afterInitialPopups => initialPopups.Count == 0;

	private float timePassed => Time.time - startTime;

	private void Awake()
	{
		startTime = Time.time;
		initialPopups = new List<InitialPopup>();
		popupHistory = new Stack<InitialPopup>();
	}

	public void AddPopup(InitialPopup popup)
	{
		initialPopups.Add(popup);
	}

	private void Update()
	{
		RemoveInvalidPopups();
		ActivatePopupIfAvaible();
		UpdateQueuePopups();
	}

	private void RemoveInvalidPopups()
	{
		initialPopups = (from p in initialPopups
			where !IsInvalid(p)
			select p).ToList();
	}

	private bool IsInvalid(InitialPopup popup)
	{
		return popup == null || popup.ShouldDispose() || HasAlreadyBeenShown(popup);
	}

	private bool HasAlreadyBeenShown(InitialPopup popup)
	{
		return popup.useHasShownKey && PlayerPrefs.GetInt(popup.hasShownKey, 0) == 1;
	}

	private void ActivatePopupIfAvaible()
	{
		InitialPopup initialPopup = initialPopups.FirstOrDefault((InitialPopup p) => p.IsReady(timePassed));
		if (initialPopup != null)
		{
			ActivatePopup(initialPopup);
			if (!(initialPopup is NotDisposablePopup))
			{
				initialPopups.Remove(initialPopup);
				popupHistory.Push(initialPopup);
			}
		}
	}

	private void ActivatePopup(InitialPopup popup)
	{
		if (popup.customExecution != null)
		{
			popup.customExecution.Show();
		}
		else
		{
			Type type = popup.stateToPush.GetType();
			if (!Manager.Get<StateMachineManager>().ContainsState(type))
			{
				UnityEngine.Debug.LogError("Tried to show initial popup, but state not found: " + type.ToString());
			}
			else
			{
				Manager.Get<StateMachineManager>().PushState(popup.stateToPush.GetType());
			}
		}
		if (popup.useHasShownKey)
		{
			PlayerPrefs.SetInt(popup.hasShownKey, 1);
		}
	}

	private void UpdateQueuePopups()
	{
		QueuePopup queuePopup = initialPopups.FirstOrDefault((InitialPopup p) => p is QueuePopup) as QueuePopup;
		if (queuePopup != null)
		{
			queuePopup.delay -= Time.deltaTime;
		}
	}
}
