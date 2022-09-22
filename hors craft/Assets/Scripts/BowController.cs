// DecompilerFi decompiler from Assembly-CSharp.dll class: BowController
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System;
using UnityEngine;

public class BowController : Weapon
{
	[Serializable]
	public class BowPullState
	{
		public float atTime;

		public GameObject bowPrefab;

		public float additionalForce;
	}

	public GameObject[] arrowPrefabs;

	public float forceBase = 10f;

	public BowPullState[] pullStates;

	private bool isPressed;

	private GameObject bow;

	private int state;

	private float pullTime;

	private GameObject arrow;

	private int arrowsIndex;

	private ushort arrowsVoxelId;

	private Inventory foundInventory;

	private Inventory inventory
	{
		get
		{
			if (foundInventory == null)
			{
				foundInventory = owner.GetComponent<Inventory>();
			}
			return foundInventory;
		}
	}

	public bool IsShooting => isPressed;

	private void Start()
	{
		isPressed = false;
		pullTime = 0f;
		state = 0;
		bow = UnityEngine.Object.Instantiate(pullStates[state].bowPrefab, base.transform.position, base.transform.rotation);
		bow.transform.SetParent(base.transform);
		SetDefaultArrows();
	}

	public override void OnPress()
	{
		int quantity = inventory.GetQuantity(arrowsVoxelId);
		if (!isPressed && quantity > 0)
		{
			arrow = UnityEngine.Object.Instantiate(arrowPrefabs[arrowsIndex], base.transform.position, base.transform.rotation);
			arrow.transform.SetParent(base.transform);
			isPressed = true;
			inventory.RemoveItem(arrowsVoxelId);
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.BOW_RELOAD);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}

	public override void OnRelease()
	{
		if (isPressed)
		{
			Arrow component = arrow.GetComponent<Arrow>();
			component.damage += damage;
			component.translation = CameraController.instance.MainCamera.transform.position - arrow.transform.position;
			component.Fire(forceBase + pullStates[state].additionalForce);
			pullTime = 0f;
			state = 0;
			ChangeBow(pullStates[state].bowPrefab);
			isPressed = false;
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.BOW_SHOOT);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}

	private void Update()
	{
		if (isPressed)
		{
			pullTime += Time.deltaTime;
			if (state + 1 < pullStates.Length && pullTime >= pullStates[state + 1].atTime)
			{
				state++;
				ChangeBow(pullStates[state].bowPrefab);
				AdjustArrowPosition();
			}
		}
	}

	private void ChangeBow(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, bow.transform.position, bow.transform.rotation);
		gameObject.transform.SetParent(bow.transform.parent);
		UnityEngine.Object.Destroy(bow);
		bow = gameObject;
	}

	private void AdjustArrowPosition()
	{
		Transform transform = bow.transform.FindChildRecursively("Arrow Spot");
		arrow.transform.position = transform.position;
		arrow.transform.rotation = transform.rotation;
	}

	private void SetDefaultArrows()
	{
		int i;
		for (i = 0; i < arrowPrefabs.Length && inventory.GetQuantity(arrowPrefabs[i].GetComponent<Arrow>().voxelId) <= 0; i++)
		{
		}
		if (i >= arrowPrefabs.Length)
		{
			i = 0;
		}
		arrowsIndex = i;
		arrowsVoxelId = arrowPrefabs[i].GetComponent<Arrow>().voxelId;
		inventory.SetCurrentItem(arrowsVoxelId);
	}
}
