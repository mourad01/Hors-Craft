// DecompilerFi decompiler from Assembly-CSharp.dll class: SoldiersRecruiterContextGenerator
using UnityEngine;

public class SoldiersRecruiterContextGenerator : MonoBehaviour
{
	private SoldiersRecruiterContext context;

	public bool isRecruitSoldierEnabled = true;

	public GameObject[] friendlySoldiersPrefabs;

	public Material[] friendlySoldierClothes;

	public Material[] enemySoldierClothes;

	private void Start()
	{
		InitContext();
		SurvivalContextsBroadcaster.instance.UpdateContext(context);
	}

	private void InitContext()
	{
		context = new SoldiersRecruiterContext
		{
			isRecruitSoldierEnabled = isRecruitSoldierEnabled,
			friendlySoldiersPrefabs = friendlySoldiersPrefabs,
			friendlySoldierClothes = friendlySoldierClothes,
			enemySoldierClothes = enemySoldierClothes
		};
	}
}
