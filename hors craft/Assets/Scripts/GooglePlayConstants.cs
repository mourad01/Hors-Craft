// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayConstants
using Common.Ksero;
using Common.Managers;
using Common.Model;
using System.Collections.Generic;

public class GooglePlayConstants : Singleton<GooglePlayConstants>
{
	private bool initialized;

	private Dictionary<string, string> constants;

	private void Initialize()
	{
		constants = new Dictionary<string, string>();
		Settings settingsForGame = KseroFiles.GetSettingsForGame(Manager.Get<ConnectionInfoManager>().gameName);
		string @string = settingsForGame.GetString("gpgs.resources");
		string[] array = @string.Split('\n');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(',');
			if (array3.Length == 2)
			{
				constants.Add(array3[0], array3[1].Replace("\"", string.Empty).Trim());
			}
		}
	}

	public string GetIDFor(string name, bool force = false)
	{
		if (!force && Manager.Contains<RankingManager>())
		{
			return name;
		}
		if (!initialized)
		{
			Initialize();
		}
		if (constants.TryGetValue(name, out string value))
		{
			return value;
		}
		return "not-found-in-ksero-runtime-settings";
	}
}
