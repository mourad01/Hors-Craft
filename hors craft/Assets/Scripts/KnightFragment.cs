// DecompilerFi decompiler from Assembly-CSharp.dll class: KnightFragment
using Common.Managers;
using Gameplay;
using GameUI;
using ItemVInventory;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightFragment : ProgressFragment
{
	[Serializable]
	public class EqConfig
	{
		public Image image;

		public Text shardsText;

		public Text eqLevelText;

		public Button upgradeButton;

		public GameObject levelUpArrow;

		public string id;
	}

	public PlayerStats weaponStats;

	public PlayerStats healthStats;

	public Text weaponStat;

	public Text healthStat;

	public EqConfig[] eqConfigs;

	public GameObject cameraParent;

	public Camera cameraPlayerPreview;

	public ModelDragRotator rotator;

	public Vector3 graphicOffset = new Vector3(0f, -0.25f, 3.4f);

	[Header("Others")]
	public GameObject updateWindow;

	public Text upgradeCost;

	public Button closeButton;

	public Button upgradeWindowUpgradeButton;

	public Text upgradeSwordStatsText;

	public Text upgradeShieldStatsText;

	public GameObject weaponStatUpgradeWindow;

	public GameObject shieldStatUpgradeWindow;

	public Text upgradeLevelText;

	private Backpack _backpack;

	private Equipment _equipment;

	private ItemsManager _itemsManager;

	private HumanRepresentation humanRep;

	protected Backpack backpack => _backpack ?? (_backpack = PlayerGraphic.GetControlledPlayerInstance().gameObject.GetComponentInChildren<Backpack>());

	protected Equipment equipment => _equipment ?? (_equipment = PlayerGraphic.GetControlledPlayerInstance().gameObject.GetComponentInChildren<Equipment>());

	protected ItemsManager itemsManager => _itemsManager ?? (_itemsManager = Manager.Get<ItemsManager>());

	protected AbstractSoftCurrencyManager softCurrency => Manager.Get<AbstractSoftCurrencyManager>();

	private string levelTranslationText => Manager.Get<TranslationsManager>().GetText("pause.knight.eq.level", "Lvl {0}");

	public override void Init(FragmentStartParameter parameter)
	{
		base.Init(parameter);
		UpdateStats();
		UpdateEq();
		InitUpgradeButtons();
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(delegate
		{
			updateWindow.SetActive(value: false);
		});
		InitHuman();
	}

	public void InitUpgradeButtons()
	{
		for (int i = 0; i < eqConfigs.Length; i++)
		{
			EqConfig config = eqConfigs[i];
			eqConfigs[i].upgradeButton.onClick.RemoveAllListeners();
			eqConfigs[i].upgradeButton.onClick.AddListener(delegate
			{
				UpgaradeClicked(config);
			});
		}
	}

	public override void Disable()
	{
		Destroy();
	}

	public override void Destroy()
	{
		cameraPlayerPreview.transform.SetParent(cameraParent.transform);
		if (humanRep != null)
		{
			humanRep.UIModeOff();
			humanRep = null;
		}
		base.Destroy();
	}

	public override void UpdateFragment()
	{
		base.UpdateFragment();
		InitHuman();
		UpdateStats();
		UpdateEq();
		UpdatePreview();
	}

	private void UpdatePreview()
	{
		if (humanRep != null)
		{
			cameraPlayerPreview.gameObject.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
		}
	}

	[ContextMenu("Refresh")]
	private void InitHuman()
	{
		if (humanRep == null)
		{
			humanRep = new HumanRepresentation(PlayerGraphic.GetControlledPlayerInstance());
			humanRep.UIModeOn(SetPlayerRepresentationPlace);
			humanRep.graphic.ShowBodyAndLegs();
			humanRep.graphic.ShowHands();
		}
	}

	private void SetPlayerRepresentationPlace(GameObject player)
	{
		cameraPlayerPreview.transform.SetParent(null);
		cameraPlayerPreview.transform.position = new Vector3(100f, 100f, 100f);
		player.transform.SetParent(cameraPlayerPreview.transform, worldPositionStays: false);
		player.transform.localPosition = Vector3.zero + graphicOffset;
		player.transform.localRotation = Quaternion.Euler(0f, 146f, 0f);
		rotator.modelToRotate = player;
	}

	private void UpdateStats()
	{
		weaponStat.text = weaponStats.GetStats().ToString("F0");
		healthStat.text = healthStats.GetStats().ToString("F0");
		upgradeSwordStatsText.text = weaponStat.text;
		upgradeShieldStatsText.text = healthStat.text;
	}

	private void UpdateEq()
	{
		for (int i = 0; i < eqConfigs.Length; i++)
		{
			ItemDefinition firstEquipment = equipment.GetFirstEquipment(eqConfigs[i].id);
			eqConfigs[i].image.sprite = firstEquipment.itemSprite;
			List<UpgradeRequirements> upgradeRequirements = firstEquipment.GetUpgradeRequirements();
			int itemCount = backpack.GetItemCount(upgradeRequirements[0].itemsIds[0]);
			int num = upgradeRequirements[0].itemsCount[0];
			bool flag = itemCount >= num;
			eqConfigs[i].shardsText.text = $"{itemCount}/{num}";
			eqConfigs[i].upgradeButton.interactable = flag;
			eqConfigs[i].levelUpArrow.SetActive(flag);
			eqConfigs[i].eqLevelText.text = string.Format(levelTranslationText, firstEquipment.level + 1);
		}
	}

	private void UpgaradeClicked(EqConfig config)
	{
		UpdateStats();
		updateWindow.SetActive(value: true);
		ItemDefinition item = equipment.GetFirstEquipment(config.id);
		int cost = itemsManager.GetUpgradeCurrencyRequirements(item);
		upgradeCost.text = cost.ToString();
		ItemsUpgradeStatsModule itemsUpgradeStatsSettings = Manager.Get<ModelManager>().itemsUpgradeStatsSettings;
		if (config.id.Equals("Sword"))
		{
			upgradeSwordStatsText.text = " + [" + itemsUpgradeStatsSettings.GetUpgradeStats(item.id, item.level + 1, item.level).ToString() + "]";
			shieldStatUpgradeWindow.SetActive(value: false);
			weaponStatUpgradeWindow.SetActive(value: true);
		}
		else
		{
			upgradeShieldStatsText.text = " + [" + itemsUpgradeStatsSettings.GetUpgradeStats(item.id, item.level + 1, item.level).ToString() + "]";
			shieldStatUpgradeWindow.SetActive(value: true);
			weaponStatUpgradeWindow.SetActive(value: false);
		}
		upgradeLevelText.text = string.Format(levelTranslationText, item.level + 2);
		upgradeWindowUpgradeButton.onClick.RemoveAllListeners();
		upgradeWindowUpgradeButton.onClick.AddListener(delegate
		{
			softCurrency.TryToBuySomething(cost, delegate
			{
				item.OnUpgrade(consumeRquired: true, backpack);
				updateWindow.SetActive(value: false);
				UpdateFragment();
			});
		});
	}
}
