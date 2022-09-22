// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestItemElement
using Common.Behaviours;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemElement : QuestUIElement
{
	[SerializeField]
	protected Text count;

	[SerializeField]
	protected Button devButtonPlus;

	[SerializeField]
	protected Button devButtonMinus;

	protected int itemId;

	public void SetText(string newText)
	{
		count.text = $"x{newText}";
	}

	public void SetItemId(int itemId)
	{
		this.itemId = itemId;
	}

	public void Start()
	{
		if (MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper)
		{
			devButtonPlus.gameObject.SetActive(value: true);
			devButtonMinus.gameObject.SetActive(value: true);
			devButtonPlus.onClick.AddListener(delegate
			{
				Singleton<PlayerData>.get.playerItems.AddToResources(itemId, 1);
			});
			devButtonMinus.onClick.AddListener(delegate
			{
				Singleton<PlayerData>.get.playerItems.AddToResources(itemId, -1);
			});
		}
	}
}
