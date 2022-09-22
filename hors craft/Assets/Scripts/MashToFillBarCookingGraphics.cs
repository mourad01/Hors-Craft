// DecompilerFi decompiler from Assembly-CSharp.dll class: MashToFillBarCookingGraphics
using Cooking;
using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MashToFillBarCookingGraphics : TappingGameGraphics
{
	public Image progressBar;

	public Image progressBorder;

	public Sprite frame;

	public Sprite frameWithGlow;

	public Color winColor;

	public Color normalColor;

	public Transform cameraPivot;

	public TimeDeviceSlot device;

	public GameObject successPrefab;

	public float defaultProgressAnim;

	private Vector3 baseCameraPosition;

	private Vector3 baseCameraRotation;

	private Transform cameraTransform;

	public override void OnProgress(float amount)
	{
		progressBar.fillAmount = amount;
	}

	public override void OnWin()
	{
		progressBorder.sprite = frameWithGlow;
		progressBorder.color = winColor;
		if (successPrefab != null)
		{
			successPrefab.SetActive(value: true);
		}
		device.GoIdle();
		StartCoroutine(CallAfter());
	}

	public override void OnResetRound()
	{
		progressBorder.sprite = frame;
		progressBorder.color = normalColor;
		callAfterAnimations();
		progressBar.fillAmount = defaultProgressAnim;
		device.Reset();
		device.startAnimating();
	}

	public override void InitGraphics()
	{
		cameraTransform = Camera.main.transform;
		baseCameraPosition = cameraTransform.position;
		baseCameraRotation = cameraTransform.eulerAngles;
		cameraTransform.position = cameraPivot.position;
		cameraTransform.rotation = cameraPivot.rotation;
		progressBar.fillAmount = defaultProgressAnim;
	}

	public override void OnFinish()
	{
		if (successPrefab != null)
		{
			successPrefab.SetActive(value: false);
		}
		cameraTransform.position = baseCameraPosition;
		cameraTransform.eulerAngles = baseCameraRotation;
	}

	private IEnumerator CallAfter(float time = 2f)
	{
		yield return new WaitForSecondsRealtime(time);
		callAfterAnimations();
	}

	public override List<GameObject> GetThingsForTutorial(TappingGameStateConnector connector)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(progressBar.gameObject);
		list.Add(connector.rightActionButton.gameObject);
		list.Add(connector.leftActionButton.gameObject);
		return list;
	}
}
