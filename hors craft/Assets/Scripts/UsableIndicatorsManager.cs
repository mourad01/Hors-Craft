// DecompilerFi decompiler from Assembly-CSharp.dll class: UsableIndicatorsManager
using Common.Managers;

public class UsableIndicatorsManager : Manager
{
	public UsableIndicatorMultiConfig config;

	public override void Init()
	{
		UsableIndicatorMultiConfig.PrefabToIndicator[] prefabs = config.prefabs;
		for (int i = 0; i < prefabs.Length; i++)
		{
			UsableIndicatorMultiConfig.PrefabToIndicator prefabToIndicator = prefabs[i];
			prefabToIndicator.config.usableIndicatorPrefab = prefabToIndicator.usableIndicatorPrefab;
		}
	}
}
