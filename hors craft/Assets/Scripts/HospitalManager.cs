// DecompilerFi decompiler from Assembly-CSharp.dll class: HospitalManager
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class HospitalManager : Manager
{
	[Serializable]
	public class Upgrade
	{
		public string name;

		public string nameTranslationKey;

		public int level;

		public Sprite icon;

		public Sprite iconInGame;

		public GameObject itemForHandPrefab;

		public int basePrice;

		public int baseUpgradePrice;

		public bool upgradeable = true;

		public float upgradePriceMultiplier;

		public int prestigeRequirement;

		public int prestigeToAddOnUnlock;

		[NonSerialized]
		public Material icoMaterial;

		[NonSerialized]
		public Action<int> OnLevelChanged;

		public int price
		{
			get
			{
				if (level == 0)
				{
					return basePrice;
				}
				return Mathf.FloorToInt((float)(level * baseUpgradePrice) * upgradePriceMultiplier);
			}
		}
	}

	[Serializable]
	public class PatientConfig
	{
		public int id;

		public int goldValue;

		public int prestigeValue;

		public int upgradeIdNeededToHeal;

		public bool isSuperstar;

		[Range(1f, 5f)]
		public int minigameDificulty;

		public MiniGameExe miniGame;

		public PatientConfig(PatientConfig patientConfig)
		{
			id = patientConfig.id;
			goldValue = patientConfig.goldValue;
			prestigeValue = patientConfig.prestigeValue;
			upgradeIdNeededToHeal = patientConfig.upgradeIdNeededToHeal;
			minigameDificulty = patientConfig.minigameDificulty;
			isSuperstar = patientConfig.isSuperstar;
			miniGame = patientConfig.miniGame;
		}
	}

	public enum PatientsType
	{
		Human,
		Animal
	}

	private const string CURED_PATIENTS = "hospital.cured.patients";

	private const string CURED_STARS = "hospital.cured.stars";

	private const string HOSPITAL_PRESTIGE = "hospital.hospital.prestige";

	public Upgrade[] upgrades;

	public PatientConfig[] patients;

	public GameObject patientEquipmentIcon;

	public Vector3 patientEquipmentIconOffset = new Vector3(0f, 2.5f, 0f);

	[HideInInspector]
	public GameObject patientSuperstarIndicator;

	public PatientsType patientsType;

	private ModelManager _modelManager;

	[HideInInspector]
	[SerializeField]
	private List<int> availableDiseses = new List<int>();

	[HideInInspector]
	[SerializeField]
	private List<int> notAvailableDiseses = new List<int>();

	[HideInInspector]
	public int currentPatientUpgradeIndex = 1;

	[HideInInspector]
	public PatientConfig currentPatientConfig;

	[HideInInspector]
	public Patient currentPatientObject;

	public bool tameWhenHealed;

	public string leaderboardPrestige = "leaderboardPrestige";

	public string leaderboardCuredPatients = "leaderboardCuredPatients";

	public string leaderboardCuredSuperstars = "leaderboardCuredSuperstars";

	public string leaderboardHospitalPrestige = "leaderboardHospitalPrestige";

	public Shader icoShader;

	public HospitalVampireMode vampireMode = new HospitalVampireMode();

	public List<CurrencyScriptableObject> currencySlots = new List<CurrencyScriptableObject>();

	private string prestigeAdded;

	private string coinsAdded;

	public bool allUpgradesUnlocked
	{
		get;
		private set;
	}

	public float minigameAddingSpeed
	{
		get;
		private set;
	}

	public float minigameLosingSpeed
	{
		get;
		private set;
	}

	private bool hasShowedFullyUpgradedPopup => PlayerPrefs.GetInt("hospital.fully.upgraded.popup", 0) == 1;

	private ModelManager modelManager => _modelManager ?? (_modelManager = Manager.Get<ModelManager>());

	public HospitalTutorial hospitalTutorial
	{
		get;
		private set;
	}

	private void Awake()
	{
		for (int i = 0; i < upgrades.Length; i++)
		{
			upgrades[i].icoMaterial = new Material(icoShader);
			if (upgrades[i].iconInGame != null)
			{
				upgrades[i].icoMaterial.SetTexture("_MainTex", upgrades[i].iconInGame.texture);
			}
			else
			{
				upgrades[i].icoMaterial.SetTexture("_MainTex", upgrades[i].icon.texture);
			}
		}
		hospitalTutorial = GetComponent<HospitalTutorial>();
	}

	public override void Init()
	{
		Load();
		if (vampireMode.enabled)
		{
			vampireMode.Init(this);
		}
	}

	private void Update()
	{
	}

	public void IncreaseUpgradeLevel(int index)
	{
		Upgrade upgrade = upgrades[index];
		upgrade.level++;
		if (upgrade.OnLevelChanged != null)
		{
			upgrade.OnLevelChanged(upgrade.level);
		}
		if (upgrade.level == 1 && !availableDiseses.Contains(index))
		{
			availableDiseses.Add(index);
			notAvailableDiseses.Remove(index);
		}
		if (!hasShowedFullyUpgradedPopup && AllUpgradesUnlocked())
		{
			PlayerPrefs.SetInt("hospital.fully.upgraded.popup", 1);
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "hospital.popup.fully.upgraded";
					t.defaultText = "Congratulations! You’re well-equipped Doctor! Now, you can upgrade your kit and recive more prestiege and gold from yours patients!";
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "pets.popup.button.back";
					t.defaultText = "back";
					b.gameObject.SetActive(value: false);
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "pets.popup.button.ok";
					t.defaultText = "ok";
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
				}
			});
		}
		if (Manager.Contains<AbstractAchievementManager>())
		{
			Manager.Get<AbstractAchievementManager>().RegisterEvent("hospital.eq");
		}
	}

	public bool AllUpgradesUnlocked()
	{
		Upgrade[] array = upgrades;
		foreach (Upgrade upgrade in array)
		{
			if (upgrade.level == 0)
			{
				return false;
			}
		}
		allUpgradesUnlocked = true;
		return true;
	}

	public void SetPatient(Patient patient)
	{
		currentPatientObject = patient;
		currentPatientConfig = patient.patientConfig;
		if (currentPatientConfig.isSuperstar)
		{
			currentPatientConfig.minigameDificulty = 5;
		}
		Pettable pettable = UnityEngine.Object.FindObjectOfType<CameraEventsSender>().inFrontObject.rigidbody.GetComponentInChildren<Pettable>();
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
		{
			currentPatientConfig.miniGame.Run(delegate
			{
				PatientCured();
				TameWhenHealed(pettable);
			}, PatientNotCured, currentPatientConfig.minigameDificulty);
		}
		else
		{
			currentPatientConfig.miniGame.Run(delegate
			{
				PatientCured();
				TameWhenHealed(pettable);
			}, null, currentPatientConfig.minigameDificulty, useFinish: false);
		}
	}

	public void TameWhenHealed(Pettable pet)
	{
		if (tameWhenHealed && pet != null)
		{
			pet.OneCallToFullTame();
		}
	}

	public int GetDisese()
	{
		List<int> list = new List<int>();
		list.AddRange(availableDiseses);
		if (notAvailableDiseses.Count > 0)
		{
			list.Add(notAvailableDiseses[0]);
		}
		int num = list[UnityEngine.Random.Range(0, list.Count)];
		List<int> list2 = new List<int>();
		for (int i = 0; i < patients.Length; i++)
		{
			if (patients[i].upgradeIdNeededToHeal == num)
			{
				list2.Add(i);
			}
		}
		if (list2.Count == 0)
		{
			return 0;
		}
		return list2[UnityEngine.Random.Range(0, list2.Count)];
	}

	public void IncreaseHospitalPrestige()
	{
		MonoBehaviourSingleton<ProgressCounter>.get.Add("hospital.hospital.prestige", 1);
	}

	private void PatientCured()
	{
		MonoBehaviourSingleton<ProgressCounter>.get.Add("hospital.cured.patients", 1);
		if (currentPatientConfig.isSuperstar)
		{
			MonoBehaviourSingleton<ProgressCounter>.get.Add("hospital.cured.stars", 1);
		}
		if (vampireMode.enabled)
		{
			currentPatientObject.HealUp(isVampire: true);
			vampireMode.FearOtherMobs();
			vampireMode.Feed();
		}
		else
		{
			currentPatientObject.HealUp();
		}
		AddReward();
		Manager.Get<StateMachineManager>().SetState<PatientCuredPopupState>(new PatientCuredPopupStateStartParameter
		{
			prestigeEarnedValue = prestigeAdded,
			moneyEarnedValue = coinsAdded
		});
		if (Manager.Contains<AbstractAchievementManager>())
		{
			Manager.Get<AbstractAchievementManager>().RegisterEvent("healed");
		}
	}

	private void PatientNotCured()
	{
		currentPatientObject.HealthDrop(modelManager.hospitalSettings.GetPatientHealthDrop(), vampireMode.enabled);
		Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("hospital.toast.minigame.failed", "You failed. Patient lost health!"));
	}

	private void AddReward(bool rewarded = false)
	{
		float num = 1f + modelManager.hospitalSettings.GetItemUpgradeBonus() * (float)(upgrades[currentPatientConfig.upgradeIdNeededToHeal].level - 1);
		int num2;
		int num3;
		if (rewarded)
		{
			num2 = Mathf.FloorToInt((float)currentPatientConfig.prestigeValue * num);
			num3 = Mathf.FloorToInt((float)currentPatientConfig.goldValue * num);
		}
		else
		{
			num2 = Mathf.FloorToInt((float)currentPatientConfig.prestigeValue * num);
			num3 = Mathf.FloorToInt((float)currentPatientConfig.goldValue * num);
		}
		if (currentPatientConfig.isSuperstar)
		{
			num2 *= modelManager.hospitalSettings.GetSuperstarBonusMultiplier();
			num3 *= modelManager.hospitalSettings.GetSuperstarBonusMultiplier();
		}
		prestigeAdded = num2.ToString();
		coinsAdded = num3.ToString();
		Manager.Get<ProgressManager>().IncreaseExperience(num2);
		Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(Mathf.FloorToInt(num3));
	}

	public void WatchedRewardedAd()
	{
		AddReward(rewarded: true);
	}

	public void CleanUp()
	{
		currentPatientConfig = null;
	}

	public void LoadSettingsFromModel()
	{
		for (int i = 0; i < upgrades.Length; i++)
		{
			upgrades[i].basePrice = modelManager.hospitalSettings.GetItemBasePrice(i);
			upgrades[i].baseUpgradePrice = modelManager.hospitalSettings.GetItemBaseUpgradePrice(i);
			upgrades[i].prestigeRequirement = modelManager.hospitalSettings.GetItemPrestigeLevelRequirement(i);
			upgrades[i].prestigeToAddOnUnlock = modelManager.hospitalSettings.GetItemPrestigeOnUnlock(i);
			upgrades[i].upgradePriceMultiplier = modelManager.hospitalSettings.GetItemUpgradePriceMultiplier();
		}
		for (int j = 0; j < patients.Length; j++)
		{
			patients[j].goldValue = modelManager.hospitalSettings.GetPatientGoldValue(j);
			patients[j].prestigeValue = modelManager.hospitalSettings.GetPatientPrestigeValue(j);
			patients[j].upgradeIdNeededToHeal = modelManager.hospitalSettings.GetPatientItemNeededToHeal(j);
		}
		minigameAddingSpeed = modelManager.hospitalSettings.GetMiniGameAddingSpeed();
		minigameLosingSpeed = modelManager.hospitalSettings.GetMiniGameLosingSpeed();
	}

	private void Save()
	{
		for (int i = 0; i < upgrades.Length; i++)
		{
			PlayerPrefs.SetInt("hospital.upgrades." + i, upgrades[i].level);
		}
	}

	private void Load()
	{
		PrefsMover("hospital.coins", delegate(int value)
		{
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(value);
		});
		PrefsMover("hospital.cured.patients", delegate(int value)
		{
			MonoBehaviourSingleton<ProgressCounter>.get.Add("hospital.cured.patients", value);
		});
		PrefsMover("hospital.cured.stars", delegate(int value)
		{
			MonoBehaviourSingleton<ProgressCounter>.get.Add("hospital.cured.stars", value);
		});
		PrefsMover("hospital.hospital.prestige", delegate(int value)
		{
			MonoBehaviourSingleton<ProgressCounter>.get.Add("hospital.hospital.prestige", value);
		});
		upgrades[0].level = PlayerPrefs.GetInt("hospital.upgrades.0", 1);
		for (int i = 1; i < upgrades.Length; i++)
		{
			upgrades[i].level = PlayerPrefs.GetInt("hospital.upgrades." + i, 0);
		}
		for (int j = 0; j < upgrades.Length; j++)
		{
			if (upgrades[j].level > 0)
			{
				availableDiseses.Add(j);
			}
			else
			{
				notAvailableDiseses.Add(j);
			}
		}
	}

	private void PrefsMover(string key, Action<int> func)
	{
		if (PlayerPrefs.HasKey(key))
		{
			func(PlayerPrefs.GetInt(key, 0));
			PlayerPrefs.DeleteKey(key);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Save();
		}
	}

	private void OnApplicationQuit()
	{
		Save();
	}
}
