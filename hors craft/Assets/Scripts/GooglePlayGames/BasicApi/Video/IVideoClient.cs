// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.BasicApi.Video.IVideoClient
using System;

namespace GooglePlayGames.BasicApi.Video
{
	public interface IVideoClient
	{
		void GetCaptureCapabilities(Action<ResponseStatus, VideoCapabilities> callback);

		void ShowCaptureOverlay();

		void GetCaptureState(Action<ResponseStatus, VideoCaptureState> callback);

		void IsCaptureAvailable(VideoCaptureMode captureMode, Action<ResponseStatus, bool> callback);

		bool IsCaptureSupported();

		void RegisterCaptureOverlayStateChangedListener(CaptureOverlayStateListener listener);

		void UnregisterCaptureOverlayStateChangedListener();
	}
}
