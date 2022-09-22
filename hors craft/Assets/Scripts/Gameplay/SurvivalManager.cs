// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.SurvivalManager
using Common.Managers;
using States;
using SurvivalMobSpawning;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class SurvivalManager : Manager
	{
		public GameObject[] weapons;

		public GameObject player;

		[HideInInspector]
		public bool playerDead;

		private SurvivalMissionPopups missionPopups;

		private SurvivalPhaseContext survivalPhase;

		protected CombatTimeContext combatTimeContext;

		protected int combatSemaphore;

		public int enemyKilledAmount
		{
			get;
			private set;
		}

		public int shootsFired
		{
			get;
			private set;
		}

		private void Awake()
		{
			SurvivalContextsBroadcaster.instance.Clear();
			InitSpawnersAndSurvivalContextListener();
		}

		private void InitSpawnersAndSurvivalContextListener()
		{
			AbstractMobSpawner[] componentsInChildren = GetComponentsInChildren<AbstractMobSpawner>();
			AbstractMobSpawner[] array = componentsInChildren;
			foreach (AbstractMobSpawner abstractMobSpawner in array)
			{
				abstractMobSpawner.Init(this);
			}
			ISurvivalContextListener[] componentsInChildren2 = GetComponentsInChildren<ISurvivalContextListener>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].AddBySurvivalManager() && (componentsInChildren2[j].ContextTypes() != null || componentsInChildren2[j].OnContextsUpdated() != null))
				{
					if (componentsInChildren2[j].ContextTypes().Length > 1 && componentsInChildren2[j].OnContextsUpdated().Length == 1)
					{
						SurvivalContextsBroadcaster.instance.Register(componentsInChildren2[j].ContextTypes(), componentsInChildren2[j].OnContextsUpdated()[0]);
					}
					else
					{
						SurvivalContextsBroadcaster.instance.Register(componentsInChildren2[j].ContextTypes(), componentsInChildren2[j].OnContextsUpdated());
					}
				}
			}
		}

		public override void Init()
		{
			missionPopups = GetComponent<SurvivalMissionPopups>();
			survivalPhase = new SurvivalPhaseContext
			{
				isCombat = false
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.SURVIVAL_PHASE, survivalPhase);
			MonoBehaviourSingleton<GameplayFacts>.get.AddFactPersistent(Fact.SURVIVAL_MODE_ENABLED);
			if (missionPopups == null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactPersistent(Fact.SURVIVAL_DEFAULT_POPUPS_ALLOWED);
			}
			combatTimeContext = (SurvivalContextsBroadcaster.instance.GetContext<CombatTimeContext>() ?? new CombatTimeContext
			{
				isCombat = false,
				becauseRestarted = true
			});
			combatTimeContext.becauseRestarted = true;
		}

		public bool IsCombatTime()
		{
			return survivalPhase.isCombat;
		}

		public void RegisterPlayer(GameObject player)
		{
			this.player = player;
		}

		public void ReleaseCombatSemaphore()
		{
			combatSemaphore = Mathf.Clamp(combatSemaphore - 1, 0, 100);
			if (combatSemaphore == 0 && survivalPhase.isCombat)
			{
				survivalPhase.isCombat = false;
				DisarmPlayers();
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_PHASE);
				GameplayState.Substates substate = player.GetComponent<PlayerController>().isInVehicle ? GameplayState.Substates.VEHICLE : GameplayState.Substates.WALKING;
				Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().SetSubstate(substate);
				Engine.SaveWorld();
				Manager.Get<GameCallbacksManager>().FrequentSave();
				Manager.Get<GameCallbacksManager>().InFrequentSave();
				DestroyEnemyMobs();
				if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
				{
					ShowDayPhaseChanged();
				}
				combatTimeContext.isCombat = false;
				combatTimeContext.becauseRestarted = false;
				SurvivalContextsBroadcaster.instance.UpdateContext(combatTimeContext);
				combatTimeContext.becauseRestarted = true;
			}
		}

		public void TakeCombatSemaphore()
		{
			combatSemaphore++;
			if (!survivalPhase.isCombat)
			{
				survivalPhase.isCombat = true;
				ArmPlayers();
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_PHASE);
				GameplayState.Substates substate = (!player.GetComponent<PlayerController>().isInVehicle) ? GameplayState.Substates.SURVIVAL_COMBAT : GameplayState.Substates.SURVIVAL_VEHICLE_COMBAT;
				if (player.GetComponent<PlayerController>().isInFlyingVehicle)
				{
					substate = GameplayState.Substates.SURVIVAL_FLYING_VEHICLE_COMBAT;
				}
				(Manager.Get<StateMachineManager>().currentState as GameplayState).SetSubstate(substate);
				if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
				{
					ShowDayPhaseChanged();
				}
				combatTimeContext.isCombat = true;
				combatTimeContext.becauseRestarted = false;
				SurvivalContextsBroadcaster.instance.UpdateContext(combatTimeContext);
				combatTimeContext.becauseRestarted = true;
			}
		}

		protected virtual void ShowDayPhaseChanged()
		{
			if (missionPopups != null)
			{
				missionPopups.DayPhaseChanged();
			}
		}

		public void DestroyEnemyMobs()
		{
			Mob[] array = Object.FindObjectsOfType<Mob>();
			foreach (Mob mob in array)
			{
				IFighting fighting = mob as IFighting;
				if (fighting != null && fighting.IsEnemy())
				{
					UnityEngine.Object.Destroy(mob.gameObject);
				}
			}
		}

		protected void ArmPlayers()
		{
			if (!(player == null))
			{
				ArmedPlayer component = player.GetComponent<ArmedPlayer>();
				if (component != null)
				{
					component.EquipDefaultWeapon();
				}
			}
		}

		protected void DisarmPlayers()
		{
			if (!(player == null))
			{
				ArmedPlayer component = player.GetComponent<ArmedPlayer>();
				if (component != null && !player.gameObject.GetComponent<PlayerController>().isInVehicle)
				{
					component.Disarm();
				}
			}
		}

		public void ShootFired()
		{
			shootsFired++;
		}

		public void EnemyKilled()
		{
			enemyKilledAmount++;
		}
	}
}
