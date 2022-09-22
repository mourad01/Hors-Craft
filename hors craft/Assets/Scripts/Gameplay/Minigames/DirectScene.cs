// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.DirectScene
using com.ootii.Cameras;
using Common.Managers;
using Gameplay.RhythmicMinigame;
using States;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class DirectScene : InteractiveObject
	{
		private const int MAX_GATHERED_MOBS = 12;

		public GameObject[] gosToHideInGame;

		public AudioClip[] successClips;

		public float gatherMobsRadius = 10f;

		public float gatherMobsDistance = 50f;

		public RuntimeAnimatorController controller;

		private HashSet<HumanMob> mobs;

		private IEnumerator Start()
		{
			yield return new WaitForSecondsRealtime(5f);
			GatherMobs();
			ChangeMobsAIState();
		}

		private void ChangeMobsAIState()
		{
			foreach (HumanMob mob in mobs)
			{
				mob.logic = AnimalMob.AnimalLogic.STAY_IN_PLACE;
				mob.ReconstructBehaviourTree();
			}
		}

		private void GatherMobs()
		{
			mobs = new HashSet<HumanMob>();
			RaycastHit[] array = Physics.BoxCastAll( new Vector3(gatherMobsRadius, gatherMobsRadius, gatherMobsRadius), base.transform.position + base.transform.forward.normalized * gatherMobsRadius / 2f, direction: base.transform.forward, orientation: Quaternion.identity, maxDistance: gatherMobsDistance,  LayerMask.GetMask("Mobs"));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				HumanMob componentInParent = raycastHit.collider.GetComponentInParent<HumanMob>();
				if (componentInParent != null && !mobs.Contains(componentInParent))
				{
					mobs.Add(componentInParent);
				}
			}
			while (mobs.Count > 12)
			{
				HashSet<HumanMob>.Enumerator enumerator = mobs.GetEnumerator();
				if (enumerator.MoveNext())
				{
					mobs.Remove(enumerator.Current);
					continue;
				}
				break;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.PlayInstrumentXTimes);
			PlayerGraphic componentInChildren = CameraController.instance.Anchor.GetComponentInChildren<PlayerGraphic>();
			GatherMobs();
			Transform humanPivot = base.transform.Find("humanPivot");
			CameraController.instance.Anchor.transform.localEulerAngles = Vector3.zero;
			CameraController.instance.Anchor.transform.position = base.transform.position;
			Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
			{
				graphicScene = new BollywoodDanceScene(componentInChildren, controller, humanPivot, mobs, successClips, gosToHideInGame)
			});
		}
	}
}
