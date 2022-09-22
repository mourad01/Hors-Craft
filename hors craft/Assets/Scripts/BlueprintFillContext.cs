// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintFillContext
using GameUI;
using System;
using UnityEngine.UI;

public class BlueprintFillContext : FactContext
{
	public Action instantFill;

	public Action destroy;

	public Action<SimpleRepeatButton> setFillVoxelButton;

	public Action<Slider, Text> setProgressSlider;

	public bool placedBlueprintVoxel;
}
