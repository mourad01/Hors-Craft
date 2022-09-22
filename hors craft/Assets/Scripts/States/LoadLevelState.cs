// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadLevelState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using System;
using Uniblocks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace States
{
	public class LoadLevelState : XCraftUIState<LoadLevelStateConnector>
	{
		private StartParameter parameter;

		private LoadLevelStateStartParameter startParameter;

		public Action startLoading;

		public Func<bool> isLoaded;

		public Action onLoaded;

		public Voxel loadingBlock;

		private float loadIn;

		private bool waitingToLoad;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			this.parameter = parameter;
			startParameter = (parameter as LoadLevelStateStartParameter);
			if (startParameter == null)
			{
				InitDefaultBehaviour();
			}
			else
			{
				startParameter.SetupLoadLevelState(this);
			}
			loadIn = Time.realtimeSinceStartup + 2.1f;
			waitingToLoad = true;
			base.connector.InitCube(loadingBlock);
		}

		private void InitDefaultBehaviour()
		{
			startLoading = LoadGameplay;
			onLoaded = delegate
			{
				base.connector.DestroyCube();
				GlobalSettings.LoadSettings();
				Manager.Get<StateMachineManager>().SetState<GameplayState>(parameter);
			};
		}

		public void LoadScene(string name)
		{
			startLoading = delegate
			{
				AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
				isLoaded = (() => async.isDone);
			};
		}

		public void LoadStateAfter(Type stateType, StartParameter parameter = null)
		{
			onLoaded = delegate
			{
				base.connector.DestroyCube();
				Manager.Get<StateMachineManager>().SetState(stateType, parameter);
			};
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (Time.realtimeSinceStartup > loadIn && waitingToLoad)
			{
				if (startLoading != null)
				{
					startLoading();
				}
				waitingToLoad = false;
			}
			if (isLoaded != null && isLoaded())
			{
				if (onLoaded != null)
				{
					onLoaded();
				}
				isLoaded = null;
			}
		}

		private void LoadGameplay()
		{
			Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
			Camera[] array2 = array;
			foreach (Camera camera in array2)
			{
				AudioListener component = camera.GetComponent<AudioListener>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			AsyncOperation async = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
			isLoaded = (() => async.isDone);
		}
	}
}
