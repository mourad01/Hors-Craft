// DecompilerFi decompiler from Assembly-CSharp.dll class: FormFactory
using Common.Managers;
using Common.Utils;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class FormFactory
{
	public static WWWForm CreateBasicWWWForm()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.SignForm();
		return wWWForm;
	}

	public static void SignForm(this WWWForm form)
	{
		string gameName = Manager.Get<ConnectionInfoManager>().gameName;
		string applicationBundleIdentifier = VersionDependend.applicationBundleIdentifier;
		form.AddField("playerId", PlayerId.GetId());
		form.AddField("platform", GetPlatform());
		form.AddField("gamename", gameName);
		form.AddField("package_id", applicationBundleIdentifier);
		byte[] bytes = Encoding.UTF8.GetBytes(gameName);
		HMAC hMAC = new HMACSHA512(bytes);
		byte[] bytes2 = Encoding.ASCII.GetBytes(applicationBundleIdentifier);
		byte[] array = hMAC.ComputeHash(bytes2);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		form.AddField("hash", stringBuilder.ToString());
	}

	private static string GetPlatform()
	{
		return "android";
	}
}
