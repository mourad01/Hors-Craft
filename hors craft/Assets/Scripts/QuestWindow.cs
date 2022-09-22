// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestWindow
using Common.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
	public delegate void OnClick();

	public GameObject questNeededPrefab;

	public GameObject questParent;

	public Image previewImage;

	public OnClick onReturnButton;

	public Text numberOfItems;

	public Button returnButton;

	public void Init(int itemId)
	{
		Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
		previewImage.sprite = ((!(craftable.sprite == null)) ? craftable.sprite : VoxelSprite.GetVoxelSprite((ushort)craftable.blockId));
		createNeededQuests(craftable);
		numberOfItems.text = Mathf.Max(0, Singleton<PlayerData>.get.playerItems.GetCraftableCount(itemId)).ToString();
	}

	private void createNeededQuests(Craftable craftable)
	{
		destroyCurrentQuests();
		for (int i = 0; i < craftable.unlockingQuestId.Count; i++)
		{
			CreateElement(Manager.Get<QuestManager>().questById[craftable.unlockingQuestId[i]]);
		}
	}

	private GameObject CreateElement(Quest quest)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(questNeededPrefab);
		gameObject.transform.parent = questParent.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		gameObject.GetComponent<CraftItem>().text.text = Manager.Get<QuestManager>().GetQuestDescription(quest.id);
		gameObject.GetComponent<CraftItem>().mainSprite.sprite = Manager.Get<QuestManager>().GetImageForQuest(quest.id);
		return gameObject;
	}

	private void destroyCurrentQuests()
	{
		IEnumerator enumerator = questParent.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}
}
