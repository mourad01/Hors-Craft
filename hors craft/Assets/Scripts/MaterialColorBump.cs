// DecompilerFi decompiler from Assembly-CSharp.dll class: MaterialColorBump
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialColorBump : MonoBehaviour
{
	public EaseType easing;

	public Color startColor;

	public Color endColor;

	public float duration = 2f;

	public List<Material> materials = new List<Material>();

	public bool playOnAwake;

	public bool destroyAfter = true;

	public Action doAfter;

	private float startTime;

	private bool active;

	private Dictionary<Material, Shader> oldShaders = new Dictionary<Material, Shader>();

	private void Awake()
	{
		if (playOnAwake)
		{
			Start();
		}
	}

	[ContextMenu("Play")]
	public void Start()
	{
		startTime = Time.realtimeSinceStartup;
		active = true;
		Shader shader = Shader.Find("_X/Diffuse + Color Highlight");
		materials.ForEach(delegate(Material m)
		{
			oldShaders[m] = m.shader;
			m.shader = shader;
		});
	}

	private void Update()
	{
		if (active)
		{
			materials = (from m in materials
				where m != null
				select m).ToList();
			float progress = (Time.realtimeSinceStartup - startTime) / duration;
			if (progress >= 1f)
			{
				End();
			}
			else
			{
				materials.ForEach(delegate(Material m)
				{
					m.SetColor("_HighlightColor", Easing.EaseColor(easing, startColor, endColor, progress));
				});
			}
		}
	}

	private void End()
	{
		materials.ForEach(delegate(Material m)
		{
			m.color = endColor;
			m.shader = oldShaders[m];
		});
		if (doAfter != null)
		{
			doAfter();
		}
		if (destroyAfter)
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
