// DecompilerFi decompiler from Assembly-CSharp.dll class: CheckConnectionInfoLink
using Common.Managers;
using UnityEngine;

public class CheckConnectionInfoLink : MonoBehaviour
{
	private void Awake()
	{
		if (!Manager.Get<ConnectionInfoManager>().homeURL.Contains("dev"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
