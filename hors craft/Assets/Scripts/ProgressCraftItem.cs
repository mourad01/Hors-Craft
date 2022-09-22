// DecompilerFi decompiler from Assembly-CSharp.dll class: ProgressCraftItem
using States;
using System;

public class ProgressCraftItem : CraftItem
{
	public TranslateText lockText;

	protected override Action<int> additionalAction => delegate(int id)
	{
		InitLevelText(ProgressCraftingRecipesFragment.GetUnlockLevel(id));
	};

	public void InitLevelText(int toUnlock)
	{
		lockText.AddVisitor((string t) => string.Format(t, toUnlock));
	}
}
