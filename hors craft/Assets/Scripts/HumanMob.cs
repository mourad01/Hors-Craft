// DecompilerFi decompiler from Assembly-CSharp.dll class: HumanMob
using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(HumanMobNavigator))]
public class HumanMob : AnimalMob
{
	public class HumanParameters : InitializeParameter
	{
		public float random;

		public HumanParameters(float random)
		{
			this.random = random;
		}
	}

	private Pettable _pettable;

	[HideInInspector]
	public int skinIndex;

	public bool hasToSetGraphic = true;

	public Pettable pettable
	{
		get
		{
			if (_pettable == null)
			{
				_pettable = GetComponent<Pettable>();
			}
			return _pettable;
		}
	}

	public virtual Skin.Gender currentGender => SkinList.instance.possibleSkins[skinIndex].gender;

	protected override void Start()
	{
		base.Start();
		if (base.animator == null)
		{
			base.animator = GetComponentInChildren<Animator>();
		}
	}

	public override void Init(InitializeParameter parameter)
	{
		base.Init(parameter);
		int skin = FindRandomSkin((parameter as HumanParameters).random);
		if (hasToSetGraphic)
		{
			SetSkin(skin);
		}
	}

	public void SetSkin(Skin.Gender gender)
	{
		List<int> array = (from s in SkinList.instance.possibleSkins
			where s.gender == gender
			select SkinList.instance.possibleSkins.IndexOf(s)).ToList();
		SetSkin(array.Random());
	}

	public void SetSkin(int index)
	{
		PlayerGraphic componentInChildren = GetComponentInChildren<PlayerGraphic>();
		if (!(componentInChildren == null))
		{
			componentInChildren.SetWholeBodyl(index);
			skinIndex = index;
		}
	}

	private int FindRandomSkin(float random)
	{
		float num = random * SkinList.instance.sumOfWeights;
		float num2 = 0f;
		int num3 = -1;
		while (num3 < SkinList.instance.possibleSkins.Count - 1 && num2 <= num)
		{
			num3++;
			num2 += SkinList.instance.genderProbabilities[SkinList.instance.possibleSkins[num3].gender];
		}
		if (num3 < SkinList.instance.possibleSkins.Count - 1)
		{
			return num3;
		}
		return SkinList.instance.possibleSkins.Count - 1;
	}
}
