// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestsDrawer
using Common.Behaviours;
using Common.Managers;
using States;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsDrawer : MonoBehaviour
{
	public QuestElement prefabElement;

	public List<QuestElement> quests;

	public GameObject questContainer;

	public Button drawerButton;

	public Button rerollButton;

	public Button passButton;

	public GameObject questNotification;

	public Button openCloseCatcher;

	public GameObject noMoreQuestContent;

	public Button goToWorldsButton;

	private void Start()
	{
		CreateQuests();
		OnRerollQuests();
		OnPassQuests();
		drawerButton.onClick.AddListener(delegate
		{
			OnButtonClick();
		});
		openCloseCatcher.onClick.AddListener(delegate
		{
			OnButtonClick();
		});
		goToWorldsButton.onClick.AddListener(delegate
		{
			GoToWorlds();
		});
		Singleton<PlayerData>.get.playerQuests.ClearAndAddWorldQuestListener(delegate
		{
			Revalidate();
		});
	}

	private void GoToWorlds()
	{
		Manager.Get<StateMachineManager>().PushState<WorldShopState>();
	}

	private void CreateQuests()
	{
		if (quests == null)
		{
			quests = new List<QuestElement>();
		}
		Revalidate();
	}

	private void OnClaimQuest(Quest quest)
	{
		Manager.Get<QuestManager>().ClaimQuest(quest);
		Revalidate();
	}

	private void AddNewQuestObject()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabElement.gameObject);
		quests.Add(gameObject.GetComponent<QuestElement>());
		gameObject.transform.SetParent(questContainer.transform);
		gameObject.transform.localScale = Vector3.one;
	}

	private void Revalidate()
	{
		bool flag = false;
		List<Quest> currentWorldQuests = Manager.Get<QuestManager>().GetCurrentWorldQuests();
		if (quests != null)
		{
			quests.ForEach(delegate(QuestElement alfa)
			{
				if ((bool)alfa)
				{
					alfa.gameObject.SetActive(value: false);
				}
			});
		}
		for (int i = 0; i < WorldsQuests.numberOfActiveQuests && i < currentWorldQuests.Count; i++)
		{
			if (quests.Count <= i)
			{
				AddNewQuestObject();
			}
			if (i <= currentWorldQuests.Count && (bool)quests[i])
			{
				quests[i].gameObject.SetActive(value: true);
				quests[i].InitializeWithData(currentWorldQuests[i], Singleton<PlayerData>.get.playerQuests.GetWorldQuestProgress(currentWorldQuests[i].GenerateWorldId()), openCloseCatcher.GetComponent<RectTransform>(), OnClaimQuest);
				if (!flag)
				{
					flag = quests[i].isReadyForClaim;
				}
			}
		}
		if (questNotification != null)
		{
			questNotification.SetActive(flag);
		}
		if ((bool)questContainer)
		{
			questContainer.SetActive(currentWorldQuests.Count != 0);
		}
		noMoreQuestContent.SetActive(currentWorldQuests.Count == 0);
		rerollButton.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
		passButton.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
	}

	private void OnRerollQuests()
	{
		rerollButton.onClick.AddListener(delegate
		{
			Manager.Get<QuestManager>().ReRollWorldQuest();
			Revalidate();
		});
	}

	private void OnPassQuests()
	{
		passButton.onClick.AddListener(delegate
		{
			Manager.Get<QuestManager>().PassCurrentQuests();
			Revalidate();
		});
	}

	public void ShowIfHidden()
	{
		Revalidate();
		if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Showing"))
		{
			GetComponent<Animator>().SetTrigger("Show");
		}
	}

	public void OnButtonClick()
	{
		Revalidate();
		if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Showing"))
		{
			GetComponent<Animator>().SetTrigger("Hide");
		}
		else
		{
			GetComponent<Animator>().SetTrigger("Show");
		}
	}
}
