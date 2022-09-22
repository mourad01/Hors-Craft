// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.MultiplayerModule
using Common.Model;
using UnityEngine;

namespace Common.Managers
{
	public class MultiplayerModule : ModelModule
	{
		public string keyShowMultiplayer()
		{
			return "show.multiplayer";
		}

		public bool MultiplayerEnabled()
		{
			return base.settings.GetInt(keyShowMultiplayer()) > 0;
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyShowMultiplayer(), 0);
		}

		public override void OnModelDownloaded()
		{
			UnityEngine.Debug.Log("SHOW MULTI: " + base.settings.GetInt(keyShowMultiplayer()));
		}
	}
}
