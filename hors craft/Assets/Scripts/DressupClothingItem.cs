// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupClothingItem
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class DressupClothingItem : MonoBehaviour
{
	public delegate void Clicked(DressupClothingItem item);

	public DressupSkin dressupSkin;

	public Skin normalSkin;

	public Text coinPrice;

	public Text glamourRequired;

	public Image itemImage;

	public GameObject glamourLock;

	public GameObject buyItImage;

	public GameObject lockedImage;

	public GameObject boughtImage;

	private bool unlocked;

	private int index;

	private BodyPart part;

	public static event Clicked OnClicked;

	private void OnEnable()
	{
		OnClicked += ClickEvent;
	}

	private void OnDisable()
	{
		OnClicked -= ClickEvent;
	}

	public void SetValues(bool unlocked, int index, BodyPart part = BodyPart.Head)
	{
		this.unlocked = unlocked;
		this.index = index;
		this.part = part;
		GetComponent<Button>().onClick.AddListener(delegate
		{
			OnClick(this.unlocked, this.index, this.part);
		});
	}

	public void LockItem(int price, int glamourRequired = 0, bool removeGlamour = false)
	{
		lockedImage.SetActive(value: true);
		buyItImage.SetActive(value: false);
		if (removeGlamour)
		{
			glamourLock.SetActive(value: false);
		}
		else if (glamourRequired > Manager.Get<ProgressManager>().level)
		{
			glamourLock.SetActive(value: true);
		}
		else
		{
			glamourLock.SetActive(value: false);
		}
	}

	public void ClickEvent(DressupClothingItem sender)
	{
		if (sender != this)
		{
			buyItImage.SetActive(value: false);
			boughtImage.SetActive(value: false);
		}
	}

	public void UnlockItem()
	{
		lockedImage.SetActive(value: false);
		glamourLock.SetActive(value: false);
		buyItImage.SetActive(value: false);
		unlocked = true;
	}

	public void Select()
	{
		buyItImage.SetActive(!unlocked);
		DressupState dressupState = Manager.Get<StateMachineManager>().currentState as DressupState;
		if (dressupSkin.modelPrefab != null)
		{
			dressupState.ShowSelectedOnModel(index, dressupSkin);
		}
		if (normalSkin.texture != null)
		{
			dressupState.ShowSelectedOnModel(index, null, normalSkin, part);
		}
		boughtImage.SetActive(value: true);
	}

	public void OnClick(bool unlocked, int index, BodyPart part = BodyPart.Head)
	{
		this.index = index;
		if (buyItImage.activeSelf && !unlocked)
		{
			DressupState dressupState = Manager.Get<StateMachineManager>().currentState as DressupState;
			if (dressupState.TryToBuyItem(index, dressupSkin, normalSkin, part))
			{
				UnlockItem();
			}
		}
		if (!glamourLock.activeSelf)
		{
			Select();
			DressupClothingItem.OnClicked(this);
		}
	}
}
