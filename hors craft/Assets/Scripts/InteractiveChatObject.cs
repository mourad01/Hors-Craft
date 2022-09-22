// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveChatObject
using com.ootii.Cameras;
using Common.Managers;
using States;
using Uniblocks;
using UnityEngine;

public class InteractiveChatObject : InteractiveObject
{
	[Header("References")]
	public Transform cameraPostionRotationObject;

	[Header("Parameters")]
	public string chatBotName;

	public bool isMale = true;

	public ChatBotState.CustomAnswers customChatAnswers;

	private Transform playerTransform;

	private Transform cameraTransform;

	private Vector3 playerPositionCopy;

	private Quaternion playerRotationCopy;

	private Quaternion cameraRotationCopy;

	protected override void Awake()
	{
		base.Awake();
	}

	public override void OnUse()
	{
		base.OnUse();
		OnTalkClicked();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void OnTalkClicked()
	{
		TrySetPlayerPosition();
		bool flag = false;
		if (customChatAnswers != null && !customChatAnswers.IsNullOrEmpty())
		{
			flag = true;
		}
		Manager.Get<StateMachineManager>().PushState<ChatBotState>(new ChatBotStateStartParameter
		{
			hideBackground = true,
			hideTamepanel = true,
			chatBootSeed = (int)Time.time,
			botName = ((!chatBotName.IsNullOrEmpty()) ? chatBotName : null),
			customAnswers = ((!flag) ? null : customChatAnswers),
			isMale = isMale,
			OnChatClosed = OnChatClosed
		});
	}

	private void TrySetPlayerPosition()
	{
		if (!(cameraPostionRotationObject == null))
		{
			playerTransform = CameraController.instance.Anchor.transform;
			cameraTransform = CameraController.instance.MainCamera.transform;
			if (!(playerTransform == null) && !(cameraTransform == null))
			{
				playerPositionCopy = playerTransform.position;
				playerRotationCopy = playerTransform.rotation;
				cameraRotationCopy = cameraTransform.rotation;
				playerTransform.position = cameraPostionRotationObject.position;
				playerTransform.rotation = cameraPostionRotationObject.rotation;
				cameraTransform.rotation = cameraPostionRotationObject.rotation;
			}
		}
	}

	private void OnChatClosed()
	{
		if (!(cameraPostionRotationObject == null) && !(playerTransform == null) && !(cameraTransform == null))
		{
			playerTransform.position = playerPositionCopy;
			playerTransform.rotation = playerRotationCopy;
			cameraTransform.rotation = cameraRotationCopy;
		}
	}
}
