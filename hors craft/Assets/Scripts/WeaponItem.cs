// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponItem
using Common.Managers;
using States;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
	private enum ItemState
	{
		LOCKED,
		UNLOCKED,
		BOUGHT,
		FULLAMMO
	}

	[SerializeField]
	private ItemState _itemState;

	[Space]
	public Button itemUnlockButton;

	public Button itemRefillButton;

	public GameObject itemUnlockedPanel;

	public GameObject itemLockedPanel;

	public GameObject itemUnlockedButtonPanel;

	public Image itemIcon;

	public Image itemLockedIcon;

	public Image itemAmmoIcon;

	public TranslateText itemName;

	public TranslateText itemLockedName;

	public TranslateText itemLevelRequirementText;

	public Text itemAmmoAmount;

	private SurvivalRankManager _survivalRankManager;

	private WeaponsContext _weaponsContext;

	private int _upgradeIndex;

	private WeaponConfig _weaponItem;

	private RankFragment _parentFragment;

	private int _ammoConfigIndex;

	private void Awake()
	{
		if (!Application.isPlaying)
		{
			_survivalRankManager = UnityEngine.Object.FindObjectOfType<SurvivalRankManager>();
		}
		else
		{
			_survivalRankManager = Manager.Get<SurvivalRankManager>();
		}
		itemUnlockButton.onClick.AddListener(BuyUpgrade);
		itemRefillButton.onClick.AddListener(BuyUpgrade);
		_parentFragment = GetComponentInParent<RankFragment>();
	}

	public void Init(int index, WeaponConfig weaponConfig, bool fullAmmo = false)
	{
		UpdateItem(weaponConfig, fullAmmo);
		_upgradeIndex = index;
		itemName.defaultText = _weaponItem.name;
		itemName.translationKey = _weaponItem.nameTranslationKey;
		itemName.ForceRefresh();
		itemLockedName.defaultText = _weaponItem.name;
		itemLockedName.translationKey = _weaponItem.nameTranslationKey;
		itemLockedName.ForceRefresh();
		itemIcon.sprite = _weaponItem.icon;
		itemLockedIcon.sprite = _weaponItem.icon;
		itemAmmoIcon.sprite = weaponConfig.ammoType.icon;
		itemLevelRequirementText.AddVisitor((string s) => s.Replace("{0}", _weaponItem.unlockAtLevel.ToString()));
		_parentFragment = GetComponentInParent<RankFragment>();
	}

	public void UpdateItem(WeaponConfig weaponConfig, bool fullAmmo = false)
	{
		_weaponItem = weaponConfig;
		AmmoContext context = SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>();
		if (_weaponsContext == null)
		{
			_weaponsContext = SurvivalContextsBroadcaster.instance.GetContext<WeaponsContext>();
		}
		int ammoIndex = context.GetAmmoIndex(_weaponItem.ammoType);
		int num = context.currentAmmo[ammoIndex];
		int num2 = context.maxAmmo[ammoIndex];
		if (num2 < 0)
		{
			itemAmmoAmount.text = "{0}".Formatted(num);
		}
		else
		{
			itemAmmoAmount.text = "{0}/{1}".Formatted(num, num2);
		}
		if (_weaponItem.claimed)
		{
			if (SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>().IsAmmoFull(_weaponItem.ammoType))
			{
				_itemState = ItemState.FULLAMMO;
			}
			else
			{
				_itemState = ItemState.BOUGHT;
			}
		}
		else if (_survivalRankManager.currentRankIndex >= _weaponItem.unlockAtLevel)
		{
			_itemState = ItemState.UNLOCKED;
		}
		else
		{
			_itemState = ItemState.LOCKED;
		}
		UpdateItemState();
	}

	private void BuyUpgrade()
	{
		if (_itemState == ItemState.BOUGHT)
		{
			_parentFragment.BuyItem("Do you want to watch an ad to refill ammo?", "survival.popup.refillammo", "Refill ammo", "survival.button.refillammo", _upgradeIndex);
		}
		else
		{
			_parentFragment.BuyItem("Do you want to watch an ad to unlock this weapon?", "survival.popup.unlock", "Unlock", "survival.button.unlock", _upgradeIndex);
		}
	}

	private void UpdateItemState()
	{
		switch (_itemState)
		{
		case ItemState.LOCKED:
			itemUnlockedPanel.SetActive(value: false);
			itemLockedPanel.SetActive(value: true);
			break;
		case ItemState.UNLOCKED:
			itemUnlockedPanel.SetActive(value: true);
			itemLockedPanel.SetActive(value: false);
			itemUnlockButton.gameObject.SetActive(value: true);
			itemRefillButton.gameObject.SetActive(value: false);
			itemUnlockedButtonPanel.SetActive(value: false);
			break;
		case ItemState.BOUGHT:
			itemUnlockedPanel.SetActive(value: true);
			itemLockedPanel.SetActive(value: false);
			itemUnlockButton.gameObject.SetActive(value: false);
			itemRefillButton.gameObject.SetActive(value: true);
			itemUnlockedButtonPanel.SetActive(value: false);
			break;
		case ItemState.FULLAMMO:
			itemUnlockedPanel.SetActive(value: true);
			itemLockedPanel.SetActive(value: false);
			itemUnlockButton.gameObject.SetActive(value: false);
			itemRefillButton.gameObject.SetActive(value: false);
			itemUnlockedButtonPanel.SetActive(value: true);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
