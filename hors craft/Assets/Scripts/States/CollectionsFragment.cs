// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CollectionsFragment
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CollectionsFragment : Fragment
	{
		public GameObject scrollListContentView;

		public GameObject fishPrefab;

		public Text legendaryCountText;

		public Text epicCountText;

		public Text rareCountText;

		public Text commonCountText;

		public Text pointsCountText;

		public Button rankingButton;

		public Button achievementsButton;

		private Fish[] fishList;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			InitFishCollection();
			ScrollToLatestFish();
			rankingButton.onClick.AddListener(delegate
			{
				ShowRanking();
			});
			achievementsButton.onClick.AddListener(delegate
			{
				ShowAchievements();
			});
			achievementsButton.gameObject.SetActive(value: false);
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			UpdateCollection();
		}

		private void InitFishCollection()
		{
			fishList = Manager.Get<FishingManager>().fishesConfig;
			Fish[] array = fishList;
			foreach (Fish fish in array)
			{
				SpawnFishPrefab(fish);
			}
			UpdateFishCount();
		}

		private void UpdateFishCount()
		{
			int[] fishCount = Manager.Get<FishingManager>().fishCount;
			int[] fishUnlockedCount = Manager.Get<FishingManager>().fishUnlockedCount;
			legendaryCountText.text = fishUnlockedCount[4].ToString() + "/" + fishCount[4].ToString();
			epicCountText.text = fishUnlockedCount[3].ToString() + "/" + fishCount[3].ToString();
			rareCountText.text = fishUnlockedCount[2].ToString() + "/" + fishCount[2].ToString();
			commonCountText.text = fishUnlockedCount[1].ToString() + "/" + fishCount[1].ToString();
			pointsCountText.text = Manager.Get<FishingManager>().GetCollectionPoints().ToString();
		}

		private void SpawnFishPrefab(Fish fish)
		{
			GameObject gameObject = Object.Instantiate(fishPrefab);
			gameObject.transform.SetParent(scrollListContentView.transform, worldPositionStays: false);
			gameObject.GetComponent<FishCollectionPrefab>().SetUp(fish);
		}

		private void UpdateCollection()
		{
			for (int i = 0; i < fishList.Length; i++)
			{
				FishCollectionPrefab component = scrollListContentView.transform.GetChild(i).GetComponent<FishCollectionPrefab>();
				component.SetUp(fishList[i]);
			}
			UpdateFishCount();
		}

		private void ScrollToLatestFish()
		{
			float num = fishList.Length;
			float num2 = Manager.Get<FishingManager>().randomFish;
			float num3 = num2 / num;
			if (num3 == 0f)
			{
				for (int i = 1; i < fishList.Length; i++)
				{
					if (fishList[i].catched)
					{
						num3 = (float)i / num;
						break;
					}
				}
			}
			scrollListContentView.GetComponentInParent<ScrollRect>().horizontalNormalizedPosition = num3;
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
		}

		private void ShowAchievements()
		{
		}
	}
}
