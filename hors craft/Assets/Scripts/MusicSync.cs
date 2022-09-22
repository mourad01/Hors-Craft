// DecompilerFi decompiler from Assembly-CSharp.dll class: MusicSync
using Common.Managers;
using States;
using System.Linq;
using UnityEngine;

public class MusicSync : MonoBehaviour
{
	public float range = 10f;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		Collider[] source = Physics.OverlapSphere(base.transform.position, range);
		MusicSync[] source2 = (from c in source
			let ms = c.gameObject.GetComponent<MusicSync>()
			where ms != null
			select ms).ToArray();
		MusicSync[] array = (from m in source2
			let d = Vector3.Distance(m.transform.position, base.transform.position)
			let t = m.GetComponent<PersistentObject>().creationTime
			where m != this && d > 0.5f && d <= range
			orderby t
			select m).ToArray();
		if (array.Length > 0)
		{
			AudioSource component = array[0].GetComponent<AudioSource>();
			if (component.clip == audioSource.clip)
			{
				audioSource.time = component.time;
			}
		}
	}

	private void Update()
	{
		if (Manager.Get<StateMachineManager>().currentState is RhythmicMinigameState)
		{
			audioSource.volume = 0f;
		}
		else
		{
			audioSource.volume = 0.6f;
		}
	}
}
