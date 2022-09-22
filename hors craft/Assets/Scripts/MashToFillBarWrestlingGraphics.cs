// DecompilerFi decompiler from Assembly-CSharp.dll class: MashToFillBarWrestlingGraphics
using Common.Managers;
using States;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class MashToFillBarWrestlingGraphics : TappingGameGraphics
{
	public Image progressBar;

	public Image progressBorder;

	public Sprite frame;

	public Sprite frameWithGlow;

	public Color winColor;

	public Color loseColor;

	public Color normalColor;

	public Text notificationText;

	public GameObject ourArm;

	public Animator enemyAnim;

	public Transform cameraPivot;

	public float defaultProgressAnim = 0.5f;

	private PlayerMovement playerMovement;

	public override void OnProgress(float amount)
	{
		enemyAnim.SetFloat("progress", amount);
		progressBar.fillAmount = amount;
	}

	public override void OnLose()
	{
		progressBorder.sprite = frameWithGlow;
		progressBorder.color = loseColor;
		notificationText.gameObject.SetActive(value: true);
		notificationText.text = Manager.Get<TranslationsManager>().GetText("tapping.failure", "FAILED!").ToUpper();
		StartCoroutine(CallAfter("fighting"));
	}

	public override void OnWin()
	{
		progressBorder.sprite = frameWithGlow;
		progressBorder.color = winColor;
		notificationText.gameObject.SetActive(value: true);
		notificationText.text = Manager.Get<TranslationsManager>().GetText("tapping.success", "SUCCESS!").ToUpper();
		StartCoroutine(CallAfter("fighting"));
	}

	public override void OnResetRound()
	{
		progressBorder.sprite = frame;
		progressBorder.color = normalColor;
		notificationText.text = Manager.Get<TranslationsManager>().GetText("tapping.getready", "GET READY!").ToUpper();
		notificationText.gameObject.SetActive(value: true);
		StartCoroutine(CallAfter("fighting", activate: true));
		progressBar.fillAmount = defaultProgressAnim;
		enemyAnim.SetFloat("progress", defaultProgressAnim);
	}

	public override void InitGraphics()
	{
		ourArm.SetActive(value: true);
		playerMovement = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>();
		playerMovement.StartCustomCutscene(cameraPivot);
		progressBar.fillAmount = defaultProgressAnim;
	}

	public override void OnFinish()
	{
		enemyAnim.SetBool("fighting", value: false);
		ourArm.SetActive(value: false);
		playerMovement.EndCustomCutscene();
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
