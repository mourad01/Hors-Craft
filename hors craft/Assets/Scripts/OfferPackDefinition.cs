// DecompilerFi decompiler from Assembly-CSharp.dll class: OfferPackDefinition
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(OfferPackDefinition))]
public class OfferPackDefinition
{
	public int test = 1;

	public List<PackItemData> packItems;

	public OfferPackDefinition()
	{
		packItems = new List<PackItemData>();
	}

	public OfferPackDefinition(string data)
	{
		string[] array = data.Split('|');
		packItems = new List<PackItemData>();
		Array.ForEach(array, delegate(string item)
		{
			PackItemData packItemData = PackFactory.ParseData(item);
			if (packItemData != null)
			{
				packItems.Add(packItemData);
			}
		});
	}

	public T TryToGet<T>() where T : PackItemData
	{
		return packItems.Find((PackItemData item) => item.GetType().Equals(typeof(T))) as T;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		packItems.ForEach(delegate(PackItemData item)
		{
			sb.AppendLine(item.ToString());
		});
		return string.Format(sb.ToString());
	}

	public string ToParsable()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (PackItemData packItem in packItems)
		{
			stringBuilder.Append(packItem.ToParsable());
			stringBuilder.Append("|");
		}
		stringBuilder.Length--;
		return stringBuilder.ToString();
	}

	public void FillWithPseudoRandom()
	{
		foreach (PackItemData packItem in packItems)
		{
			packItem.FillWithRandom();
		}
	}

	public bool IsValid()
	{
		using (List<PackItemData>.Enumerator enumerator = packItems.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				PackItemData current = enumerator.Current;
				if (!current.IsValid())
				{
					UnityEngine.Debug.LogError($"packitemdata not valid! {current.type}");
				}
				return false;
			}
		}
		return true;
	}
}
