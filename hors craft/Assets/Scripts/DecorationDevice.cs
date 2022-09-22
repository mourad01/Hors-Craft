// DecompilerFi decompiler from Assembly-CSharp.dll class: DecorationDevice
using System.Linq;

public class DecorationDevice : Device
{
	public bool useBaseSetVisualization;

	protected override void SetUpgradeValues(UpgradeEffect effect, float value)
	{
	}

	protected override void SetVisualization(int upgrade)
	{
		if (useBaseSetVisualization)
		{
			base.SetVisualization(upgrade);
		}
		else if (upgradePrefabs != null && upgradePrefabs.Length != 0)
		{
			(from up in upgradePrefabs
				where upgrade >= up.minLevel
				select up).ToList();
			upgradePrefabs.ToList().ForEach(delegate(UpgradeConfig up)
			{
				up.prefab.SetActive(upgrade >= up.minLevel);
			});
		}
	}
}
