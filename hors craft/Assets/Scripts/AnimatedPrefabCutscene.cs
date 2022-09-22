// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimatedPrefabCutscene
using com.ootii.Cameras;
using Common;
using Common.Managers;
using Common.Utils;
using States;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatedCutscene", menuName = "ScriptableObjects/Animated 2D Cutscene")]
public class AnimatedPrefabCutscene : ScriptableCutscene
{
	public GameObject prefab;

	public Sprite sprite;

	public float duration = 3f;

	private float startTime;

	private GameObject instance;

	public override void Show()
	{
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
		{
			startTime = Time.realtimeSinceStartup;
			Manager.Get<StateMachineManager>().PushState<SimpleCutsceneState>();
			if (sprite != null)
			{
				Manager.Get<StateMachineManager>().GetStateInstance<SimpleCutsceneState>().FadeFor(sprite, SpawnAnimatedPrefab);
			}
			else
			{
				SpawnAnimatedPrefab();
			}
		}
	}

	private void SpawnAnimatedPrefab()
	{
		TimeScaleHelper.value = 0f;
		CameraController.instance.MainCamera.gameObject.SetActive(value: false);
		instance = Object.Instantiate(prefab);
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
		return Time.realtimeSinceStartup > startTime + duration;
	}

	public override void OnEnd()
	{
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
		{
			CameraController.instance.MainCamera.gameObject.SetActive(value: true);
			Manager.Get<StateMachineManager>().GetStateInstance<SimpleCutsceneState>().FadeOut(OnEndState, 1.3f);
			UnityEngine.Object.Destroy(instance);
		}
	}

	private void OnEndState()
	{
		TimeScaleHelper.value = 1f;
		Manager.Get<StateMachineManager>().PopState();
	}
}
