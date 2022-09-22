// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupClothesSaveLoad
using System;
using System.Collections;
using UnityEngine;

public class DressupClothesSaveLoad
{
	private string CreateKey(DressupSkin.Placement placement)
	{
		return "dressup.saved" + placement.ToString();
	}

	private string CreateKey(BodyPart placement)
	{
		return "dressup.saved" + placement.ToString();
	}

	public void SaveSkin(DressupSkin.Placement placement, int index)
	{
		PlayerPrefs.SetInt(CreateKey(placement), index);
		PlayerPrefs.Save();
	}

	public void SaveSkin(BodyPart placement, int index)
	{
		PlayerPrefs.SetInt(CreateKey(placement), index);
		PlayerPrefs.Save();
	}

	public int GetSkinIndex(DressupSkin.Placement placement)
	{
		return PlayerPrefs.GetInt(CreateKey(placement), -1);
	}

	public int GetSkinIndex(BodyPart placement)
	{
		return PlayerPrefs.GetInt(CreateKey(placement), 0);
	}

	public int[] GetAllSkinIndexes()
	{
		int[] array = new int[7];
		int num = 0;
		IEnumerator enumerator = Enum.GetValues(typeof(DressupSkin.Placement)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				array[num] = GetSkinIndex((DressupSkin.Placement)current);
				num++;
			}
			return array;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public bool IsSelected(int id)
	{
		int[] allSkinIndexes = GetAllSkinIndexes();
		foreach (int num in allSkinIndexes)
		{
			if (id == num)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSelected(int id, BodyPart part)
	{
		if (part == BodyPart.Body && id == GetSkinIndex(BodyPart.Body))
		{
			return true;
		}
		if (part == BodyPart.Legs && id == GetSkinIndex(BodyPart.Legs))
		{
			return true;
		}
		return false;
	}
}
