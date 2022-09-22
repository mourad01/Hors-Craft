// DecompilerFi decompiler from Assembly-CSharp.dll class: DisableMountingMobConfig
using System.Collections.Generic;
using UnityEngine;

public class DisableMountingMobConfig : AbstractMobConfig
{
	[Header("Type the exact names of game objects")]
	[SerializeField]
	private List<string> mobsToConfig;

	public override void Config(GameObject mobObj)
	{
		string item = mobObj.name.Replace("(Clone)", string.Empty);
		if (mobsToConfig.Contains(item))
		{
			Mountable component = mobObj.GetComponent<Mountable>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}
}
