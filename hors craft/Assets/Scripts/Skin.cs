// DecompilerFi decompiler from Assembly-CSharp.dll class: Skin
using System;
using UnityEngine;

[Serializable]
public class Skin
{
	public enum Gender
	{
		MALE,
		FEMALE,
		TRANS
	}

	public int id;

	public Texture2D texture;

	public Gender gender;

	public Skin(int id, Texture2D texture, Gender gender = Gender.MALE)
	{
		this.id = id;
		this.texture = texture;
		this.gender = gender;
	}

	public Skin(Skin other, int id)
	{
		this.id = id;
		texture = other.texture;
		gender = other.gender;
	}
}
