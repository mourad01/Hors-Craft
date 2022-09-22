// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimatedPrefabMultiCutscene
using com.ootii.Cameras;
using Common;
using Common.Managers;
using Common.Utils;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatedMultiCutscene", menuName = "ScriptableObjects/Animated Multi 2D Cutscene")]
public class AnimatedPrefabMultiCutscene : ScriptableCutscene
{
	[Serializable]
	public class SingleCutsceneInfo
	{
		public GameObject prefab;

		public Sprite sprite;

		public float duration = 3f;
	}

	public List<SingleCutsceneInfo> cutscenes = new List<SingleCutsceneInfo>();

	private SingleCutsceneInfo currentCutscene;

	private Queue<SingleCutsceneInfo> cutscenesToShow;

	private float startTime;

	private GameObject instance;

	private bool waitingForSceneToLoad;

	public override void Show()
	{
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
		{
			Manager.Get<StateMachineManager>().PushState<SimpleCutsceneState>();
			cutscenesToShow = new Queue<SingleCutsceneInfo>(cutscenes);
			SetupNewScene();
			ShowNextCutscene();
		}
	}

	private bool SetupNewScene()
	{
		currentCutscene = cutscenesToShow.DequeueOrNull();
		if (currentCutscene != null)
		{
			return true;
		}
		return false;
	}

	private void ShowNextCutscene()
	{
		waitingForSceneToLoad = false;
		startTime = Time.realtimeSinceStartup;
		if (currentCutscene.sprite != null)
		{
			Manager.Get<StateMachineManager>().GetStateInstance<SimpleCutsceneState>().FadeFor(currentCutscene.sprite, SpawnAnimatedPrefab);
		}
		else
		{
			SpawnAnimatedPrefab();
		}
	}

	private void SpawnAnimatedPrefab()
	{
		TimeScaleHelper.value = 0f;
		CameraController.instance.MainCamera.gameObject.SetActive(value: false);
		instance = UnityEngine.Object.Instantiate(currentCutscene.prefab);
		Animator[] componentsInChildren = instance.GetComponentsInChildren<Animator>();
		Animator[] array = componentsInChildren;
		foreach (Animator animator in array)
		{
			animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		}
		ParticleSystem[] componentsInChildren2 = instance.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array2 = componentsInChildren2;
		foreach (ParticleSystem particleSystem in array2)
		{
			RealtimeParticleSystem component = particleSystem.gameObject.GetComponent<RealtimeParticleSystem>();
			if (component == null)
			{
				component = particleSystem.gameObject.AddComponent<RealtimeParticleSystem>();
			}
		}
	}

	public override bool EndCondition()
	{
		if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
		{
			return true;
		}
		if (Time.realtimeSinceStartup > startTime + currentCutscene.duration && !waitingForSceneToLoad)
		{
			if (SetupNewScene())
			{
				waitingForSceneToLoad = true;
				if (instance != null)
				{
					UnityEngine.Object.Destroy(instance);
				}
				Manager.Get<StateMachineManager>().GetStateInstance<SimpleCutsceneState>().FadeOut(ShowNextCutscene, 1f);
				return false;
			}
			return true;
		}
		return false;
	}

	public override void OnEnd()
	{
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
		{
			if (instance != null)
			{
				UnityEngine.Object.Destroy(instance);
			}
			CameraController.instance.MainCamera.gameObject.SetActive(value: true);
			Manager.Get<StateMachineManager>().GetStateInstance<SimpleCutsceneState>().FadeOut(OnEndState, 1f);
		}
	}

	private void OnEndState()
	{
		TimeScaleHelper.value = 1f;
		Manager.Get<StateMachineManager>().PopState();
	}
}
