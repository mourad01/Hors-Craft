// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RankFragment
using Common.Managers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class RankFragment : Fragment
	{
		public GameObject itemsContentPanel;

		public GameObject itemPrefab;

		public Text levelText;

		public Text rankText;

		public Text stat1Text;

		public Text stat2Text;

		public Text stat3Text;

		public Slider experienceSlider;

		public Button rankingButton;

		public Button achievementsButton;

		public Button watchAdButton;

		[Space]
		public TopNotification unlockedNotificationPrefab;

		private TopNotification _unlockedInstance;

		private Text _coinsAnimatorText;

		private SurvivalManager _survivalManager;

		private WeaponsContext _weaponsContext;

		private SurvivalRankManager _survivalRankManager;

		private WeaponConfig[] _weaponsConfigs;

		private AmmoContextGenerator _ammoContextGenerator;

		private TopNotification unlockedInstance
		{
			get
			{
				if (_unlockedInstance != null || unlockedNotificationPrefab == null)
				{
					return _unlockedInstance;
				}
				_unlockedInstance = Object.Instantiate(unlockedNotificationPrefab.gameObject).GetComponentInChildren<TopNotification>();
				_unlockedInstance.GetComponentInChildren<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
				return _unlockedInstance;
			}
		}

		private AmmoContextGenerator ammoContextGenerator
		{
			get
			{
				if (_ammoContextGenerator == null)
				{
					_ammoContextGenerator = Manager.Get<SurvivalManager>().gameObject.GetComponent<AmmoContextGenerator>();
				}
				return _ammoContextGenerator;
			}
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			_survivalRankManager = Manager.Get<SurvivalRankManager>();
			_survivalManager = Manager.Get<SurvivalManager>();
			_weaponsContext = SurvivalContextsBroadcaster.instance.GetContext<WeaponsContext>();
			InitUpgrades();
			rankingButton.onClick.AddListener(ShowRanking);
			watchAdButton.onClick.AddListener(WatchAd);
			achievementsButton.transform.parent.gameObject.SetActive(value: false);
			achievementsButton.onClick.AddListener(ShowAchievements);
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			UpdateCollection();
		}

		private void InitUpgrades()
		{
			_weaponsConfigs = _weaponsContext.weaponsConfigs;
			for (int i = 0; i < _weaponsConfigs.Length; i++)
			{
				SpawnItemPrefabs(i, _weaponsConfigs[i]);
			}
			UpdateStats();
		}

		private void UpdateStats()
		{
			stat1Text.text = SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().day.ToString();
			stat2Text.text = MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor("EnemyKill").ToString();
			stat3Text.text = _survivalManager.shootsFired.ToString();
			levelText.text = _survivalRankManager.currentRankIndex.ToString();
			rankText.text = _survivalRankManager.rankName;
			experienceSlider.minValue = _survivalRankManager.currentRankMinPoints;
			experienceSlider.maxValue = _survivalRankManager.pointsToNextRank;
			experienceSlider.value = _survivalRankManager.currentPoints;
		}

		private void SpawnItemPrefabs(int index, WeaponConfig weaponConfig)
		{
			GameObject gameObject = Object.Instantiate(itemPrefab);
			gameObject.transform.SetParent(itemsContentPanel.transform, worldPositionStays: false);
			gameObject.GetComponent<WeaponItem>().Init(index, weaponConfig);
		}

		private void UpdateCollection()
		{
			for (int i = 0; i < _weaponsConfigs.Length; i++)
			{
				WeaponItem component = itemsContentPanel.transform.GetChild(i).GetComponent<WeaponItem>();
				component.UpdateItem(_weaponsConfigs[i]);
			}
			UpdateStats();
		}

		public void BuyItem(string defaultText, string translateText, string defaultTextButton, string translateTextButton, int itemIndex = -1)
		{
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				voxelSpritesToUnlock = null,
				type = AdsCounters.Upgrades,
				reason = StatsManager.AdReason.XSURVIVAL_AMMO,
				description = defaultText,
				translationKey = translateText,
				numberOfAdsNeeded = 1,
				onSuccess = delegate
				{
					if (itemIndex >= 0)
					{
						if (_weaponsContext.weaponsConfigs[itemIndex].claimed)
						{
							ammoContextGenerator.AddAmmo(_weaponsContext.weaponsConfigs[itemIndex].ammoType);
						}
						else
						{
							_weaponsContext.weaponsConfigs[itemIndex].claimed = true;
							ammoContextGenerator.AddAmmo(_weaponsContext.weaponsConfigs[itemIndex].ammoType, 30);
						}
						UpdateCollection();
					}
				},
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.defaultText = defaultTextButton;
					componentInChildren.translationKey = translateTextButton;
					componentInChildren.ForceRefresh();
				}
			});
		}

		private void HideNotificationImmediately()
		{
			if (unlockedInstance != null)
			{
				unlockedInstance.HideImmediately();
			}
		}

		public override void Destroy()
		{
			HideNotificationImmediately();
		}

		private void ShowRanking()
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(RankingsState)))
			{
				Manager.Get<StateMachineManager>().PushState(typeof(RankingsState));
			}
		}

		private void WatchAd()
		{
		}

		private void ShowAchievements()
		{
		}
	}
}
