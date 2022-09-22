// DecompilerFi decompiler from Assembly-CSharp.dll class: GiftCraftable
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class GiftCraftable : MonoBehaviour, ICustomCraftingItem
{
	public Skin.Gender gender;

	public Sprite sprite;

	public void OnCraftAction()
	{
	}

	public void OnUseAction(int id)
	{
		if (Manager.Get<LoveManager>().lovedOne == null)
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "love.not.found";
					t.defaultText = "You need to have a partner to give him gifts!";
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.gameObject.SetActive(value: false);
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "menu.ok";
					t.defaultText = "ok";
				}
			});
			return;
		}
		Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		Manager.Get<LoveManager>().GiveGift(this);
		Singleton<PlayerData>.get.playerItems.AddCraftable(id, -1);
	}
}
