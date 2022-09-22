// DecompilerFi decompiler from Assembly-CSharp.dll class: KnightTutorialGenerator
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using Gameplay;
using ItemVInventory;
using States;
using SurvivalMobSpawning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class KnightTutorialGenerator : MonoBehaviour
{
	private const int parallelSteps = 3;

	private GenericTutorial genericTutorial;

	private StringsContainer pathsContainer;

	private int _parallelStepsDone;

	private int parallelStepsDone
	{
		get
		{
			return _parallelStepsDone;
		}
		set
		{
			_parallelStepsDone = value;
			if (_parallelStepsDone == 3)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
				AbstractMobSpawner abstractMobSpawner = UnityEngine.Object.FindObjectOfType<AbstractMobSpawner>();
				AbstractMobSpawner abstractMobSpawner2 = abstractMobSpawner;
				abstractMobSpawner2.onStartSpawning = (Action)Delegate.Remove(abstractMobSpawner2.onStartSpawning, new Action(StartFirstWave));
				AbstractMobSpawner abstractMobSpawner3 = abstractMobSpawner;
				abstractMobSpawner3.onStopSpawning = (Action)Delegate.Remove(abstractMobSpawner3.onStopSpawning, new Action(EndFirstWave));
				Health component = CameraController.instance.Anchor.GetComponent<Health>();
				Health health = component;
				health.onHpChangeAction = (Health.DoOnHpChange)Delegate.Remove(health.onHpChangeAction, new Health.DoOnHpChange(HalfHealth));
				genericTutorial.NextStep();
				genericTutorial.EndTutorial();
				UnityEngine.Debug.LogError("End");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void Start()
	{
		genericTutorial = GetComponent<GenericTutorial>();
		pathsContainer = GetComponent<StringsContainer>();
		GameplayPopupsController gameplayPopupsController = UnityEngine.Object.FindObjectOfType<GameplayPopupsController>();
		if (gameplayPopupsController != null)
		{
			UnityEngine.Object.Destroy(gameplayPopupsController.gameObject);
		}
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
		InitTutorial();
		AbstractMobSpawner abstractMobSpawner = UnityEngine.Object.FindObjectOfType<AbstractMobSpawner>();
		AbstractMobSpawner abstractMobSpawner2 = abstractMobSpawner;
		abstractMobSpawner2.onStartSpawning = (Action)Delegate.Combine(abstractMobSpawner2.onStartSpawning, new Action(StartFirstWave));
		WaveSurvivedState waveSurvivedState = UnityEngine.Object.FindObjectOfType<WaveSurvivedState>();
		WaveSurvivedState waveSurvivedState2 = waveSurvivedState;
		waveSurvivedState2.onStart = (Action)Delegate.Combine(waveSurvivedState2.onStart, new Action(EndFirstWave));
	}

	private void InitTutorial()
	{
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		AddZerotStep();
		AddFirstStep();
		AddSecondStep();
		AddThirdStep();
		AddFourthStep();
		AddFifthStep();
		AddSixthStep();
		AddSeventhStep();
		AddEighthStep();
		AddNinthStep();
		genericTutorial.StartTutorial();
	}

	public void StartFirstWave()
	{
		AddTenthStep();
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep(0.5f);
		AbstractMobSpawner abstractMobSpawner = UnityEngine.Object.FindObjectOfType<AbstractMobSpawner>();
		AbstractMobSpawner abstractMobSpawner2 = abstractMobSpawner;
		abstractMobSpawner2.onStartSpawning = (Action)Delegate.Remove(abstractMobSpawner2.onStartSpawning, new Action(StartFirstWave));
	}

	public void EndFirstWave()
	{
		AddEleventhStep();
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep(0.5f);
		WaveSurvivedState waveSurvivedState = UnityEngine.Object.FindObjectOfType<WaveSurvivedState>();
		WaveSurvivedState waveSurvivedState2 = waveSurvivedState;
		waveSurvivedState2.onStart = (Action)Delegate.Remove(waveSurvivedState2.onStart, new Action(EndFirstWave));
	}

	public void HalfHealth()
	{
		Health component = CameraController.instance.Anchor.GetComponent<Health>();
		if (component.hp <= component.maxHp / 2f)
		{
			AddTwelfthStep();
			genericTutorial.autoChangeSteps = true;
			genericTutorial.allowInputThroughMask = false;
			genericTutorial.NextStep(0.5f);
			Health health = component;
			health.onHpChangeAction = (Health.DoOnHpChange)Delegate.Remove(health.onHpChangeAction, new Health.DoOnHpChange(HalfHealth));
		}
	}

	private void AddZerotStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.NextStep(2f);
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject p)
		{
			p.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private void StepZeroAction()
	{
		CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
		CameraController.instance.InputSource.IsEnabled = false;
		GameObject gameObject = (from d in UnityEngine.Object.FindObjectsOfType<TrainingDummy>()
			orderby Vector3.Distance(base.transform.position, d.transform.position)
			select d).First().gameObject;
		CameraController.instance.Anchor.LookAt(gameObject.transform);
		CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
		((LookAtTargetMotor)CameraController.instance.ActiveMotor).Target = gameObject.transform;
		TimeScaleHelper.value = 0f;
		Manager.Get<SurvivalManager>().TakeCombatSemaphore();
	}

	private void AddFirstStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.1";
		tutorialStep.infoDefaultText = "Witaj giermku";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
		{
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_INIT,
				action = delegate
				{
					StepOneAction();
				}
			}
		};
		GenericTutorial.TutorialStep step = tutorialStep;
		AddTutorialStepToArray(step);
	}

	private void StepOneAction()
	{
		CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
		CameraController.instance.InputSource.IsEnabled = false;
		GameObject gameObject = (from d in UnityEngine.Object.FindObjectsOfType<TrainingDummy>()
			orderby Vector3.Distance(base.transform.position, d.transform.position)
			select d).First().gameObject;
		gameObject.GetComponentInChildren<TrainingDummy>().hasToDrop = true;
		CameraController.instance.Anchor.LookAt(gameObject.transform);
		Vector3 eulerAngles = CameraController.instance.Anchor.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		CameraController.instance.Anchor.eulerAngles = eulerAngles;
		CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
		((LookAtTargetMotor)CameraController.instance.ActiveMotor).Target = gameObject.transform;
		TimeScaleHelper.value = 0f;
		Manager.Get<SurvivalManager>().TakeCombatSemaphore();
		genericTutorial.autoChangeSteps = true;
	}

	private void AddSecondStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.2";
		tutorialStep.infoDefaultText = "Teraz zaatakuj kukłę, tak jak cię uczyłem!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		GenericTutorial.TutorialStep step = tutorialStep;
		AddElementToClickOnSpawn(step, GetShootButton, AddShootingModuleCallbacks);
		AddTutorialStepToArray(step);
		AdWhiteWhenAttack();
	}

	private GameObject GetShootButton()
	{
		return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<ShootingModule>()
			.attackButton.gameObject;
		}

		private void AdWhiteWhenAttack()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.infoKey = "tutorial.text.2";
			tutorialStep.infoDefaultText = string.Empty;
			tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
			tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
					action = delegate
					{
						genericTutorial.autoChangeSteps = false;
						TimeScaleHelper.value = 1f;
						genericTutorial.NextStep(3f);
					}
				}
			};
			GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
			tutorialStep2.configInfoPanelAfterSpawn = delegate(GameObject g)
			{
				g.SetActive(value: false);
			};
			AddTutorialStepToArray(tutorialStep2);
		}

		private void AddThirdStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
			tutorialStep.infoKey = "tutorial.text.3";
			tutorialStep.infoDefaultText = "Świetna technika!Dobrze cię wyszkoliłem!";
			tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_STEP_INIT,
					action = delegate
					{
						StepThreeAction();
					}
				}
			};
			GenericTutorial.TutorialStep step = tutorialStep;
			AddTutorialStepToArray(step);
		}

		private void StepThreeAction()
		{
			TimeScaleHelper.value = 0f;
			genericTutorial.autoChangeSteps = true;
		}

		private void AddFourthStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.infoKey = "tutorial.text.4";
			tutorialStep.infoDefaultText = "Zdobyłeś nowe przedmioty! Przyjrzyjmy się im!";
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
			AddElementToClickOnSpawn(tutorialStep2, GetPauseButton);
			tutorialStep2.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
				action = delegate
				{
					Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
					{
						disableAds = true,
						categoryToOpen = "Stats"
					});
					genericTutorial.autoChangeSteps = false;
					genericTutorial.blockInputBetweenSteps = true;
					genericTutorial.allowInputThroughMask = true;
					StartCoroutine(WaitForIt(pathsContainer.GetValue("KnightStatsPanel")));
				}
			});
			AddTutorialStepToArray(tutorialStep2);
		}

		private IEnumerator WaitForIt(string path)
		{
			CanvasManager canvasManager = Manager.Get<CanvasManager>();
			Canvas canvas = canvasManager.canvas;
			PauseStateConnector pause;
			while (true)
			{
				PauseStateConnector componentInChildren;
				pause = (componentInChildren = canvas.GetComponentInChildren<PauseStateConnector>());
				if (!(componentInChildren == null))
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			Transform pauseTransform = pause.transform;
			while (true)
			{
				Transform childByPath = pauseTransform.GetChildByPath(path, clearFromClone: true);
				if (!(childByPath == null))
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			genericTutorial.autoChangeSteps = true;
			genericTutorial.allowInputThroughMask = false;
			genericTutorial.NextStep(0.1f);
		}

		private GameObject GetPauseButton()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<PauseModule>()
				.pauseButton.gameObject;
			}

			private void AddFifthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "tutorial.text.5";
				tutorialStep.infoDefaultText = "To twój rynsztunek: miecz i tarcza.";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				GenericTutorial.TutorialStep step = tutorialStep;
				AddElementToClickOnSpawn(step, GetKnightStatsPanel);
				AddTutorialStepToArray(step);
			}

			private GameObject GetKnightStatsPanel()
			{
				if (!(Manager.Get<StateMachineManager>().currentState is PauseState))
				{
					return null;
				}
				string value = pathsContainer.GetValue("KnightStatsPanel");
				return GetObjectFromPause(value);
			}

			private void AddSixthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "tutorial.text.6";
				tutorialStep.infoDefaultText = "Zdobyłeś wymaganą ilość odłamków. Możesz ulepszyć swój miecz.";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				GenericTutorial.TutorialStep step = tutorialStep;
				AddElementToClickOnSpawn(step, GetKnightUpgrade, AddButtonCallbacksWithINitProgressButtons);
				AddTutorialStepToArray(step);
			}

			private GameObject GetKnightUpgrade()
			{
				Equipment equipment = UnityEngine.Object.FindObjectOfType<Equipment>();
				Backpack backpack = UnityEngine.Object.FindObjectOfType<Backpack>();
				ItemDefinition firstEquipment = equipment.GetFirstEquipment("Sword");
				if (firstEquipment.CanBeUpgraded(backpack))
				{
					string value = pathsContainer.GetValue("SwordUpgradeButton");
					return GetObjectFromPause(value);
				}
				string value2 = pathsContainer.GetValue("ShieldUpgradeButton");
				return GetObjectFromPause(value2);
			}

			private void AddSeventhStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "tutorial.text.7";
				tutorialStep.infoDefaultText = string.Empty;
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
				AddElementToClickOnSpawn(tutorialStep2, GetKnightPopupUpgrade, AddDefaultButtonCallbacks);
				tutorialStep2.configInfoPanelAfterSpawn = delegate(GameObject g)
				{
					g.SetActive(value: false);
				};
				AddTutorialStepToArray(tutorialStep2);
			}

			private GameObject GetKnightPopupUpgrade()
			{
				return GetObjectFromPause(pathsContainer.GetValue("UpgradeButton"));
			}

			private void AddEighthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "tutorial.text.8";
				tutorialStep.infoDefaultText = "Zobacz twoje statystyki wzrosły!";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				GenericTutorial.TutorialStep step = tutorialStep;
				AddElementToClickOnSpawn(step, GetKnightStats);
				AddTutorialStepToArray(step);
			}

			private GameObject GetKnightStats()
			{
				return GetObjectFromPause(pathsContainer.GetValue("StatsPanel"));
			}

			private void AddNinthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "tutorial.text.9";
				tutorialStep.infoDefaultText = "Doskonale pojąłeś! Zbieraj monety i sprzęt aby rosnąć w siłę a może kiedyś mi dorównasz!";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
				GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
				AddElementToClickOnSpawn(tutorialStep2, GetMenuExitButton, AddDefaultButtonCallbacks);
				tutorialStep2.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
					action = delegate
					{
						genericTutorial.autoChangeSteps = false;
						genericTutorial.blockInputBetweenSteps = true;
						genericTutorial.allowInputThroughMask = true;
						CameraController.instance.InputSource.IsEnabled = true;
						Vector3 eulerAngles = CameraController.instance.Anchor.eulerAngles;
						eulerAngles.x = 0f;
						eulerAngles.z = 0f;
						CameraController.instance.Anchor.eulerAngles = eulerAngles;
						CameraController.instance.SetDefaultCameraPreset();
						CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
						Health component = CameraController.instance.Anchor.GetComponent<Health>();
						component.onHpChangeAction = (Health.DoOnHpChange)Delegate.Combine(component.onHpChangeAction, new Health.DoOnHpChange(HalfHealth));
						StartCoroutine(WaitForWeaponRelase());
						genericTutorial.NextStep(0.5f);
						TimeScaleHelper.value = 1f;
					}
				});
				AddTutorialStepToArray(tutorialStep2);
				AddWhiteStep();
			}

			private IEnumerator WaitForWeaponRelase()
			{
				yield return new WaitForSeconds(0.5f);
				ArmedPlayer.died = true;
				Manager.Get<SurvivalManager>().ReleaseCombatSemaphore();
				ArmedPlayer.died = false;
			}

			private GameObject GetMenuExitButton()
			{
				return GetObjectFromPause(pathsContainer.GetValue("PauseExit"));
			}

			private void AddTenthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
				tutorialStep.infoKey = "tutorial.text.10";
				tutorialStep.infoDefaultText = "Co się dzieje? Alarm? Ktoś nas atakuje!";
				tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
				tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
				{
					new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_STEP_INIT,
						action = delegate
						{
							TimeScaleHelper.value = 0f;
						}
					}
				};
				GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
				tutorialStep2.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
					action = delegate
					{
						EndParallel();
						genericTutorial.NextStep(0.5f);
					}
				});
				AddTutorialStepToArray(tutorialStep2);
				AddWhiteStep();
			}

			private void AddEleventhStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
				tutorialStep.infoKey = "tutorial.text.11";
				tutorialStep.infoDefaultText = "Doskonale Ci poszło!\nMożesz rywalizować z innymi graczami o miano najznamienitszego rycerza!\nSwoją pozycję w rankingu możesz sprawdzić w menu!";
				tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
				tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
				{
					new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_STEP_INIT,
						action = delegate
						{
							TimeScaleHelper.value = 0f;
						}
					}
				};
				GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
				tutorialStep2.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
					action = delegate
					{
						EndParallel();
						genericTutorial.NextStep(0.5f);
					}
				});
				AddTutorialStepToArray(tutorialStep2);
				AddWhiteStep();
			}

			private void AddTwelfthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "tutorial.text.12";
				tutorialStep.infoDefaultText = "Hej masz mało życia! Użyj potiona.";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
				{
					new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_STEP_INIT,
						action = delegate
						{
							TimeScaleHelper.value = 0f;
						}
					}
				};
				GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
				AddElementToClickOnSpawn(tutorialStep2, GetPotionButton, AddDefaultButtonCallbacks);
				tutorialStep2.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
					action = delegate
					{
						EndParallel();
						genericTutorial.NextStep(0.5f);
					}
				});
				AddTutorialStepToArray(tutorialStep2);
				AddWhiteStep();
			}

			private GameObject GetPotionButton()
			{
				return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<PotionsModule>()
					.potionsButton.gameObject;
				}

				private void AddWhiteStep()
				{
					GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
					tutorialStep.infoKey = "tutorial.text.9";
					tutorialStep.infoDefaultText = "Doskonale pojąłeś! Zbieraj monety i sprzęt aby rosnąć w siłę a może kiedyś mi dorównasz!";
					tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
					tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
					tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
					{
						new GenericTutorial.TutorialCallback
						{
							type = GenericTutorial.CallbackType.ON_STEP_INIT,
							action = delegate
							{
								TimeScaleHelper.value = 1f;
							}
						}
					};
					GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
					tutorialStep2.configInfoPanelAfterSpawn = delegate(GameObject g)
					{
						g.SetActive(value: false);
					};
					AddTutorialStepToArray(tutorialStep2);
				}

				private GameObject GetObjectFromPause(string path)
				{
					CanvasManager canvasManager = Manager.Get<CanvasManager>();
					Canvas canvas = canvasManager.canvas;
					PauseStateConnector componentInChildren = canvas.GetComponentInChildren<PauseStateConnector>();
					Transform transform = componentInChildren.transform;
					Transform childByPath = transform.GetChildByPath(path, clearFromClone: true);
					return childByPath.gameObject;
				}

				private GameObject GetGameObjectAtPath(string path, GameObject startPoint)
				{
					string[] array = path.Split('/');
					Transform transform = startPoint.transform;
					int num = 0;
					while (transform != null && num < array.Length)
					{
						Transform transform2 = transform.Find(array[num]);
						if (transform2 == null)
						{
							transform2 = transform.Find(array[num] + "(Clone)");
						}
						transform = transform2;
						num++;
					}
					if (transform == null)
					{
						UnityEngine.Debug.LogError("Couldn't find object " + array[num - 1] + " in path " + path);
					}
					return transform.gameObject;
				}

				private void AddTutorialStepToArray(GenericTutorial.TutorialStep step)
				{
					GenericTutorial.TutorialStep[] array = new GenericTutorial.TutorialStep[genericTutorial.tutorialSteps.Length + 1];
					Array.Copy(genericTutorial.tutorialSteps, array, array.Length - 1);
					array[array.Length - 1] = step;
					genericTutorial.tutorialSteps = array;
				}

				private void AddElementToClickOnSpawn(GenericTutorial.TutorialStep step, Func<GameObject> getGO, Action<GenericTutorial.TutorialStep, GameObject> addOtherInitialCallbacks = null)
				{
					if (step.tutorialCallbacks == null)
					{
						step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
					}
					step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_STEP_INIT,
						action = delegate
						{
							step.element = getGO();
							if (addOtherInitialCallbacks != null)
							{
								addOtherInitialCallbacks(step, step.element);
							}
						}
					});
				}

				private void AddDefaultButtonCallbacks(GenericTutorial.TutorialStep step, GameObject buttonGO)
				{
					if (step.tutorialCallbacks == null)
					{
						step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
					}
					step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
						action = delegate
						{
							Button componentInChildren = buttonGO.GetComponentInChildren<Button>();
							componentInChildren.onClick.Invoke();
						}
					});
				}

				private void AddButtonCallbacksWithINitProgressButtons(GenericTutorial.TutorialStep step, GameObject buttonGO)
				{
					if (step.tutorialCallbacks == null)
					{
						step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
					}
					step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
						action = delegate
						{
							UnityEngine.Object.FindObjectOfType<KnightFragment>().InitUpgradeButtons();
							Button componentInChildren = buttonGO.GetComponentInChildren<Button>();
							componentInChildren.onClick.Invoke();
						}
					});
				}

				private void AddShootingModuleCallbacks(GenericTutorial.TutorialStep step, GameObject buttonGO)
				{
					if (step.tutorialCallbacks == null)
					{
						step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
					}
					step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
						action = delegate
						{
							Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<ShootingModule>()
								.ForceButtonUpdate(arg: true);
						}
					});
				}

				private void EndParallel()
				{
					genericTutorial.autoChangeSteps = false;
					genericTutorial.blockInputBetweenSteps = true;
					genericTutorial.allowInputThroughMask = true;
					CameraController.instance.InputSource.IsEnabled = true;
					CameraController.instance.SetDefaultCameraPreset();
					CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
					parallelStepsDone++;
				}
			}
