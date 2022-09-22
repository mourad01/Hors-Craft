// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerData
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData>
{
	public PlayerItems playerItems;

	public PlayerQuests playerQuests;

	public PlayerWorlds playerWorlds;

	public PlayerPurchases playerPurchases;

	public PlayerData()
	{
		playerItems = PlayerItems.LoadFromPlayerPrefs();
		playerQuests = PlayerQuests.LoadFromPlayerPrefs();
		playerWorlds = PlayerWorlds.LoadFromPlayerPrefs();
		playerPurchases = new PlayerPurchases();
	}

	public void OnWorldChange()
	{
		playerItems = PlayerItems.LoadFromPlayerPrefs();
		playerQuests = PlayerQuests.LoadFromPlayerPrefs();
		playerWorlds = PlayerWorlds.LoadFromPlayerPrefs();
	}

	public void GetRedisId(Action<string> idConsumer)
	{
		SimpleRequestMaker.MakeRequest(Manager.Get<ConnectionInfoManager>().homeURL, "SoftCurrency/PlayerId", new Dictionary<string, object>
		{
			{
				"deviceId",
				SystemInfo.deviceUniqueIdentifier
			}
		}, delegate(WWW www)
		{
			idConsumer(www.text);
		}, delegate
		{
			UnityEngine.Debug.LogError(idConsumer);
		});
	}
}
