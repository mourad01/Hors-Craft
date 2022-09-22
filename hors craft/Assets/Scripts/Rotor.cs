// DecompilerFi decompiler from Assembly-CSharp.dll class: Rotor
using System;
using UnityEngine;

public class Rotor : MonoBehaviour
{
	[Serializable]
	private struct FadeParams
	{
		[Range(0f, 1f)]
		public float minAlpha;

		[Range(0f, 1f)]
		public float maxAlpha;

		public float rateOfChange;
	}

	[Serializable]
	private struct TurnRates
	{
		[Range(0f, 1f)]
		public int xTurnRate;

		[Range(0f, 1f)]
		public int yTurnRate;

		[Range(0f, 1f)]
		public int zTurnRate;
	}

	[SerializeField]
	private float maxRPS;

	[SerializeField]
	private float rpsGainPerSecond;

	[SerializeField]
	private GameObject model;

	[SerializeField]
	private MeshRenderer fade;

	[SerializeField]
	private FadeParams fadeParams;

	[SerializeField]
	private TurnRates turnRates;

	private Vector3 currentRotation;

	private float currentRps;

	[SerializeField]
	private bool isRotating;

	private float currentSinAngle;

	private void Awake()
	{
		isRotating = false;
	}

	private void Start()
	{
		currentRotation = model.transform.localEulerAngles;
		currentRps = 0f;
		if ((bool)fade)
		{
			fade.material.color = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void Update()
	{
		if (isRotating)
		{
			Rotate();
		}
	}

	public void StartRotating()
	{
		fade.gameObject.SetActive(value: true);
		isRotating = true;
	}

	public void StopRotating()
	{
		isRotating = false;
		currentRps = 0f;
		fade.gameObject.SetActive(value: false);
	}

	private void Rotate()
	{
		currentRps = Mathf.Clamp(currentRps + rpsGainPerSecond * Time.deltaTime, 0f, maxRPS);
		float num = currentRps * Time.deltaTime;
		currentRotation += new Vector3(num * (float)turnRates.xTurnRate, num * (float)turnRates.yTurnRate, num * (float)turnRates.zTurnRate);
		model.transform.localEulerAngles = currentRotation;
		if ((bool)fade)
		{
			FadeAnim();
		}
	}

	private void FadeAnim()
	{
		currentSinAngle += fadeParams.rateOfChange;
		float num = Mathf.Abs(Mathf.Sin(currentSinAngle)) * fadeParams.maxAlpha + fadeParams.minAlpha;
		if (num > fadeParams.maxAlpha)
		{
			num = fadeParams.maxAlpha;
		}
		fade.material.color = new Color(0f, 0f, 0f, num);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(base.gameObject.transform.position, base.gameObject.transform.position + base.gameObject.transform.TransformDirection(Vector3.up) * 5f);
	}
}
