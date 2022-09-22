// DecompilerFi decompiler from Assembly-CSharp.dll class: PrivacyPolicyButton
using Common.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PrivacyPolicyButton : MonoBehaviour
{
	public string defaultUrl = "https://projectx-mobile.apps.tensquaregames.com/policy/bcindex";

	private string downloadedUrl
	{
		get
		{
			return PlayerPrefs.GetString("privacy.policy.url", defaultUrl);
		}
		set
		{
			PlayerPrefs.SetString("privacy.policy.url", value);
		}
	}

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	private void Start()
	{
		StartCoroutine(DownloadURL());
	}

	private IEnumerator DownloadURL()
	{
		string homeUrl = Manager.Get<ConnectionInfoManager>().homeURL;
		WWW www = new WWW(homeUrl + "policy/getUrl?packageid=" + VersionDependend.applicationBundleIdentifier, FormFactory.CreateBasicWWWForm());
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			UnityEngine.Debug.LogWarning("Download policy URL error: " + www.error);
			yield return null;
		}
		if (!string.IsNullOrEmpty(www.text))
		{
			UnityEngine.Debug.Log("Downloaded policy URL: " + www.text);
			downloadedUrl = www.text;
		}
	}

	private void OnClick()
	{
		Application.OpenURL(ConstructURL());
	}

	private string ConstructURL()
	{
		string str = (!string.IsNullOrEmpty(downloadedUrl)) ? downloadedUrl : defaultUrl;
		str = str + "?gamename=" + WWW.EscapeURL(Manager.Get<ConnectionInfoManager>().gameName);
		str = str + "&gametitle=" + WWW.EscapeURL(Application.productName);
		return str + "&platform=android";
	}
}
