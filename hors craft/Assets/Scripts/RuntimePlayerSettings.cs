// DecompilerFi decompiler from Assembly-CSharp.dll class: RuntimePlayerSettings
using UnityEngine;

public class RuntimePlayerSettings
{
	private const string ASSET_PATH = "Assets/Resources/RuntimePlayerSettings.asset";

	private const string ASSET_PATH_IN_RESOURCES = "RuntimePlayerSettings";

	private static RuntimePlayerSettingsInstance loadedAsset;

	public static RuntimePlayerSettingsInstance get => GetAsset();

	private static RuntimePlayerSettingsInstance GetAsset()
	{
		loadedAsset = Resources.Load<RuntimePlayerSettingsInstance>("RuntimePlayerSettings");
		if (loadedAsset == null)
		{
			UnityEngine.Debug.LogError("There's no RuntimePlayerSettings asset in Resources!");
		}
		return loadedAsset;
	}
}
