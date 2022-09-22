// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.Adventure.QuestWorldObjectAction
using Common.Managers;

namespace QuestSystems.Adventure
{
	public class QuestWorldObjectAction : QuestWorldObjectBase
	{
		public int itemToGiveAfterCutScene = -1;

		public override void OnQuestUpdate(EQuestState currentState)
		{
		}

		private void PlayCutScene()
		{
		}

		private void GiveItem()
		{
		}

		private bool GetCutSceneManager(out CutScenesManager manager)
		{
			manager = null;
			if (!Manager.Contains<CutScenesManager>())
			{
				return false;
			}
			manager = Manager.Get<CutScenesManager>();
			if (manager == null)
			{
				return false;
			}
			return manager;
		}
	}
}
