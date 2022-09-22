// DecompilerFi decompiler from Assembly-CSharp.dll class: DiscoLight
using UnityEngine;

public class DiscoLight : MonoBehaviour
{
	public float interval = 0.5f;

	public float value = 0.8f;

	public float saturation = 1f;

	private Light discoLight;

	private float timer;

	private void Awake()
	{
		discoLight = GetComponent<Light>();
	}

	private void Update()
	{
		if (timer > interval)
		{
			timer = 0f;
			PickNewLight();
		}
		timer += Time.deltaTime;
	}

	private void PickNewLight()
	{
		Color.RGBToHSV(discoLight.color, out float H, out float S, out float V);
		H = UnityEngine.Random.value;
		V = value;
		S = saturation;
		discoLight.color = Color.HSVToRGB(H, S, V);
	}
}
