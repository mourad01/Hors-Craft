// DecompilerFi decompiler from Assembly-CSharp.dll class: AmmoObject
using Common.Managers;
using Gameplay;
using Gameplay.Audio;
using UnityEngine;

public class AmmoObject : CoinObject
{
	public AmmoType _ammoType;

	public bool hasFixedAmount;

	public int fixedAmount;

	private AmmoContextGenerator _ammoContextGenerator;

	private AmmoContextGenerator ammoContextGenerator
	{
		get
		{
			if (_ammoContextGenerator == null)
			{
				_ammoContextGenerator = Manager.Get<SurvivalManager>().gameObject.GetComponent<AmmoContextGenerator>();
			}
			return _ammoContextGenerator;
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (!pickedUp && GameObject.FindGameObjectWithTag("Player") == other.gameObject)
		{
			pickedUp = true;
			UnityEngine.Object.Destroy(base.gameObject);
			PlaySound(GameSound.RESOURCE_PICKUP);
			ammoContextGenerator.AddAmmo(_ammoType, (!hasFixedAmount) ? value : fixedAmount);
		}
	}
}
