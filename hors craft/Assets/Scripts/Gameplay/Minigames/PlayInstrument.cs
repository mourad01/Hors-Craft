// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.PlayInstrument
using com.ootii.Cameras;
using Common.Managers;
using Gameplay.RhythmicMinigame;
using States;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class PlayInstrument : InteractiveObject
	{
		private const int MAX_GATHERED_MOBS = 12;

		public GameObject[] gosToHideInGame;

		public AudioClip[] successClips;

		public float gatherMobsRadius = 10f;

		public float gatherMobsDistance = 50f;

		public RuntimeAnimatorController controller;

		public InstrumentConfig[] instrumentConfigs;

		public override void OnUse()
		{
			base.OnUse();
			Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.PlayInstrumentXTimes);
			PlayerGraphic componentInChildren = CameraController.instance.Anchor.GetComponentInChildren<PlayerGraphic>();
			HashSet<HumanMob> mobs = FindMobs();
			mobs = FilterMobs(mobs);
			Transform humanPivot = base.transform.Find("humanPivot");
			Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
			{
				graphicScene = new ShowScene(componentInChildren, instrumentConfigs, controller, humanPivot, mobs, successClips, gosToHideInGame)
			});
			CameraController.instance.Anchor.transform.localEulerAngles = Vector3.zero;
			CameraController.instance.Anchor.transform.position = base.transform.position;
		}

		private HashSet<HumanMob> FindMobs()
		{
			HashSet<HumanMob> hashSet = new HashSet<HumanMob>();
			RaycastHit[] array = Physics.BoxCastAll(new Vector3(gatherMobsRadius, gatherMobsRadius, gatherMobsRadius), base.transform.position, direction: base.transform.forward, orientation: Quaternion.identity, maxDistance: gatherMobsDistance,  LayerMask.GetMask("Mobs"));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				HumanMob componentInParent = raycastHit.collider.GetComponentInParent<HumanMob>();
				if (componentInParent != null && !hashSet.Contains(componentInParent))
				{
					hashSet.Add(componentInParent);
				}
			}
			return hashSet;
		}

		private HashSet<HumanMob> FilterMobs(HashSet<HumanMob> mobs)
		{
			List<HumanMob> list = mobs.ToList();
			Dictionary<HumanMob, float> distances = new Dictionary<HumanMob, float>();
			foreach (HumanMob item in list)
			{
				distances.Add(item, Vector3.Distance(item.transform.position, base.transform.position));
			}
			list.Sort((HumanMob a, HumanMob b) => (distances[a] - distances[b] > 0f) ? 1 : (-1));
			return new HashSet<HumanMob>(list.Take(12));
		}
	}
}
