// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DidYouKnowPopupState
using Common.Managers;
using Common.Managers.States;
using System.Collections;
using UnityEngine;

namespace States
{
	public class DidYouKnowPopupState : XCraftUIState<DidYouKnowPopupStateConnector>
	{
		private DidYouKnowManager manager;

		private int didYouKnowIndex;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			manager = Manager.Get<DidYouKnowManager>();
			didYouKnowIndex = manager.GetDidYouKnowIndexToDisplay();
			base.connector.onCancel = OnCancel;
			base.connector.onNextHint = OnNextHint;
			base.connector.onGoTo = OnGoTo;
			base.connector.onPlay = OnPlay;
			SetPlayButtonImage();
			base.connector.Init(didYouKnowIndex, manager.NextHintEnabled(), manager.GoToFeatureEnabled());
		}

		public void OnPlay()
		{
			PlayVideo();
		}

		public void OnNextHint()
		{
			manager.SetNextDidYouKnowIndex();
			Manager.Get<StateMachineManager>().PopState();
			Manager.Get<StateMachineManager>().PushState<DidYouKnowPopupState>();
		}

		public void OnGoTo()
		{
			Manager.Get<StateMachineManager>().PopState();
			manager.OnGoToState();
			manager.OnPopupClose();
		}

		public void OnCancel()
		{
			manager.OnPopupClose();
			Manager.Get<StateMachineManager>().PopState();
		}

		private void PlayVideo()
		{
			string videoFilePath = manager.GetVideoFilePath(didYouKnowIndex);
			Handheld.PlayFullScreenMovie(videoFilePath, Color.black, FullScreenMovieControlMode.CancelOnInput);
		}

		private void SetPlayButtonImage()
		{
			string text = manager.GetPlayImageFilePath(didYouKnowIndex);
			if (didYouKnowIndex > 0)
			{
				text = $"file://{text}";
			}
			StartCoroutine(LoadAndSetImage(text));
		}

		private IEnumerator LoadAndSetImage(string path)
		{
			WWW www = new WWW(path, FormFactory.CreateBasicWWWForm());
			yield return www;
			Texture2D texture = www.texture;
			base.connector.playButtonImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0f, 0f));
		}
	}
}
