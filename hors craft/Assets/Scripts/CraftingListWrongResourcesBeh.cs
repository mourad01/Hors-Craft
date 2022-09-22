// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftingListWrongResourcesBeh
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CraftingListWrongResourcesBeh : CustomSearchBehaviourAbstractPure
{
	private class CountPath
	{
		private class IdCounts
		{
			public int Id;

			public int Count;

			public HashSet<string> Pathes = new HashSet<string>();

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Id: {0}; Count: {1};\n", Id, Count);
				foreach (string pathe in Pathes)
				{
					stringBuilder.AppendLine(pathe);
				}
				return stringBuilder.ToString();
			}
		}

		private List<IdCounts> Ids = new List<IdCounts>();

		public CountPath Add(int id, string path)
		{
			IdCounts idCounts = Ids.FirstOrDefault((IdCounts o) => o.Id == id);
			if (idCounts == null)
			{
				idCounts = new IdCounts();
				idCounts.Id = id;
				Ids.Add(idCounts);
			}
			idCounts.Count++;
			idCounts.Pathes.Add(path);
			return this;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine((Ids.Count <= 1) ? "Correct" : "Wrong");
			for (int i = 0; i < Ids.Count; i++)
			{
				stringBuilder.AppendLine(Ids[i].ToString());
			}
			return stringBuilder.ToString();
		}
	}

	private static Dictionary<Sprite, CountPath> dict = new Dictionary<Sprite, CountPath>();

	public override Action<GameObject, string> GetFunction()
	{
		return Function;
	}

	public override Action GetLastAction()
	{
		return Last;
	}

	private void Function(GameObject gameObject, string path)
	{
		CraftableList component = gameObject.GetComponent<CraftableList>();
		if (component == null || component.resourcesList == null)
		{
			UnityEngine.Debug.LogError("Empty list in " + path);
			return;
		}
		for (int i = 0; i < component.resourcesList.Count; i++)
		{
			CraftableList.ResourceSpawn resourceSpawn = component.resourcesList[i];
			if (resourceSpawn.id == 14 && resourceSpawn.image.name == "wood")
			{
				resourceSpawn.id = 8;
			}
			if (dict.ContainsKey(resourceSpawn.image))
			{
				dict[resourceSpawn.image].Add(resourceSpawn.id, path);
			}
			else
			{
				dict.Add(resourceSpawn.image, new CountPath().Add(resourceSpawn.id, path));
			}
		}
		for (int j = 0; j < component.craftableList.Count; j++)
		{
			Craftable craftable = component.craftableList[j];
			for (int k = 0; k < craftable.requiredResourcesToCraft.Count; k++)
			{
				if (craftable.requiredResourcesToCraft[k].id == 14)
				{
					craftable.requiredResourcesToCraft[k].id = 8;
				}
			}
		}
	}

	private void Last()
	{
		foreach (KeyValuePair<Sprite, CountPath> item in dict)
		{
			UnityEngine.Debug.LogErrorFormat("Sprite name: {0}; Log: {1}", item.Key.name, item.Value.ToString());
		}
	}
}
