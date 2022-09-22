// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestGiverMob
public class QuestGiverMob : HumanMob
{
	public bool automaticDespawning;

	public override void Despawn()
	{
		if (automaticDespawning)
		{
			base.Despawn();
		}
	}
}
