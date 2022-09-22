// DecompilerFi decompiler from Assembly-CSharp.dll class: ApplyLightingSettings
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class ApplyLightingSettings : MonoBehaviour
{
	[Serializable]
	public class LightingInfo
	{
		public Material skyboxMaterial;

		public Color ambientColor = Color.gray;

		[Range(0f, 8f)]
		public float ambienIntensity = 1f;

		public bool fogEnabled;

		public Color fogColor = Color.gray;

		public bool hasRain;

		public bool hasNightVignette;

		public bool hasHalosAndLights;
	}

	public LightingInfo info;

	public void ApplyLighting()
	{
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			Skybox component = camera.GetComponent<Skybox>();
			if (component != null)
			{
				component.material = info.skyboxMaterial;
			}
		}
		RenderSettings.skybox = info.skyboxMaterial;
		RenderSettings.ambientMode = AmbientMode.Flat;
		RenderSettings.ambientSkyColor = info.ambientColor;
		RenderSettings.ambientIntensity = info.ambienIntensity;
		RenderSettings.fog = info.fogEnabled;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogColor = info.fogColor;
		RenderSettings.fogStartDistance = 65f;
		RenderSettings.fogEndDistance = 80f;
	}
}
