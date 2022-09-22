// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestNotification
using Common.Managers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotification : TopNotification
{
	public class QuestUnlockInfo : ShowInformation
	{
		public bool isQuest;

		public int id = -1;
	}

	public Sprite empty;

	public Image image;

	public Text text;

	private Sprite GetSpriteForQuest(int id)
	{
		if (Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate() || Manager.Get<ModelManager>().worldsSettings.AreWorldQuestEnabled())
		{
			WorldData currentWorld = Manager.Get<SavedWorldManager>().GetCurrentWorld();
			int questCountPassedInWorld = Singleton<PlayerData>.get.playerQuests.GetQuestCountPassedInWorld(currentWorld.uniqueId);
			if (currentWorld.recepies == null)
			{
				return empty;
			}
			foreach (SavedWorldManager.Recipe recepy in currentWorld.recepies)
			{
				if (recepy.quest_needed == questCountPassedInWorld)
				{
					Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(recepy.id);
					UnityEngine.Debug.LogError("getting graphic");
					return craftable.GetGraphic();
				}
			}
			return empty;
		}
		Craftable craftable2 = Manager.Get<CraftingManager>().GetCraftable(id);
		return craftable2.GetGraphic();
	}

	public override void SetElement(ShowInformation information)
	{
		QuestUnlockInfo questUnlockInfo = information as QuestUnlockInfo;
		if (questUnlockInfo != null)
		{
			text.text = questUnlockInfo.information;
			if (!questUnlockInfo.isQuest)
			{
				image.sprite = GetSpriteForQuest(questUnlockInfo.id);
			}
		}
	}
}
