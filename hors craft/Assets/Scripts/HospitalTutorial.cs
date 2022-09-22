// DecompilerFi decompiler from Assembly-CSharp.dll class: HospitalTutorial
using com.ootii.Cameras;
using Common.Managers;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class HospitalTutorial : MonoBehaviour
{
	public float npcOffset = 2.5f;

	public Vector3 cameraOffset = Vector3.zero;

	private GenericTutorial genericTutorial;

	private const string TUTORIAL_FINISHED = "hospital.manager.tutorial.finished";

	private HospitalManager hospitalManager;

	private GameObject npc;

	private void Awake()
	{
		genericTutorial = GetComponent<GenericTutorial>();
		hospitalManager = Manager.Get<HospitalManager>();
		GameplayPopupsController gameplayPopupsController = UnityEngine.Object.FindObjectOfType<GameplayPopupsController>();
		if (gameplayPopupsController != null)
		{
			UnityEngine.Object.Destroy(gameplayPopupsController.gameObject);
		}
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
		InitTutorial();
	}

	private void InitTutorial()
	{
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		AddZeroStep();
		AddFirstStep();
		AddSecondStep();
		AddThirdStep();
		AddFourthStep();
		AddFifthStep();
		AddSixthStep();
		AddSeventhStep();
		AddEighthStep();
		AddNinthStep();
		AddTenthStep();
		genericTutorial.StartTutorial();
	}

	private void AddZeroStep()
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
				StepZeroAction();
			}
		});
		AddTutorialStepToArray(tutorialStep);
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject p)
		{
			p.SetActive(value: false);
		};
	}

	private void StepZeroAction()
	{
		genericTutorial.autoChangeSteps = false;
		genericTutorial.NextStep(3f);
	}

	private void SpawnPatient()
	{
		MobsManager mobsManager = Manager.Get<MobsManager>();
		npc = UnityEngine.Object.Instantiate(mobsManager.spawnConfigs[UnityEngine.Random.Range(0, mobsManager.spawnConfigs.Length)].prefab);
		npc.GetComponent<Pettable>().TryBePatient();
		npc.transform.position = CameraController.instance.MainCamera.transform.position + CameraController.instance.MainCamera.transform.forward * npcOffset + Vector3.down;
		npc.transform.LookAt(CameraController.instance.MainCamera.transform);
		Transform transform = npc.transform;
		Vector3 localEulerAngles = npc.transform.localEulerAngles;
		float y = localEulerAngles.y;
		Vector3 localEulerAngles2 = npc.transform.localEulerAngles;
		transform.localEulerAngles = new Vector3(0f, y, localEulerAngles2.z);
		npc.GetComponent<AnimalMob>().logic = AnimalMob.AnimalLogic.STAY_IN_PLACE;
	}

	private void AddFirstStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "hospital.tutorial.0";
		tutorialStep.infoDefaultText = "Thank goodness you're here! We have an epidemic here!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
		{
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_INIT,
				action = delegate
				{
					SpawnPatient();
					Time.timeScale = 0f;
					genericTutorial.autoChangeSteps = true;
					CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
					CameraController.instance.InputSource.IsEnabled = false;
				}
			}
		};
		GenericTutorial.TutorialStep step = tutorialStep;
		AddTutorialStepToArray(step);
	}

	private void AddSecondStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "hospital.tutorial.1";
		tutorialStep.infoDefaultText = "No time to waste! We need to help this person!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
		{
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_INIT,
				action = delegate
				{
					StepTwoAction();
				}
			}
		};
		GenericTutorial.TutorialStep step = tutorialStep;
		AddElementToClickOnSpawn(step, () => npc);
		AddTutorialStepToArray(step);
	}

	private void StepTwoAction()
	{
		if (npc == null)
		{
			SpawnPatient();
		}
		CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
		((LookAtTargetMotor)CameraController.instance.ActiveMotor).Target = npc.transform;
		((LookAtTargetMotor)CameraController.instance.ActiveMotor).TargetOffset = cameraOffset;
		RaycastHit[] collection = Physics.RaycastAll(CameraController.instance.MainCamera.transform.position, CameraController.instance.MainCamera.transform.forward, Vector3.Distance(CameraController.instance.MainCamera.transform.position, npc.transform.position));
		List<RaycastHit> list = new List<RaycastHit>(collection);
		if (PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<CameraEventsSender>().inFrontObject.transform != null)
		{
			list.Add(PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<CameraEventsSender>().inFrontObject);
		}
		Rigidbody componentInChildren = npc.GetComponentInChildren<Rigidbody>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].rigidbody == null || list[i].rigidbody != componentInChildren)
			{
				list[i].transform.position = CameraController.instance.MainCamera.transform.position - 2f * CameraController.instance.MainCamera.transform.forward;
			}
		}
	}

	private void AddThirdStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "hospital.tutorial.2";
		tutorialStep.infoDefaultText = "Tap a \"cure\" button!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
		{
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
				action = delegate
				{
					genericTutorial.autoChangeSteps = false;
				}
			},
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
				action = delegate
				{
					Time.timeScale = 1f;
					StartCoroutine(WaitForState(typeof(CurePatientState)));
				}
			}
		};
		GenericTutorial.TutorialStep step = tutorialStep;
		AddElementToClickOnSpawn(step, GetCureButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(step);
	}

	private GameObject GetCureButton()
	{
		return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<CurePatientModule>()
			.cureButton.gameObject;
		}

		private void AddFourthStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
			tutorialStep.infoKey = "hospital.tutorial.3";
			tutorialStep.infoDefaultText = "To heal this person, you need to catch the virus molecule inside the pill!";
			tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
					action = delegate
					{
						genericTutorial.autoChangeSteps = true;
					}
				}
			};
			GenericTutorial.TutorialStep step = tutorialStep;
			AddTutorialStepToArray(step);
		}

		private void AddFifthStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.infoKey = "hospital.tutorial.4";
			tutorialStep.infoDefaultText = "Once the heart icon is filled with color, your treatment is successful!";
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
					action = delegate
					{
						Manager.Get<StateMachineManager>().GetStateInstance<CurePatientState>().activeFight = true;
						genericTutorial.autoChangeSteps = false;
						genericTutorial.blockInputBetweenSteps = true;
						genericTutorial.allowInputThroughMask = true;
						StartCoroutine(WaitForState(typeof(GameplayState)));
						genericTutorial.NextStep();
					}
				}
			};
			GenericTutorial.TutorialStep step = tutorialStep;
			AddElementToClickOnSpawn(step, GetCureCureStateButton, AddDefaultButtonCallbacks);
			AddTutorialStepToArray(step);
			AddWhiteStep();
		}

		private GameObject GetCureCureStateButton()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<CurePatientState>().GetCureButton();
		}

		private void AddSixthStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
			tutorialStep.infoKey = "hospital.tutorial.5";
			tutorialStep.infoDefaultText = "You've done well with this case! I'm impressed! But this world is full of people who need your help! Some of them might need better items for their treatment!";
			tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
					action = delegate
					{
						genericTutorial.autoChangeSteps = true;
						genericTutorial.blockInputBetweenSteps = true;
						genericTutorial.allowInputThroughMask = false;
						npc.GetComponent<AnimalMob>().logic = AnimalMob.AnimalLogic.WANDER;
					}
				}
			};
			GenericTutorial.TutorialStep step = tutorialStep;
			AddTutorialStepToArray(step);
		}

		private void AddSeventhStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.infoKey = "hospital.tutorial.6";
			tutorialStep.infoDefaultText = "Tap a \"menu\" button! ";
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
					action = delegate
					{
						Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
						{
							disableAds = true,
							categoryToOpen = "HospitalUpgrades"
						});
						genericTutorial.autoChangeSteps = false;
						genericTutorial.blockInputBetweenSteps = true;
						genericTutorial.allowInputThroughMask = true;
						StartCoroutine(WaitForState(typeof(PauseState)));
						genericTutorial.NextStep();
					}
				}
			};
			GenericTutorial.TutorialStep step = tutorialStep;
			AddElementToClickOnSpawn(step, GetPauseButton);
			AddTutorialStepToArray(step);
			AddWhiteStep();
		}

		private GameObject GetPauseButton()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<PauseModule>()
				.pauseButton.gameObject;
			}

			private void AddEighthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "hospital.tutorial.7";
				tutorialStep.infoDefaultText = "This is your EQUIPMENT window.";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
				{
					new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
						action = delegate
						{
							genericTutorial.blockInputBetweenSteps = false;
							genericTutorial.allowInputThroughMask = false;
							genericTutorial.autoChangeSteps = true;
						}
					}
				};
				GenericTutorial.TutorialStep step = tutorialStep;
				AddElementToClickOnSpawn(step, GetEqPanel);
				AddTutorialStepToArray(step);
			}

			private GameObject GetEqPanel()
			{
				PauseState pauseState = Manager.Get<StateMachineManager>().currentState as PauseState;
				HospitalUpgradesFragment hospitalUpgradesFragment = pauseState.getCurrentFragment.instance.GetComponent<Fragment>() as HospitalUpgradesFragment;
				return hospitalUpgradesFragment.upgradesContentPanel;
			}

			private void AddNinthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "hospital.tutorial.8";
				tutorialStep.infoDefaultText = "It's easy: the better EQUIPMENT you have, the more diseases you can cure. To earn coins, treat as many patients as you can!";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				GenericTutorial.TutorialStep step = tutorialStep;
				AddElementToClickOnSpawn(step, GetEqPanel);
				AddTutorialStepToArray(step);
			}

			private void AddTenthStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.infoKey = "hospital.tutorial.9";
				tutorialStep.infoDefaultText = "This is your PRESTIGE! Treat patients, buy EQUIPMENT, CRAFT hospital themed items and advance on the \"Best Doctors\" leaderboard!";
				tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
				tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
				{
					new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
						action = delegate
						{
							MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
							genericTutorial.EndTutorial();
							UnityEngine.Object.Destroy(base.gameObject);
							genericTutorial.autoChangeSteps = false;
							genericTutorial.blockInputBetweenSteps = true;
							genericTutorial.allowInputThroughMask = true;
							CameraController.instance.InputSource.IsEnabled = true;
							CameraController.instance.SetDefaultCameraPreset();
							CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
							if (hospitalManager.vampireMode.enabled)
							{
								hospitalManager.vampireMode.TryInitHunger();
							}
						}
					}
				};
				GenericTutorial.TutorialStep step = tutorialStep;
				AddElementToClickOnSpawn(step, GetPrestigeSliderPanel);
				AddTutorialStepToArray(step);
			}

			private GameObject GetPrestigeSliderPanel()
			{
				PauseState pauseState = Manager.Get<StateMachineManager>().currentState as PauseState;
				HospitalUpgradesFragment hospitalUpgradesFragment = pauseState.getCurrentFragment.instance.GetComponent<Fragment>() as HospitalUpgradesFragment;
				return hospitalUpgradesFragment.levelSlider.gameObject;
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

			private IEnumerator WaitForState(Type state)
			{
				yield return null;
				StateMachineManager stateMachine = Manager.Get<StateMachineManager>();
				while (stateMachine.currentState.GetType() != state)
				{
					yield return null;
				}
				yield return null;
				genericTutorial.NextStep();
			}

			private void AddWhiteStep()
			{
				GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
				tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
				tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
				tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
				{
					new GenericTutorial.TutorialCallback
					{
						type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
						action = delegate
						{
							genericTutorial.autoChangeSteps = false;
							genericTutorial.blockInputBetweenSteps = false;
							genericTutorial.allowInputThroughMask = true;
						}
					}
				};
				GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
				AddTutorialStepToArray(tutorialStep2);
				tutorialStep2.configInfoPanelAfterSpawn = delegate(GameObject p)
				{
					p.SetActive(value: false);
				};
			}
		}
