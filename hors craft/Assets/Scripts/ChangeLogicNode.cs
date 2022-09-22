// DecompilerFi decompiler from Assembly-CSharp.dll class: ChangeLogicNode
using Common.BehaviourTrees;

internal class ChangeLogicNode : MobNode
{
	private AnimalMob.AnimalLogic logic;

	private AnimalMob animalMob;

	public ChangeLogicNode(AnimalMob mob, AnimalMob.AnimalLogic logic)
		: base(mob)
	{
		animalMob = mob;
		this.logic = logic;
	}

	public override void Update()
	{
		if (animalMob.logic != logic)
		{
			animalMob.SetLogicWithSavingPrevious(logic);
			animalMob.ReconstructBehaviourTree();
		}
		base.status = Status.SUCCESS;
	}
}
