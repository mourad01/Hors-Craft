// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlueprintAnimationState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class BlueprintAnimationState : XCraftUIState<BlueprintAnimationStateConnector>
	{
		public GameObject fireworksPrefab;

		public GameObject cameraPrefab;

		public AnimationCurve cameraMovement;

		public float animationTime;

		public float skipAnimationTime;

		private IsometricCamera cam;

		private GameObject camPivot;

		private PlayerGraphic player;

		private ParticleSystem fireworks;

		private float timer;

		private BlueprintAnimationStateStartParameter blueprintStartParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		//protected override bool canShowBanner => false;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			timer = Time.realtimeSinceStartup;
			base.connector.skipButton.onClick.AddListener(Skip);
			blueprintStartParameter = (startParameter as BlueprintAnimationStateStartParameter);
			TimeScaleHelper.value = 0f;
			float num = (!(blueprintStartParameter.size.x > blueprintStartParameter.size.y)) ? blueprintStartParameter.size.y : blueprintStartParameter.size.x;
			InitCamera(blueprintStartParameter.position, (int)((!(num > blueprintStartParameter.size.z)) ? blueprintStartParameter.size.z : num));
			DeactivatePlayer();
			InitFireworks(blueprintStartParameter.position, blueprintStartParameter.size);
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (Time.realtimeSinceStartup > animationTime + timer)
			{
				EndCompletedAnimation();
			}
		}

		private void EndCompletedAnimation()
		{
			UnityEngine.Object.Destroy(cam.gameObject);
			UnityEngine.Object.Destroy(fireworks.gameObject);
			UnityEngine.Object.Destroy(camPivot);
			TimeScaleHelper.value = 1f;
			player.gameObject.SetActive(value: true);
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("blueprint");
			}
			Manager.Get<StateMachineManager>().PopState();
		}

		private void InitCamera(Vector3 position, int height)
		{
			GameObject gameObject = Object.Instantiate(cameraPrefab);
			cam = gameObject.GetComponent<IsometricCamera>();
			camPivot = new GameObject();
			camPivot.transform.position = position;
			cam.Init(cameraMovement, camPivot.transform, height);
		}

		private void DeactivatePlayer()
		{
			player = PlayerGraphic.GetControlledPlayerInstance();
			if (player.GetComponent<PlayerMovement>().IsMounted())
			{
				player.GetComponent<PlayerMovement>().ForceUnmount();
			}
			player.gameObject.SetActive(value: false);
		}

		private void InitFireworks(Vector3 position, Vector3 size)
		{
			GameObject gameObject = Object.Instantiate(fireworksPrefab);
			gameObject.transform.position = new Vector3(position.x, position.y + size.y / 2f, position.z);
			fireworks = gameObject.GetComponent<ParticleSystem>();
			float num = (!(size.x > size.z)) ? size.z : size.x;
			fireworks.startSpeed = 4f + 0.25f * num;
			fireworks.emissionRate = 1f + 0.025f * num;
		}

		private void Skip()
		{
			if (Time.realtimeSinceStartup > skipAnimationTime + timer)
			{
				EndCompletedAnimation();
			}
		}
	}
}
