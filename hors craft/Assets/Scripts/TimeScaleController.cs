// DecompilerFi decompiler from Assembly-CSharp.dll class: TimeScaleController
using System.Collections;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
	public AnimationCurve slowDownCurve;

	private float currentSlowDownTime;

	public AnimationCurve speedUpCurve;

	private float currentSpeedUpTime;

	public void SlowDown(float targetScale = 0f, float time = 0f)
	{
		currentSlowDownTime = 0f;
		StartCoroutine(SlowDownCO(targetScale, time));
	}

	private IEnumerator SlowDownCO(float targetScale = 0f, float time = 0f)
	{
		float originalScale = Time.timeScale;
		float deltaScale = Mathf.Abs(targetScale - originalScale);
		while (currentSlowDownTime < time)
		{
			currentSlowDownTime += Time.unscaledDeltaTime;
			float progress = currentSlowDownTime / time;
			float currentTimeScale = Time.timeScale = targetScale + slowDownCurve.Evaluate(progress) * deltaScale;
			yield return null;
		}
		Time.timeScale = targetScale;
	}

	public void SpeedUp(float targetScale = 1f, float time = 0f)
	{
		currentSpeedUpTime = 0f;
		StartCoroutine(SpeedUpCO(targetScale));
	}

	private IEnumerator SpeedUpCO(float targetScale = 1f, float time = 0f)
	{
		float originalScale = Time.timeScale;
		float deltaScale = Mathf.Abs(targetScale - originalScale);
		while (currentSpeedUpTime < time)
		{
			currentSpeedUpTime += Time.unscaledDeltaTime;
			float progress = currentSpeedUpTime / time;
			float currentTimeScale = Time.timeScale = originalScale + speedUpCurve.Evaluate(progress) * deltaScale;
			yield return null;
		}
		Time.timeScale = targetScale;
	}
}
