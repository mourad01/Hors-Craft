// DecompilerFi decompiler from Assembly-CSharp.dll class: GoogleAnalyticsV4Manager
using Common.Managers;
using UnityEngine;

public class GoogleAnalyticsV4Manager : Manager
{
	public GameObject googleAnalyticsV4Prefab;

	public string androidTrackingCode;

	public string IOSTrackingCode;

	public string otherTrackingCode;

	private bool anonymizeIP = true;

	private bool sendLaunchEvent = true;

	private bool enableAdId = true;

	public override void Init()
	{
	}

	public override void OnConsentSpecified(bool consentAquired)
	{
	}
}
