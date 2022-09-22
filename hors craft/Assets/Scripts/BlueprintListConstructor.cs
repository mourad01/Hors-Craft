// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintListConstructor
using Common.Managers;
using System.Collections.Generic;
using System.Linq;

public class BlueprintListConstructor : ScrollableListConstructor
{
	public override ScrollableListRawContent ConstructList()
	{
		List<Craftable> blueprints = GetBlueprints();
		BlueprintListRawContent blueprintListRawContent = new BlueprintListRawContent();
		blueprintListRawContent.blueprints = blueprints;
		return blueprintListRawContent;
	}

	private List<Craftable> GetBlueprints()
	{
		List<Craftable> craftableList = Manager.Get<CraftingManager>().GetCraftableListInstance().craftableList;
		return (from c in craftableList
			where c.recipeCategory == Craftable.RecipeCategory.BLUEPRINT && c.customCraftableObject != null
			select c).ToList();
	}
}
