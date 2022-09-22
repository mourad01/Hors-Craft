// DecompilerFi decompiler from Assembly-CSharp.dll class: GridLayoutConstraintsBasedOnAspectRatio
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GridLayoutConstraintsBasedOnAspectRatio : MonoBehaviour
{
	[Serializable]
	public class Config
	{
		public float aspect;

		public int count;
	}

	public GridLayoutGroup grid;

	public RectTransform viewport;

	public List<Config> configs;

	public bool debug;

	private void OnGUI()
	{
		if (!(grid == null) && !(viewport == null))
		{
			float aspect = viewport.rect.width / viewport.rect.height;
			if (debug)
			{
				UnityEngine.Debug.LogError(aspect);
			}
			Config config = configs.LastOrDefault((Config c) => c.aspect <= aspect);
			GridLayoutGroup gridLayoutGroup = grid;
			int? num = config?.count;
			gridLayoutGroup.constraintCount = ((!num.HasValue) ? grid.constraintCount : num.Value);
		}
	}
}
