// DecompilerFi decompiler from Assembly-CSharp.dll class: GameSpecificObjectChanger
using Common.Managers;
using System;
using System.Linq;
using UnityEngine;

public class GameSpecificObjectChanger : MonoBehaviour
{
	[Serializable]
	public class ObjectConfiguration
	{
		public GameObject obj;

		public string[] gameNames;
	}

	[Header("Write common in gamename to set default configuration")]
	public ObjectConfiguration[] configurations;

	private void Awake()
	{
		string gameName = Manager.Get<ConnectionInfoManager>().gameName;
		bool flag = false;
		ObjectConfiguration[] array = configurations;
		foreach (ObjectConfiguration objectConfiguration in array)
		{
			bool flag2 = objectConfiguration.gameNames.Any((string g) => g.Equals(gameName));
			objectConfiguration.obj.SetActive(flag2);
			if (flag2)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			configurations.FirstOrDefault((ObjectConfiguration c) => c.gameNames.Contains("common"))?.obj.SetActive(value: true);
		}
	}
}
