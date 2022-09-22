// DecompilerFi decompiler from Assembly-CSharp.dll class: HospitalVampireMode
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HospitalVampireMode : IGameCallbacksListener
{
	public bool enabled;

	[Header("Parameters")]
	public LayerMask fearMobsLayerMask;

	public float sphereCastRadius = 15f;

	public float maxHungerFeed = 50f;

	public int singleFeedQuantity = 2;

	public float fasterMovementWalkingSpeed = 8f;

	public float scaredEmoticonYOffset = 1f;

	[Header("References")]
	public SkinList vampireSkinList;

	public Sprite scaredEmoticon;

	private Hunger hungerInstance;

	private HospitalManager hospitalManager;

	public void Init(HospitalManager manager)
	{
		hospitalManager = manager;
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		if (PlayerPrefs.GetInt("hospital.tutorial.shown", 0) == 1)
		{
			TryInitHunger();
		}
		if (manager.upgrades[7].level == 0)
		{
			manager.upgrades[7].OnLevelChanged = OnFasterMovementLevelChanged;
		}
		else
		{
			EnableFasterMovement();
		}
	}

	public void TryInitHunger()
	{
		if (hospitalManager.upgrades[4].level == 0)
		{
			InitHunger(hospitalManager.gameObject);
			hospitalManager.upgrades[4].OnLevelChanged = OnNoHungerLevelChanged;
		}
	}

	public void RemoveHunger()
	{
		if (hungerInstance != null)
		{
			UnityEngine.Object.Destroy(hungerInstance);
			hungerInstance = null;
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContexts<HungerContext>(Fact.HUNGER);
		}
	}

	public void EnableFasterMovement()
	{
		hospitalManager.StartCoroutine(TryEnableFasterMovement());
	}

	public void Feed()
	{
		if (hungerInstance != null)
		{
			hungerInstance.Feed(singleFeedQuantity);
		}
	}

	public void FearOtherMobs()
	{
		RaycastHit[] array = Physics.SphereCastAll(CameraController.instance.MainCamera.transform.position, sphereCastRadius, CameraController.instance.MainCamera.transform.forward, float.PositiveInfinity, fearMobsLayerMask.value);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			Pettable component = raycastHit.collider.gameObject.GetComponent<Pettable>();
			if (component != null)
			{
				component.FearRunFromPlayerMode();
			}
		}
	}

	public void OnNoHungerLevelChanged(int level)
	{
		if (level > 0)
		{
			RemoveHunger();
		}
	}

	public void OnFasterMovementLevelChanged(int level)
	{
		if (level > 0)
		{
			EnableFasterMovement();
		}
	}

	private void InitHunger(GameObject handler)
	{
		if (!(hungerInstance != null))
		{
			hungerInstance = handler.AddComponent<Hunger>();
			hungerInstance.maxFeed = maxHungerFeed;
			hungerInstance.saveLevelToWorldPrefs = false;
			hungerInstance.OnFeedLevelIsExhausted = ShowGameOverPopup;
			hungerInstance.Activate();
		}
	}

	private IEnumerator DoActionAfterTime(float seconds, Action action)
	{
		yield return new WaitForSeconds(seconds);
		action?.Invoke();
	}

	private void ShowGameOverPopup()
	{
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureTitle = delegate(TranslateText t)
			{
				t.translationKey = "hospital.popup.gameover.title";
				t.defaultText = "Koniec gry";
			},
			configureMessage = delegate(TranslateText t)
			{
				t.translationKey = "hospital.popup.gameover.message";
				t.defaultText = "Pij krew aby przetrwać lub odblokuj ulepszenie NoHunger";
			},
			configureRightButton = delegate(Button b, TranslateText t)
			{
				t.translationKey = "hospital.popup.gameover.okbutton";
				t.defaultText = "ok";
				b.gameObject.SetActive(value: true);
				b.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
					LoadLastSave();
				});
			},
			configureLeftButton = delegate(Button b, TranslateText t)
			{
				b.gameObject.SetActive(value: false);
			}
		});
	}

	private void LoadLastSave()
	{
		SavedWorldManager.UnloadCraftGameplay();
		Manager.Get<StateMachineManager>().SetState<LoadLevelState>();
	}

	private bool ChangePlayerMovementSpeed()
	{
		if (CameraController.instance.Anchor != null)
		{
			PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
			if (component != null)
			{
				component.walkingMaxForwardSpeed = fasterMovementWalkingSpeed;
				component.walkingMaxBackwardsSpeed = fasterMovementWalkingSpeed;
				component.walkingMaxSidewaysSpeed = fasterMovementWalkingSpeed;
				return true;
			}
		}
		return false;
	}

	private IEnumerator TryEnableFasterMovement()
	{
		bool setted = false;
		for (int i = 0; i < 10; i++)
		{
			if (setted)
			{
				break;
			}
			yield return new WaitForSeconds(0.1f);
			setted = ChangePlayerMovementSpeed();
		}
	}

	public void OnGameplayStarted()
	{
		if (hospitalManager.upgrades[7].level != 0)
		{
			EnableFasterMovement();
		}
		if (hungerInstance != null)
		{
			hungerInstance.OnFeedLevelIsExhausted = ShowGameOverPopup;
			hungerInstance.Activate();
		}
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayRestarted()
	{
	}
}
