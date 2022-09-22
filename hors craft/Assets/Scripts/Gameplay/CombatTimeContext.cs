// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CombatTimeContext
namespace Gameplay
{
	public class CombatTimeContext : SurvivalContext
	{
		public bool isCombat;

		public bool becauseRestarted;

		public override string ToString()
		{
			return $"IsCombat: {isCombat}; Restarted: {becauseRestarted};";
		}
	}
}
