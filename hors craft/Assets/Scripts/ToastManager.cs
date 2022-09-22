// DecompilerFi decompiler from Assembly-CSharp.dll class: ToastManager
using Common.Managers;
using UnityEngine;

public class ToastManager : Manager
{
	public Toast mainToasterPrefab;

	private Toast _toasterInstance;

	private Toast toasterInstance
	{
		get
		{
			if (_toasterInstance == null && mainToasterPrefab != null)
			{
				_toasterInstance = Object.Instantiate(mainToasterPrefab.gameObject).GetComponentInChildren<Toast>();
			}
			return _toasterInstance;
		}
	}

	public override void Init()
	{
	}

	public void HideNotificationImmediately()
	{
		if (toasterInstance != null)
		{
			toasterInstance.HideImmediately();
		}
	}

	public void ShowToast(string text)
	{
		if (!(toasterInstance == null))
		{
			TopNotification.ShowInformation information = new TopNotification.ShowInformation(text);
			toasterInstance.Show(information);
		}
	}

	public void ShowToast(string text, float time)
	{
		if (!(toasterInstance == null))
		{
			TopNotification.ShowInformation information = new TopNotification.ShowInformation(text, time);
			toasterInstance.Show(information);
		}
	}
}
