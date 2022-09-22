// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.GameEvents.GameEventsManager
namespace Common.Utils.GameEvents
{
	public static class GameEventsManager
	{
		public delegate void OnGameEvent(GameEvent ev);

		public static event OnGameEvent listeners;

		public static void Propagate(GameEvent ev)
		{
			if (GameEventsManager.listeners != null)
			{
				GameEventsManager.listeners(ev);
			}
		}
	}
}
