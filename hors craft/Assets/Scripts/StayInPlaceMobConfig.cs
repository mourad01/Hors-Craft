// DecompilerFi decompiler from Assembly-CSharp.dll class: StayInPlaceMobConfig
using System.Collections.Generic;
using UnityEngine;

public class StayInPlaceMobConfig : AbstractMobConfig
{
	[Header("Type the exact names of game objects")]
	[SerializeField]
	private List<string> mobsToConfig;

	public override void Config(GameObject mobObj)
	{
		string item = mobObj.name.Replace("(Clone)", string.Empty);
		if (mobsToConfig.Contains(item))
		{
			AnimalMob component = mobObj.GetComponent<AnimalMob>();
			if (component != null)
			{
				component.logic = AnimalMob.AnimalLogic.STAY_IN_PLACE;
				component.ReconstructBehaviourTree();
			}
		}
	}
}
