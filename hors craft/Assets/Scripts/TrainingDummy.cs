// DecompilerFi decompiler from Assembly-CSharp.dll class: TrainingDummy
using Common.Managers;
using ItemVInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
	[Serializable]
	public struct DropConfig
	{
		public string[] ids;

		public int[] amounts;
	}

	public bool hasToDrop;

	public int respawnTime = 15;

	public DropConfig randomConfing;

	public DropConfig fixedConfig;

	public int minRandom;

	public int maxRandom;

	public bool dropOnPlayer;

	public Animator animator;

	private Health _health;

	protected Health health => _health ?? (_health = GetComponent<Health>());

	private void Start()
	{
		Health health = this.health;
		health.onDieAction = (Action)Delegate.Combine(health.onDieAction, new Action(StartAnim));
	}

	private void StartAnim()
	{
		animator.SetTrigger("Die");
	}

	private void Drop()
	{
		if (hasToDrop)
		{
			Manager.Get<ItemsManager>().Drop(GetDropListFixed(), (!dropOnPlayer) ? base.transform.position : PlayerGraphic.GetControlledPlayerInstance().transform.position);
			Manager.Get<ItemsManager>().Drop(GetDropListRandom(), (!dropOnPlayer) ? base.transform.position : PlayerGraphic.GetControlledPlayerInstance().transform.position);
			hasToDrop = false;
		}
		base.transform.GetChild(0).gameObject.SetActive(value: false);
		StartCoroutine(Respawn());
	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(respawnTime);
		base.transform.GetChild(0).gameObject.SetActive(value: true);
		this.health.hp = this.health.maxHp;
		Health health = this.health;
		health.onDieAction = (Action)Delegate.Combine(health.onDieAction, new Action(StartAnim));
	}

	private List<ItemsManager.DropConfig> GetDropListRandom()
	{
		List<ItemsManager.DropConfig> list = new List<ItemsManager.DropConfig>();
		List<ItemsManager.DropConfig> list2 = new List<ItemsManager.DropConfig>();
		int num = UnityEngine.Random.Range(minRandom, maxRandom);
		for (int i = 0; i < randomConfing.ids.Length; i++)
		{
			list.Add(new ItemsManager.DropConfig
			{
				amount = randomConfing.amounts[i],
				itemId = randomConfing.ids[i]
			});
		}
		for (int j = 0; j < num; j++)
		{
			list2.Add(list.GetAndRemoveRandomItem());
		}
		return list2;
	}

	private List<ItemsManager.DropConfig> GetDropListFixed()
	{
		return fixedConfig.ids.Select((string t, int i) => new ItemsManager.DropConfig
		{
			amount = fixedConfig.amounts[i],
			itemId = t
		}).ToList();
	}
}
