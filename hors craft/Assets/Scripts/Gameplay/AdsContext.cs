// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AdsContext
using Common.Managers.States;
using States;

namespace Gameplay
{
	public class AdsContext
	{
		public static string StateToStringId(State state)
		{
			if (state.GetType() == typeof(CookingAfterStageState))
			{
				return "cooking";
			}
			return state.GetType().Name.ToLower().Replace("state", string.Empty);
		}
	}
}
