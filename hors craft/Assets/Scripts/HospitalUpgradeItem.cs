// DecompilerFi decompiler from Assembly-CSharp.dll class: HospitalUpgradeItem
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class HospitalUpgradeItem : MonoBehaviour
{
	private enum ItemState
	{
		LOCKED,
		UNLOCKED,
		BOUGHT,
		UPGRADEABLE
	}

	[SerializeField]
	private ItemState itemState;

	[Space]
	public Button itemUnlockButton;

	public Button itemUpgradeButton;

	public GameObject itemUnlockedPanel;

	public GameObject itemLockedPanel;

	public GameObject itemUnlockedButtonPanel;

	public Image itemIcon;

	public Image itemLockedIcon;

	public TranslateText itemName;

	public TranslateText itemLevel;

	public TranslateText itemLockedName;

	public Text itemUnlockPriceText;

	public Text itemUpgradePriceText;

	public TranslateText itemPrestigeRequirementText;

	private ProgressManager progressManager;

	private HospitalManager hospitalManager;

	private int upgradeIndex;

	private HospitalManager.Upgrade itemUpgrade;

	private HospitalUpgradesFragment parentFragment;

	private void Awake()
	{
		if (!Application.isPlaying)
		{
			progressManager = UnityEngine.Object.FindObjectOfType<ProgressManager>();
			hospitalManager = UnityEngine.Object.FindObjectOfType<HospitalManager>();
		}
		else
		{
			progressManager = Manager.Get<ProgressManager>();
			hospitalManager = Manager.Get<HospitalManager>();
		}
		itemUnlockButton.onClick.AddListener(delegate
		{
			BuyUpgrade();
		});
		itemUpgradeButton.onClick.AddListener(delegate
		{
			BuyUpgrade();
		});
		parentFragment = GetComponentInParent<HospitalUpgradesFragment>();
	}

	public void Init(int index, HospitalManager.Upgrade upgrade, bool upgradeable = false)
	{
		UpdateItem(upgrade, upgradeable);
		upgradeIndex = index;
		itemName.defaultText = itemUpgrade.name;
		itemName.translationKey = itemUpgrade.nameTranslationKey;
		itemName.ForceRefresh();
		itemLockedName.translationKey = itemUpgrade.nameTranslationKey;
		itemLockedName.ForceRefresh();
		itemIcon.sprite = itemUpgrade.icon;
		itemLockedIcon.sprite = itemUpgrade.icon;
		itemPrestigeRequirementText.AddVisitor((string s) => s.Replace("{0}", itemUpgrade.prestigeRequirement.ToString()));
		itemLevel.AddVisitor((string s) => s.Replace("{0}", itemUpgrade.level.ToString()));
		parentFragment = GetComponentInParent<HospitalUpgradesFragment>();
	}

	private void BuyUpgrade()
	{
		if (upgradeIndex > 1 && hospitalManager.upgrades[upgradeIndex - 1].level == 0)
		{
			ShowGenericPopup("hospital.popup.unlock.prev.item", "You have to buy all previous items to buy that one!", showLeftButton: true, showRightButton: false);
		}
		else
		{
			Manager.Get<AbstractSoftCurrencyManager>().TryToBuySomething(itemUpgrade.price, delegate
			{
				if (itemUpgrade.level == 0)
				{
					parentFragment.ShowPatientUnlockedNotification(itemUpgrade.icon);
				}
				parentFragment.PlayCoinsAnimator(-itemUpgrade.price);
				progressManager.IncreaseExperience(itemUpgrade.prestigeToAddOnUnlock);
				hospitalManager.IncreaseUpgradeLevel(upgradeIndex);
				parentFragment.UpdateFragment();
			});
		}
	}

	private void ShowGenericPopup(string translationKey, string defaultText, bool showLeftButton = true, bool showRightButton = true)
	{
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureMessage = delegate(TranslateText t)
			{
				t.translationKey = translationKey;
				t.defaultText = defaultText;
			},
			configureLeftButton = delegate(Button b, TranslateText t)
			{
				t.translationKey = "pets.popup.button.back";
				t.defaultText = "back";
				b.gameObject.SetActive(showLeftButton);
				b.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
			},
			configureRightButton = delegate(Button b, TranslateText t)
			{
				t.translationKey = "pets.popup.button.ok";
				t.defaultText = "yes";
				b.gameObject.SetActive(showRightButton);
				b.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
			}
		});
	}

	public void UpdateItem(HospitalManager.Upgrade upgrade, bool upgradeable = false)
	{
		itemUpgrade = upgrade;
		itemUnlockPriceText.text = itemUpgrade.price.ToString();
		itemUpgradePriceText.text = itemUpgrade.price.ToString();
		if (itemUpgrade.level == 0)
		{
			if (progressManager.level >= itemUpgrade.prestigeRequirement)
			{
				itemState = ItemState.UNLOCKED;
			}
			else
			{
				itemState = ItemState.LOCKED;
			}
		}
		else if (itemUpgrade.level > 0)
		{
			if (upgradeable)
			{
				itemState = ItemState.UPGRADEABLE;
			}
			else
			{
				itemState = ItemState.BOUGHT;
			}
		}
		itemLevel.AddVisitor((string s) => s.Replace("{0}", (itemUpgrade.level * 10).ToString()));
		UpdateItemState();
	}

	private void UpdateItemState()
	{
		switch (itemState)
		{
		case ItemState.LOCKED:
			itemUnlockedPanel.SetActive(value: false);
			itemLockedPanel.SetActive(value: true);
			itemLevel.gameObject.SetActive(value: false);
			break;
		case ItemState.UNLOCKED:
			itemUnlockedPanel.SetActive(value: true);
			itemLevel.gameObject.SetActive(value: false);
			itemLockedPanel.SetActive(value: false);
			itemUnlockButton.gameObject.SetActive(value: true);
			itemUpgradeButton.gameObject.SetActive(value: false);
			itemUnlockedButtonPanel.SetActive(value: false);
			break;
		case ItemState.BOUGHT:
			itemUnlockedPanel.SetActive(value: true);
			itemLevel.gameObject.SetActive(value: false);
			itemLockedPanel.SetActive(value: false);
			itemUnlockButton.gameObject.SetActive(value: false);
			itemUpgradeButton.gameObject.SetActive(value: false);
			itemUnlockedButtonPanel.SetActive(value: true);
			break;
		case ItemState.UPGRADEABLE:
			itemUnlockedPanel.SetActive(value: true);
			itemLevel.gameObject.SetActive(value: true);
			itemLockedPanel.SetActive(value: false);
			itemUnlockButton.gameObject.SetActive(value: false);
			itemUpgradeButton.gameObject.SetActive(value: true);
			itemUnlockedButtonPanel.SetActive(value: false);
			break;
		}
	}
}
