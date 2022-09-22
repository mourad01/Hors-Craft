// DecompilerFi decompiler from Assembly-CSharp.dll class: WalkwayScene
using com.ootii.Cameras;
using Common.Managers;
using Gameplay.Minigames;
using Gameplay.RhythmicMinigame;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class WalkwayScene : RhythmReactor
{
	private Transform walkWayObject;

	private Vector3 humanBodyOldPosition;

	private Vector3 walkwayOldPosition;

	private Vector3 offset;

	private RuntimeAnimatorController playerAnimator;

	private Transform oldParent;

	private GameObject newParent;

	private string animatorKeyMiss = "DressupClickMiss";

	private string[] animatorKeySequencesOk = new string[3]
	{
		"DressupSequenceOk1",
		"DressupSequenceOk2",
		"DressupSequenceOk3"
	};

	private Dictionary<string, WalkwayAditionalObject> aditionalObjects;

	private string animatorKeyWalking = "DressupStartWalking";

	private string animatorKeyWonGame = "DressupMinigameSuccess";

	private bool playerMissed;

	private PlayerMovement _playerMovement;

	public WalkwayScene(PlayerGraphic playerGraphic, Transform walkWayObject, Transform walkwayStartPoint, Vector3 startOffset, RuntimeAnimatorController overridePlayerAnimator = null, Dictionary<string, WalkwayAditionalObject> aditionalObjects = null)
	{
		if (_playerMovement == null)
		{
			_playerMovement = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
		}
		_playerMovement.EnableMovement(enable: false);
		playerAnimator = null;
		playerMissed = false;
		this.walkWayObject = walkWayObject;
		humanBodyOldPosition = playerGraphic.mainBody.transform.position;
		walkwayOldPosition = walkWayObject.transform.position;
		offset = startOffset;
		oldParent = playerGraphic.mainBody.transform.parent;
		newParent = new GameObject();
		newParent.transform.SetParent(walkwayStartPoint);
		SetPlayerPostion(newParent.transform, walkwayStartPoint.position);
		SetPlayerPostion(playerGraphic.mainBody.transform, walkwayStartPoint.position);
		playerGraphic.mainBody.transform.SetParent(newParent.transform);
		playerGraphic.mainBody.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		if (overridePlayerAnimator != null)
		{
			OverrideStandardAnimator(playerGraphic, overridePlayerAnimator);
		}
		SendTriggerToAnimator(animatorKeyWalking);
		this.aditionalObjects = aditionalObjects;
		foreach (KeyValuePair<string, WalkwayAditionalObject> aditionalObject in aditionalObjects)
		{
			if (!(aditionalObject.Value.aditionalObject == null))
			{
				WalkWayPart component = aditionalObject.Value.aditionalObject.GetComponent<WalkWayPart>();
				if (!(component == null))
				{
					component.Init();
					component.Move();
				}
			}
		}
		if (aditionalObjects.ContainsKey("rays"))
		{
			aditionalObjects["rays"].aditionalObject.GetComponent<ParticleSystem>().Play();
		}
		if (aditionalObjects.ContainsKey("lights"))
		{
			aditionalObjects["lights"].aditionalObject.gameObject.SetActive(value: true);
		}
	}

	private void SendTriggerToAnimator(string key)
	{
		PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
		if (!(controlledPlayerInstance == null))
		{
			Animator component = controlledPlayerInstance.mainBody.GetComponent<Animator>();
			if (!(component == null))
			{
				component.SetTrigger(key);
			}
		}
	}

	private void OverrideStandardAnimator(PlayerGraphic playerGraphic, RuntimeAnimatorController overridePlayerAnimator)
	{
		Animator component = playerGraphic.mainBody.GetComponent<Animator>();
		if (component == null)
		{
			UnityEngine.Debug.Log("No animator found!");
			return;
		}
		playerAnimator = playerGraphic.mainBody.GetComponent<Animator>().runtimeAnimatorController;
		playerGraphic.mainBody.GetComponent<Animator>().runtimeAnimatorController = overridePlayerAnimator;
	}

	private void SetPlayerPostion(Transform player, Vector3 newPosition)
	{
		player.position = newPosition;
	}

	private void ShowFlashes()
	{
		UnityEngine.Debug.Log("Flash!");
		if (aditionalObjects == null || !aditionalObjects.ContainsKey("FlashesLeft") || aditionalObjects["FlashesLeft"] == null || aditionalObjects["FlashesLeft"].aditionalObject == null)
		{
			return;
		}
		ParticleSystem component = aditionalObjects["FlashesLeft"].aditionalObject.GetComponent<ParticleSystem>();
		if (component == null)
		{
			return;
		}
		component.Play();
		if (aditionalObjects.ContainsKey("FlashesRight") && aditionalObjects["FlashesRight"] != null && !(aditionalObjects["FlashesRight"].aditionalObject == null))
		{
			ParticleSystem component2 = aditionalObjects["FlashesRight"].aditionalObject.GetComponent<ParticleSystem>();
			if (!(component2 == null))
			{
				component2.Play();
			}
		}
	}

	public StatsManager.MinigameType GetGameType()
	{
		return StatsManager.MinigameType.WALKWAY;
	}

	public void OnBeat(int beatIndex)
	{
	}

	public void OnBlobFall(int beatType)
	{
		playerMissed = true;
		SendTriggerToAnimator(animatorKeyMiss);
	}

	public void OnBlobInShootZone(int beatType)
	{
		UnityEngine.Debug.Log("On Blob In Shoot Zone!");
	}

	public void OnCorrectHit(int beatType)
	{
		UnityEngine.Debug.Log("Correct Hit!");
	}

	public void OnCorrectSequence()
	{
		UnityEngine.Debug.Log("OnCorrectSequence");
		ShowFlashes();
		SendTriggerToAnimator(GetRandomOkAnimation());
		PauseWalkWay();
	}

	private string GetRandomOkAnimation()
	{
		return animatorKeySequencesOk[Random.Range(0, 2)];
	}

	public void OnFailureScene()
	{
		UnityEngine.Debug.Log("OnFailureScene");
	}

	public void OnFinish()
	{
		PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
		controlledPlayerInstance.mainBody.transform.SetParent(oldParent);
		controlledPlayerInstance.mainBody.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		controlledPlayerInstance.mainBody.transform.position = humanBodyOldPosition;
		walkWayObject.position = walkwayOldPosition;
		OverrideStandardAnimator(controlledPlayerInstance, playerAnimator);
		if (_playerMovement == null)
		{
			_playerMovement = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
		}
		_playerMovement.EnableMovement(enable: true);
		_playerMovement.ChangeMoveMode(PlayerMovement.Mode.WALKING);
		foreach (KeyValuePair<string, WalkwayAditionalObject> aditionalObject in aditionalObjects)
		{
			if (!(aditionalObject.Value.aditionalObject == null))
			{
				WalkWayPart component = aditionalObject.Value.aditionalObject.GetComponent<WalkWayPart>();
				if (!(component == null))
				{
					component.CleanUp();
				}
			}
		}
		if (aditionalObjects.ContainsKey("lights"))
		{
			aditionalObjects["lights"].aditionalObject.gameObject.SetActive(value: false);
		}
	}

	public void OnIncorrectHit(int beatType)
	{
		UnityEngine.Debug.Log("On INCorrect Hit!");
		PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
		controlledPlayerInstance.mainBody.GetComponent<Animator>().SetTrigger(animatorKeyMiss);
		PauseWalkWay();
	}

	public void OnSpawnBlob(int beatType)
	{
	}

	public void OnSuccessScene()
	{
		UnityEngine.Debug.Log("Adding money to player!!!!");
		Manager.Get<ProgressManager>().IncreaseExperience(100);
		Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(50);
		if (!playerMissed)
		{
			Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.WinCatWalkFlawlessy);
		}
	}

	public void SetPosition(Camera camera)
	{
		camera.cullingMask = -1;
		walkWayObject.position = camera.transform.position + offset;
	}

	public void SetTempo(float tempo)
	{
	}

	public void Update()
	{
	}

	private void PauseWalkWay(float time = 1f)
	{
		foreach (KeyValuePair<string, WalkwayAditionalObject> aditionalObject in aditionalObjects)
		{
			if (!(aditionalObject.Value.aditionalObject == null))
			{
				WalkWayPart component = aditionalObject.Value.aditionalObject.GetComponent<WalkWayPart>();
				if (!(component == null))
				{
					component.Pause(time);
				}
			}
		}
	}
}
