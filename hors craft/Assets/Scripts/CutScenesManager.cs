// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenesManager
using Common.Managers;
using QuestSystems;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CutScenesManager : Manager
{
	public int sceneToTest;

	[SerializeField]
	protected int finishedParts;

	protected int currentScene;

	public CutScene[] scenes;

	public Action<int> onSceneEnded;

	public GameObject cameraObjectHolderPrefab;

	protected Camera cutScenesCam;

	protected ImageGrabber imageGrabber;

	protected Transform currentCaller;

	private static bool isCutScenePlaying;

	protected Dictionary<string, AnimatorHandler> animatorHandlers = new Dictionary<string, AnimatorHandler>();

	private Transform oldPlayerCameraParent;

	private Vector3 oldPosition;

	public ImageGrabber GetImageGrabber => imageGrabber;

	public Transform CurrentCaller => currentCaller;

	public static bool IsCutScenePlaying => isCutScenePlaying;

	public float CurrentActTime()
	{
		return scenes[currentScene].GetCurrentActTime();
	}

	public Camera GetSceneCamera()
	{
		if (cutScenesCam == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(cameraObjectHolderPrefab);
			cutScenesCam = gameObject.GetComponentInChildren<Camera>();
		}
		return cutScenesCam;
	}

	public void ResetProgress()
	{
		for (int i = 0; i < scenes.Length; i++)
		{
			string key = $"scene.{i}.PlayedTime";
			PlayerPrefs.DeleteKey(key);
		}
		PlayerPrefs.Save();
	}

	public override void Init()
	{
		imageGrabber = GetComponent<ImageGrabber>();
	}

	[ContextMenu("test")]
	public void Test()
	{
		StartScene(sceneToTest);
		UnityEngine.Debug.Break();
	}

	[ContextMenu("Show workers")]
	public void TestWorkers()
	{
		foreach (KeyValuePair<string, AnimatorHandler> animatorHandler in animatorHandlers)
		{
			UnityEngine.Debug.Log($"{animatorHandler.Key}, {animatorHandler.Value}");
		}
	}

	public void StartScene(int id, Transform caller = null)
	{
		if (isCutScenePlaying)
		{
			return;
		}
		UnityEngine.Debug.Log($"trying to play scene {id}");
		GetSceneCamera();
		if (scenes != null && scenes.Length > id && scenes[id] != null && scenes[currentScene].CanPlayNormalScene(id))
		{
			finishedParts = 0;
			currentScene = id;
			currentCaller = caller;
			if (scenes[currentScene].freezePlayer)
			{
				FreezePlayer(state: true);
			}
			Manager.Get<StateMachineManager>().PushState<CutScenesState>(new CutScenesStateParameter(delegate
			{
				scenes[id].Init();
				isCutScenePlaying = true;
				if (!scenes[id].StartScene())
				{
					UnityEngine.Debug.LogError("Error loading a scene!");
					End();
				}
				else
				{
					scenes[id].RegisterPlayed(id);
				}
			}));
		}
	}

	internal void OnPartEnd()
	{
		finishedParts++;
		int partsInActCount = scenes[currentScene].PartsInActCount;
		if (finishedParts >= partsInActCount)
		{
			finishedParts = 0;
			if (!scenes[currentScene].StartNextAct())
			{
				UnityEngine.Debug.Log("No more acts. Scene ended.");
				End();
			}
		}
	}

	private void End()
	{
		isCutScenePlaying = false;
		if (scenes[currentScene].freezePlayer)
		{
			FreezePlayer(state: false);
		}
		if (cutScenesCam != null && cutScenesCam.transform != null)
		{
			GameObject gameObject = new GameObject("temp camera Holder");
			cutScenesCam.transform.root.SetParent(gameObject.transform);
			gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(gameObject);
		}
		currentCaller = null;
		if (Manager.Get<StateMachineManager>().IsCurrentStateA<CutScenesState>())
		{
			Manager.Get<StateMachineManager>().PopState();
		}
		if (onSceneEnded != null)
		{
			onSceneEnded(currentScene);
			onSceneEnded = null;
		}
	}

	internal void RegisterAnimator(string key, AnimatorHandler animatorHandler)
	{
		UnityEngine.Debug.Log($"Registering animator : {key}");
		if (animatorHandlers == null)
		{
			animatorHandlers = new Dictionary<string, AnimatorHandler>();
		}
		if (animatorHandlers.ContainsKey(key))
		{
			animatorHandlers[key] = animatorHandler;
		}
		else
		{
			animatorHandlers.Add(key, animatorHandler);
		}
		Init();
	}

	public bool GetAnimator(string key, out Animator animator)
	{
		UnityEngine.Debug.Log($"Getting animator ( {key} ) ...");
		animator = null;
		if (animatorHandlers == null)
		{
			return false;
		}
		if (!animatorHandlers.ContainsKey(key))
		{
			return false;
		}
		if (animatorHandlers[key].animator == null)
		{
			return false;
		}
		UnityEngine.Debug.Log("... Got animator!");
		animator = animatorHandlers[key].animator;
		return true;
	}

	internal void StartIntroScene(bool outro = false, Transform caller = null)
	{
		if (!Manager.Contains<SavedWorldManager>())
		{
			return;
		}
		SavedWorldManager savedWorldManager = Manager.Get<SavedWorldManager>();
		if (savedWorldManager == null)
		{
			return;
		}
		int currentWorldDataIndex = savedWorldManager.currentWorldDataIndex;
		int num = 0;
		while (true)
		{
			if (num < scenes.Length)
			{
				if (scenes[num].CanPlayIntro(currentWorldDataIndex, num, outro))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		StartScene(num, caller);
	}

	private void FreezePlayer(bool state)
	{
	}
}
