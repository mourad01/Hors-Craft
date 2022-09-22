// DecompilerFi decompiler from Assembly-CSharp.dll class: WWWTimeOut
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWTimeOut : MonoBehaviour
{
	private float tempTime;

	public static void CreateWWW(float timeoutTime, Action<string> onSuccess, Action<string> onError, string url, byte[] postData = null, Dictionary<string, string> headers = null)
	{
		GameObject gameObject = new GameObject("request:" + url);
		gameObject.AddComponent<WWWTimeOut>().Init(timeoutTime, onSuccess, onError, url, postData, headers);
	}

	public void Init(float timeoutTime, Action<string> onSuccess, Action<string> onError, string url, byte[] postData = null, Dictionary<string, string> headers = null)
	{
		tempTime = 0f;
		StartCoroutine(StartRequest(timeoutTime, onSuccess, onError, url, postData, headers));
	}

	private IEnumerator StartRequest(float timeoutTime, Action<string> onSuccess, Action<string> onError, string url, byte[] postData = null, Dictionary<string, string> headers = null)
	{
		WWW request = (postData == null) ? new WWW(url) : ((headers != null) ? new WWW(url, postData, headers) : new WWW(url, postData));
		while (!request.isDone && request.error == null && tempTime < timeoutTime)
		{
			tempTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		if (request.error != null)
		{
			onError?.Invoke(request.error);
		}
		else if (tempTime > timeoutTime)
		{
			onError?.Invoke("timeout");
		}
		else
		{
			onSuccess?.Invoke(request.text);
		}
		onFinish();
	}

	private void onFinish()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
