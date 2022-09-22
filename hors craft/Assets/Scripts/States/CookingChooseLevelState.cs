// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingChooseLevelState
using Common.Managers;
using Common.Managers.States;
using Cooking;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace States
{
	public class CookingChooseLevelState : XCraftUIState<CookingChooseLevelStateConnector>
	{
		public bool useAdditionalIcons = true;

		public bool useDefaultIcons = true;

		public GameObject additionalButtonIcon;

		public GameObject[] iconsReplacement;

		public GameObject levelPrefab;

		public GameObject stagePrefab;

		private WorkController workController;

		private CookingLevel choosenLevel;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			workController = Manager.Get<CookingManager>().workController;
			Manager.Get<CookingManager>().ShowTopBar();
			CookingModule cookingSettings = Manager.Get<ModelManager>().cookingSettings;
			InitConnectorCooking();
			int num = 1;
			for (int i = 1; i <= cookingSettings.GetNumberOfChapters(); i++)
			{
				GameObject gameObject = Object.Instantiate(stagePrefab, base.connector.ChaptersList.transform);
				gameObject.transform.localScale = Vector3.one;
				CookingChapter component = gameObject.GetComponent<CookingChapter>();
				component.SetName(i);
				int num2 = 0;
				while (num2 < cookingSettings.GetNumberOfLevelsForChapter(i))
				{
					GameObject gameObject2 = Object.Instantiate(levelPrefab, component.levelsHolder.transform);
					gameObject2.transform.localScale = Vector3.one;
					CookingLevel levelScript = gameObject2.GetComponent<CookingLevel>();
					levelScript.SetLevel(num, num2);
					levelScript.Unlock(workController.workData.IsLevelUnlocked(num));
					levelScript.SetStars(workController.workData.GetLevelStars(num));
					levelScript.button.onClick.AddListener(delegate
					{
						OnLevel(levelScript);
					});
					if (workController.workData.IsLevelUnlocked(num))
					{
						choosenLevel = levelScript;
					}
					num2++;
					num++;
				}
			}
			choosenLevel.Choose(choose: true);
			if (additionalButtonIcon == null)
			{
				useAdditionalIcons = false;
			}
			else
			{
				GameObject[] additionalButtonIcons = base.connector.additionalButtonIcons;
				foreach (GameObject gameObject3 in additionalButtonIcons)
				{
					GameObject gameObject4 = Object.Instantiate(additionalButtonIcon);
					gameObject4.transform.SetParent(gameObject3.transform, worldPositionStays: false);
					gameObject4.transform.localScale = Vector3.one;
				}
			}
			if (iconsReplacement != null && iconsReplacement.Length == 3 && base.connector.buttonIcons.Length == 3)
			{
				for (int k = 0; k < 3 && iconsReplacement[k] != null && base.connector.buttonIcons[k] != null; k++)
				{
					GameObject gameObject5 = Object.Instantiate(iconsReplacement[k]);
					gameObject5.transform.SetParent(base.connector.buttonIcons[k].transform.parent, worldPositionStays: false);
					gameObject5.transform.localScale = Vector3.one;
					gameObject5.transform.SetAsFirstSibling();
				}
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			InitConnectorCooking();
			Manager.Get<CookingManager>().ShowTopBar();
		}

		private void InitConnectorCooking()
		{
			base.connector.onStartButton = OnStart;
			base.connector.onRankingButton = OnRankings;
			base.connector.onUpgradesButton = OnUpgrades;
			base.connector.onLeaveButton = OnLeave;
			base.connector.leaveButton.gameObject.SetActive(Manager.Get<ModelManager>().cookingSettings.IsLeaveButtonEnabled());
			GameObject[] buttonIcons = base.connector.buttonIcons;
			foreach (GameObject gameObject in buttonIcons)
			{
				gameObject.SetActive(useDefaultIcons);
			}
		}

		private void OnLevel(CookingLevel level)
		{
			if (choosenLevel.levelNumber == level.levelNumber)
			{
				OnStart();
				return;
			}
			choosenLevel.Choose(choose: false);
			choosenLevel = level;
			choosenLevel.Choose(choose: true);
		}

		private void OnStart()
		{
			workController.choosenLevel = choosenLevel;
			Manager.Get<StateMachineManager>().SetState<CookingBeforeStageState>();
		}

		private void OnRankings()
		{
			Manager.Get<StateMachineManager>().PushState<RankingsState>();
		}

		private void OnUpgrades()
		{
			Manager.Get<StateMachineManager>().PushState<CookingUpgradesState>();
		}

		private void OnLeave()
		{
			Manager.Get<CookingManager>().HideTopBar();
			Manager.Get<CookingManager>().prestigeAcquired = workController.workData.prestigeLevel;
			SceneManager.UnloadScene(workController.gameObject.scene);
			Manager.Get<StateMachineManager>().SetState<LoadLevelState>();
		}
	}
}
