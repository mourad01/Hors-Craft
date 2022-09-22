// DecompilerFi decompiler from Assembly-CSharp.dll class: States.HospitalUpgradesFragment
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class HospitalUpgradesFragment : ProgressFragment
	{
		private const string CURED_PATIENTS = "hospital.cured.patients";

		private const string CURED_STARS = "hospital.cured.stars";

		private const string HOSPITAL_PRESTIGE = "hospital.hospital.prestige";

		public GameObject upgradesContentPanel;

		public GameObject hospitalUpgradeItemPrefab;

		public Animator coinsAnimator;

		[Space]
		public TopNotification patientUnlockedNotificationPrefab;

		private TopNotification _patientUnlockedInstance;

		private Text coinsAnimatorText;

		private HospitalManager hospitalManager;

		private HospitalManager.Upgrade[] upgradesList;

		private bool allUpgradesUnlocked;

		private TopNotification patientUnlockedInstance
		{
			get
			{
				if (_patientUnlockedInstance == null && patientUnlockedNotificationPrefab != null)
				{
					_patientUnlockedInstance = Object.Instantiate(patientUnlockedNotificationPrefab.gameObject).GetComponentInChildren<TopNotification>();
					_patientUnlockedInstance.GetComponentInChildren<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
				}
				return _patientUnlockedInstance;
			}
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			hospitalManager = Manager.Get<HospitalManager>();
			coinsAnimatorText = coinsAnimator.GetComponentInChildren<Text>();
			InitUpgrades();
			rankingButton.gameObject.SetActive(value: true);
			rankingButton.onClick.AddListener(delegate
			{
				ShowRanking();
			});
			achievementsButton.transform.parent.gameObject.SetActive(value: false);
			achievementsButton.onClick.AddListener(delegate
			{
				ShowAchievements();
			});
			UnityEngine.Debug.Log($"Hospital: Trying to save score: {Singleton<GooglePlayConstants>.get.GetIDFor(hospitalManager.leaderboardPrestige)}: {Manager.Get<ProgressManager>().level}");
			Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(hospitalManager.leaderboardPrestige), Manager.Get<ProgressManager>().level);
			Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(hospitalManager.leaderboardCuredPatients), MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor("hospital.cured.patients"));
			Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(hospitalManager.leaderboardCuredSuperstars), MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor("hospital.cured.stars"));
			Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(hospitalManager.leaderboardHospitalPrestige), MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor("hospital.hospital.prestige"));
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			UpdateCollection();
		}

		public void PlayCoinsAnimator(int amount)
		{
			string text;
			if (amount > 0)
			{
				coinsAnimatorText.color = Color.green;
				text = "+{0}".Formatted(Mathf.Abs(amount));
			}
			else
			{
				coinsAnimatorText.color = Color.red;
				text = "-{0}".Formatted(Mathf.Abs(amount));
			}
			coinsAnimatorText.text = text;
			coinsAnimator.SetTrigger("Show");
		}

		private void InitUpgrades()
		{
			upgradesList = hospitalManager.upgrades;
			allUpgradesUnlocked = hospitalManager.AllUpgradesUnlocked();
			for (int i = 0; i < upgradesList.Length; i++)
			{
				SpawnUpgradePrefab(i, upgradesList[i], upgradesList[i].upgradeable && allUpgradesUnlocked);
			}
			UpdateHospitalStats();
		}

		private void UpdateHospitalStats()
		{
		}

		private void SpawnUpgradePrefab(int index, HospitalManager.Upgrade upgrade, bool upgradeable)
		{
			GameObject gameObject = Object.Instantiate(hospitalUpgradeItemPrefab);
			gameObject.transform.SetParent(upgradesContentPanel.transform, worldPositionStays: false);
			gameObject.GetComponent<HospitalUpgradeItem>().Init(index, upgrade, upgradeable);
		}

		private void UpdateCollection()
		{
			allUpgradesUnlocked = hospitalManager.AllUpgradesUnlocked();
			for (int i = 0; i < upgradesList.Length; i++)
			{
				HospitalUpgradeItem component = upgradesContentPanel.transform.GetChild(i).GetComponent<HospitalUpgradeItem>();
				component.UpdateItem(upgradesList[i], upgradesList[i].upgradeable && allUpgradesUnlocked);
			}
			UpdateHospitalStats();
		}

		private void HideNotificationImmediately()
		{
			if (patientUnlockedInstance != null)
			{
				patientUnlockedInstance.HideImmediately();
			}
		}

		public void ShowPatientUnlockedNotification(Sprite icon)
		{
			if (!(patientUnlockedInstance == null))
			{
				PatientUnlockedNotification.PatientUnlockedInformation patientUnlockedInformation = new PatientUnlockedNotification.PatientUnlockedInformation();
				patientUnlockedInformation.icon = icon;
				patientUnlockedInformation.setOnTop = true;
				patientUnlockedInformation.timeToHide = 1.5f;
				PatientUnlockedNotification.PatientUnlockedInformation information = patientUnlockedInformation;
				patientUnlockedInstance.Show(information);
				patientUnlockedInstance.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			}
		}

		public override void Destroy()
		{
			HideNotificationImmediately();
		}

		private void ShowRanking()
		{
			if (Manager.Contains<RankingManager>())
			{
				Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Ranking");
			}
			else
			{
				Manager.Get<SocialPlatformManager>().social.ShowRankings();
			}
			Manager.Get<StatsManager>().RankingClicked();
		}

		private void ShowAchievements()
		{
			Manager.Get<StatsManager>().AchievementsClicked();
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Achievements");
			}
			else
			{
				Social.ShowAchievementsUI();
			}
		}
	}
}
