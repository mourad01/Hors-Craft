// DecompilerFi decompiler from Assembly-CSharp.dll class: MashToFillBarHealingGraphics
using com.ootii.Cameras;
using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MashToFillBarHealingGraphics : TappingGameGraphics
{
	public Image progressBar;

	public Image progressBorder;

	public Sprite frame;

	public Sprite frameWithGlow;

	public Color winColor;

	public Color loseColor;

	public Color normalColor;

	public Text notificationText;

	public Animator enemyAnim;

	public float defaultProgressAnim = 0.5f;

	public GameObject healingStationPrefab;

	public GameObject realPatient;

	public RuntimeAnimatorController patientAnimator;

	public GameObject bg;

	private GameObject healingStation;

	private RuntimeAnimatorController oldAnimator;

	private GameObject patientGraphic;

	private Vector3 baseCameraPosition;

	private Vector3 baseCameraRotation;

	private Transform cameraTransform;

	public override void OnProgress(float amount)
	{
		enemyAnim.SetFloat("progress", amount);
		progressBar.fillAmount = amount;
	}

	public override void OnLose()
	{
		progressBorder.sprite = frameWithGlow;
		progressBorder.color = loseColor;
		StartCoroutine(CallAfter("fighting"));
	}

	public override void OnWin()
	{
		progressBorder.sprite = frameWithGlow;
		progressBorder.color = winColor;
		StartCoroutine(CallAfter("fighting"));
	}

	public override void OnResetRound()
	{
		progressBorder.sprite = frame;
		progressBorder.color = normalColor;
		StartCoroutine(CallAfter("fighting", activate: true));
		progressBar.fillAmount = defaultProgressAnim;
		enemyAnim.SetFloat("progress", defaultProgressAnim);
	}

	public override void InitGraphics()
	{
		enemyAnim = realPatient.GetComponentInChildren<Animator>();
		oldAnimator = enemyAnim.runtimeAnimatorController;
		enemyAnim.runtimeAnimatorController = patientAnimator;
		healingStation = Object.Instantiate(healingStationPrefab, enemyAnim.transform.position, enemyAnim.transform.rotation);
		healingStation.transform.LookAt(CameraController.instance.transform);
		patientGraphic = enemyAnim.gameObject;
		patientGraphic.transform.SetParent(healingStation.transform.GetChild(0).transform);
		patientGraphic.SetLayerRecursively(healingStation.layer);
		progressBar.fillAmount = defaultProgressAnim;
		bg.transform.SetParent(base.transform.parent);
		bg.transform.SetAsFirstSibling();
	}

	public override void OnFinish()
	{
		enemyAnim.SetBool("fighting", value: false);
		CameraController.instance.SetDefaultCameraPreset();
		enemyAnim.runtimeAnimatorController = oldAnimator;
		patientGraphic.transform.SetParent(realPatient.transform);
		patientGraphic.transform.localPosition = Vector3.zero;
		patientGraphic.transform.localEulerAngles = Vector3.zero;
		patientGraphic.SetLayerRecursively(realPatient.layer);
		UnityEngine.Object.Destroy(healingStation);
		healingStation = null;
		enemyAnim = null;
		bg.transform.SetParent(base.transform);
		bg.transform.SetAsFirstSibling();
	}

	private IEnumerator CallAfter(string animationBool = "", bool activate = false, float time = 2f)
	{
		yield return new WaitForSecondsRealtime(time);
		if (animationBool.IsNOTNullOrEmpty())
		{
			enemyAnim.SetBool(animationBool, activate);
		}
		notificationText.gameObject.SetActive(value: false);
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
