// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.ImageGrabber
using Common.Managers;
using UnityEngine;

namespace QuestSystems
{
	public class ImageGrabber : MonoBehaviour
	{
		public string mainFolder = "Quests";

		public string subKey = "quest";

		public string defaultKey = "default";

		public virtual Sprite GetSprite(int index, out Sprite newSprite, EQuestState state)
		{
			newSprite = null;
			if (state == EQuestState.afterDone)
			{
				state = EQuestState.done;
			}
			GetSprite(index, out newSprite, state.ToString());
			return newSprite;
		}

		public virtual bool GetSprite(int index, out Sprite newSprite, string fileName)
		{
			string text = GeneratePathToImage(index, fileName);
			newSprite = Resources.Load<Sprite>(text);
			if (newSprite == null)
			{
				text = GeneratePathToImage(index, defaultKey);
				newSprite = Resources.Load<Sprite>(text);
			}
			return (text != null) ? true : false;
		}

		protected virtual string GeneratePathToImage(int index, string fileName)
		{
			string gameName = Manager.Get<ConnectionInfoManager>().gameName;
			return $"{gameName}/{mainFolder}/{subKey}.{index}/{fileName}";
		}
	}
}
