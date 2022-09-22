// DecompilerFi decompiler from Assembly-CSharp.dll class: SimpleRequestMaker
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleRequestMaker : MonoBehaviour
{
	public static void MakeRequest(string mainUrl, string requestUrl, Dictionary<string, object> parameters, Action<WWW> onSuccess = null, Action<string> onError = null)
	{
		GameObject gameObject = new GameObject("Request:" + requestUrl);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(mainUrl);
		stringBuilder.Append(requestUrl);
		if (parameters != null && parameters.Count > 0)
		{
			stringBuilder.Append("?");
			stringBuilder.Append(ParametersToString(parameters));
		}
		SimpleRequestMaker simpleRequestMaker = gameObject.AddComponent<SimpleRequestMaker>();
		simpleRequestMaker.Init(stringBuilder.ToString(), onSuccess, onError);
	}

	private static string ParametersToString(Dictionary<string, object> parameters)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, object> parameter in parameters)
		{
			stringBuilder.Append($"{parameter.Key}={parameter.Value}");
			stringBuilder.Append("&");
		}
		stringBuilder.Length--;
		return stringBuilder.ToString();
	}

	public void Init(string url, Action<WWW> onSuccess, Action<string> onError)
	{
		StartCoroutine(RequestCoor(url, onSuccess, onError));
	}

	public void InitPost(WWWForm www, string url, Action<UnityWebRequest> onSuccess, Action<string> onError)
	{
		StartCoroutine(PostCoor(www, url, onSuccess, onError));
	}

	private IEnumerator PostCoor(WWWForm form, string url, Action<UnityWebRequest> onSuccess, Action<string> onError)
	{
		UnityWebRequest www = UnityWebRequest.Post(url, form);
		try
		{
			yield return www.Send();
			if (www.isNetworkError)
			{
				onError?.Invoke(www.error);
			}
			else
			{
				onSuccess?.Invoke(www);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		finally
		{
			//base._003C_003E__Finally0();
		}
	}

	public static void MakePost(string mainUrl, string requestUrl, Dictionary<string, object> parameters, string fileData, Action<UnityWebRequest> onSuccess = null, Action<string> onError = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(mainUrl);
		stringBuilder.Append(requestUrl);
		WWWForm wWWForm = FormFactory.CreateBasicWWWForm();
		foreach (KeyValuePair<string, object> parameter in parameters)
		{
			wWWForm.AddField(parameter.Key, parameter.Value.ToString());
		}
		wWWForm.AddField("data", fileData);
		MakePost(stringBuilder.ToString(), wWWForm, onSuccess, onError);
	}

	public static void MakePost(string url, WWWForm form, Action<UnityWebRequest> onSuccess = null, Action<string> onError = null)
	{
		GameObject gameObject = new GameObject("Post:" + url);
		SimpleRequestMaker simpleRequestMaker = gameObject.AddComponent<SimpleRequestMaker>();
		simpleRequestMaker.InitPost(form, url, onSuccess, onError);
	}

	private IEnumerator RequestCoor(string url, Action<WWW> onSuccess, Action<string> onError)
	{
		WWW www = new WWW(url, FormFactory.CreateBasicWWWForm());
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError?.Invoke(www.error);
		}
		else
		{
			onSuccess?.Invoke(www);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
