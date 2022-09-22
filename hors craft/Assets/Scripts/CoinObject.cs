// DecompilerFi decompiler from Assembly-CSharp.dll class: CoinObject
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using UnityEngine;

public class CoinObject : MonoBehaviour
{
	public float forceMultiplier = 250f;

	private int resourceId;

	private ushort craftableId;

	private int myId;

	protected bool pickedUp;

	protected int value;

	public void Init(int value, Vector3 position, bool addRandomForce = false)
	{
		this.value = value;
		base.transform.position = position;
		if (addRandomForce)
		{
			AddRandomForce();
		}
	}

	private void AddRandomForce()
	{
		GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), 1f, UnityEngine.Random.Range(-0.4f, 0.4f)) * forceMultiplier);
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (!pickedUp && GameObject.FindGameObjectWithTag("Player") == other.gameObject)
		{
			pickedUp = true;
			UnityEngine.Object.Destroy(base.gameObject);
			PlaySound(GameSound.RESOURCE_PICKUP);
			if (Manager.Contains<AbstractSoftCurrencyManager>())
			{
				Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(value);
			}
		}
	}

	protected void PlaySound(GameSound clip)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(clip);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		Sound sound2 = sound;
		sound2.Play();
	}
}
