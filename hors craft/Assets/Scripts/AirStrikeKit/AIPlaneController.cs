// DecompilerFi decompiler from Assembly-CSharp.dll class: AirStrikeKit.AIPlaneController
using Common.BehaviourTrees;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System;
using System.Collections;
using UnityEngine;

namespace AirStrikeKit
{
	[RequireComponent(typeof(FlightSystem))]
	public class AIPlaneController : EnemyMob
	{
		public GameObject deathAnimation;

		public GameObject barrel;

		public GameObject shootTrailPrefab;

		public AudioClip shootClip;

		public float chanceToHit = 0.1f;

		public string[] TargetTag;

		public GameObject Target;

		public float TimeToLock;

		public float AttackDirection = 0.5f;

		public float attackAngle = 30f;

		public float DistanceLock = float.MaxValue;

		public float DistanceAttack = 300f;

		public Vector3 BattlePosition;

		public GameObject CenterOfBattle;

		public float FlyDistance = 1000f;

		public AIState AIstate = AIState.Patrol;

		[HideInInspector]
		public int WeaponSelected;

		public int AttackRate = 80;

		private float timestatetemp;

		private float timetolockcount;

		private FlightSystem flight;

		private bool attacking;

		private Vector3 directionTurn;

		private TargetBehavior targetHavior;

		private Vector3 targetpositionTemp;

		public Action onDie;

		public bool Avoidance = true;

		public float AvoidChackDistance = 100f;

		public float AvoidCheckOffset = 10f;

		private float dot;

		public float projectileSpread = 20f;

		private LineRenderer _shootTrails;

		private LineRenderer shootTrails
		{
			get
			{
				if (_shootTrails == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(shootTrailPrefab);
					_shootTrails = gameObject.GetComponent<LineRenderer>();
				}
				return _shootTrails;
			}
		}

		protected override void Start()
		{
			base.Start();
			timetolockcount = Time.time;
			flight = GetComponent<FlightSystem>();
			flight.AutoPilot = true;
			timestatetemp = 0f;
			base.navigator.enabled = false;
			base.transform.position = base.transform.position + Vector3.up * 100f;
		}

		protected override ParallelNode ConstructTopParallelNode()
		{
			ParallelNode parallelNode = new ParallelNode();
			SelectorNode selectorNode = new SelectorNode();
			selectorNode.Add(ConstructDieBehaviour());
			parallelNode.Add(selectorNode);
			return parallelNode;
		}

		protected override Node ConstructDieBehaviour()
		{
			SequenceNode sequenceNode = new SequenceNode();
			sequenceNode.Add(new IsDeadNode(this, GetComponent<Health>()));
			SelectorNode selectorNode = new SelectorNode();
			selectorNode.Add(new HasAlreadyDiedNode(this));
			selectorNode.Add(new DieNode(this));
			sequenceNode.Add(selectorNode);
			return sequenceNode;
		}

		private void TargetBehaviorCal()
		{
			if ((bool)Target)
			{
				Vector3 lhs = targetpositionTemp - Target.transform.position;
				float y = targetpositionTemp.y;
				Vector3 position = Target.transform.position;
				float num = Mathf.Abs(y - position.y);
				targetpositionTemp = Target.transform.position;
				if (lhs == Vector3.zero)
				{
					targetHavior = TargetBehavior.Static;
				}
				else
				{
					targetHavior = TargetBehavior.Moving;
					if (num > 0.5f)
					{
						targetHavior = TargetBehavior.Flying;
					}
				}
			}
			if ((bool)CenterOfBattle && flight.PositionTarget.y < BattlePosition.y)
			{
				flight.PositionTarget.y = BattlePosition.y;
			}
		}

		protected new void Update()
		{
			if (!flight || Time.timeScale == 0f)
			{
				return;
			}
			if ((bool)CenterOfBattle)
			{
				BattlePosition = CenterOfBattle.gameObject.transform.position;
			}
			else
			{
				FindBattleCenter();
			}
			if (CenterOfBattle != null)
			{
				BattlePosition = CenterOfBattle.gameObject.transform.position;
			}
			TargetBehaviorCal();
			switch (AIstate)
			{
			case AIState.Patrol:
				FindTarget();
				break;
			case AIState.Idle:
				if (Vector3.Distance(flight.PositionTarget, base.transform.position) <= FlyDistance)
				{
					AIstate = AIState.Patrol;
					timestatetemp = Time.time;
				}
				break;
			case AIState.Attacking:
				if ((bool)Target)
				{
					flight.PositionTarget = Target.transform.position;
					if (!shootTarget(flight.PositionTarget))
					{
						if (attacking)
						{
							if (Time.time > timestatetemp + 5f)
							{
								turnPosition();
							}
						}
						else if (Time.time > timestatetemp + 7f)
						{
							turnPosition();
						}
					}
				}
				else
				{
					AIstate = AIState.Patrol;
					timestatetemp = Time.time;
				}
				if (Vector3.Distance(BattlePosition, base.transform.position) > FlyDistance)
				{
					gotoCenter();
				}
				break;
			case AIState.TurnPosition:
			{
				if (Time.time > timestatetemp + 7f)
				{
					timestatetemp = Time.time;
					AIstate = AIState.Attacking;
				}
				if (Vector3.Distance(BattlePosition, base.transform.position) > FlyDistance)
				{
					gotoCenter();
				}
				float y = flight.PositionTarget.y;
				if (targetHavior == TargetBehavior.Static)
				{
					directionTurn.y = 0f;
					flight.PositionTarget += (base.transform.forward + directionTurn) * flight.Speed;
					flight.PositionTarget.y = y;
					flight.PositionTarget.y += flight.Speed / 2f;
				}
				else
				{
					flight.PositionTarget += (base.transform.forward + directionTurn) * flight.Speed;
					flight.PositionTarget.y = y;
					flight.PositionTarget.y += flight.Speed / 2f;
					flight.PositionTarget = BattlePosition + new Vector3(UnityEngine.Random.Range(0f - FlyDistance, FlyDistance), UnityEngine.Random.Range(0f, FlyDistance / 2f), UnityEngine.Random.Range(0f - FlyDistance, FlyDistance));
				}
				break;
			}
			}
			if (Avoidance)
			{
				Vector3 lhs = avoidDirection();
				if (lhs != Vector3.zero)
				{
					flight.FollowTarget = true;
					flight.PositionTarget = base.transform.position + lhs.normalized * AvoidChackDistance;
					UnityEngine.Debug.DrawLine(base.transform.position, flight.PositionTarget, Color.blue);
				}
			}
		}

		private void FindTarget()
		{
			for (int i = 0; i < TargetTag.Length; i++)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
				if (array.Length <= 0)
				{
					continue;
				}
				float num = 2.14748365E+09f;
				for (int j = 0; j < array.Length; j++)
				{
					if (!array[j])
					{
						continue;
					}
					if (timetolockcount + TimeToLock < Time.time)
					{
						bool flag = false;
						if (array[j].GetComponent<PlayerController>() != null)
						{
							flag = true;
						}
						float num2 = Vector3.Distance(array[j].transform.position, base.transform.position);
						if (flag)
						{
							num2 = Mathf.Min(num2, 300f);
						}
						if (DistanceLock > num2)
						{
							if (Target != null && Target.gameObject.GetComponent<PlayerController>() != null)
							{
								timestatetemp = Time.time;
								flight.FollowTarget = true;
								AIstate = AIState.Attacking;
								return;
							}
							if (num > num2 && (UnityEngine.Random.Range(0, 100) > 80 || flag))
							{
								num = num2;
								Target = array[j];
								flight.FollowTarget = true;
								AIstate = AIState.Attacking;
								timestatetemp = Time.time;
								if (flag && UnityEngine.Random.value > 0.15f)
								{
									return;
								}
							}
						}
					}
					shootTarget(array[j].transform.position);
				}
			}
		}

		private Vector3 avoidDirection()
		{
			Vector3[] array = new Vector3[5]
			{
				base.transform.forward + base.transform.up,
				base.transform.forward - base.transform.up,
				base.transform.forward,
				base.transform.forward + base.transform.right,
				base.transform.forward - base.transform.right
			};
			bool flag = false;
			RaycastHit hitInfo;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Debug.DrawLine(base.transform.position + base.transform.forward * AvoidCheckOffset, base.transform.position + array[i].normalized * AvoidChackDistance, Color.white);
				if (Physics.Raycast(base.transform.position + base.transform.forward * AvoidCheckOffset, array[i].normalized, out hitInfo, AvoidChackDistance))
				{
					UnityEngine.Debug.DrawLine(base.transform.position + base.transform.forward * AvoidCheckOffset, base.transform.position + array[i].normalized * AvoidChackDistance, Color.red);
					flag = true;
					if (i == 1)
					{
						return array[i];
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (!Physics.Raycast(base.transform.position + base.transform.forward * AvoidCheckOffset, array[j].normalized, out hitInfo, AvoidChackDistance))
					{
						UnityEngine.Debug.DrawLine(base.transform.position + base.transform.forward * AvoidCheckOffset, base.transform.position + array[j].normalized * AvoidChackDistance, Color.green);
						return array[j];
					}
				}
				return -base.transform.forward;
			}
			return Vector3.zero;
		}

		private bool shootTarget(Vector3 targetPos)
		{
			Vector3 normalized = (targetPos - base.transform.position).normalized;
			dot = Vector3.Dot(normalized, base.transform.forward);
			float num = Vector3.Distance(targetPos, base.transform.position);
			if (num <= DistanceAttack)
			{
				if (!(dot >= AttackDirection) || !(Vector3.Angle(base.transform.forward, targetPos - base.transform.position) < attackAngle))
				{
					return false;
				}
				attacking = true;
				if (UnityEngine.Random.Range(0, 100) <= AttackRate)
				{
					Shoot(Target);
				}
				if (num < DistanceAttack / 3f)
				{
					turnPosition();
				}
			}
			else
			{
				flight.SpeedUp();
			}
			return true;
		}

		private void turnPosition()
		{
			directionTurn = new Vector3(UnityEngine.Random.Range(-2, 1) + 1, UnityEngine.Random.Range(-2, 1) + 1, UnityEngine.Random.Range(-2, 1) + 1);
			AIstate = AIState.TurnPosition;
			timestatetemp = Time.time;
			attacking = false;
		}

		private void gotoCenter()
		{
			flight.PositionTarget = BattlePosition;
		}

		private void FindBattleCenter()
		{
			if (!CenterOfBattle && (bool)PlayerGraphic.GetControlledPlayerInstance())
			{
				GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
				if (gameObject != null)
				{
					CenterOfBattle = gameObject;
				}
			}
		}

		public override void Die()
		{
			if (onDie != null)
			{
				onDie();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(deathAnimation);
			gameObject.transform.position = base.transform.position;
			base.Die();
		}

		private void Shoot(GameObject target)
		{
			if (chanceToHit >= UnityEngine.Random.Range(0f, 1f))
			{
				DealDamage(target);
			}
			PlaySound(shootClip);
			Vector3 normalized = (target.transform.position - barrel.transform.position).normalized;
			normalized = Vector3.Lerp(normalized, UnityEngine.Random.insideUnitSphere, projectileSpread / 360f);
			StartCoroutine(ShootTrail(barrel.transform.position, normalized, barrel.transform.position + base.transform.forward * DistanceAttack + UnityEngine.Random.insideUnitSphere * projectileSpread));
		}

		private void PlaySound(AudioClip clip)
		{
			if (!(clip == null))
			{
				Sound sound = new Sound();
				sound.clip = clip;
				sound.mixerGroup = Manager.Get<MixersManager>().uiMixerGroup;
				Sound sound2 = sound;
				sound2.Play();
			}
		}

		private void DealDamage(GameObject target)
		{
			Health[] componentsInParent = target.GetComponentsInParent<Health>();
			int num = componentsInParent.Length;
			if (componentsInParent == null)
			{
				return;
			}
			bool flag = componentsInParent[num - 1].hp > 0f;
			componentsInParent[num - 1].OnHit(dmg, target.transform.position - base.transform.position);
			if (flag && componentsInParent[num - 1].hp <= 0f)
			{
				IFighting componentInParent = target.GetComponentInParent<IFighting>();
				if (componentInParent != null && componentInParent.IsEnemy())
				{
					Manager.Get<QuestManager>().OnEnemyKilled();
				}
			}
		}

		protected IEnumerator ShootTrail(Vector3 fr, Vector3 forward, Vector3 to)
		{
			shootTrails.enabled = true;
			int count = Mathf.Max(2, Mathf.CeilToInt((to - fr).magnitude) / 10);
			shootTrails.positionCount = count;
			for (int j = 0; j < count; j++)
			{
				shootTrails.SetPosition(j, Vector3.Lerp(fr, to, (float)(j + 1) / (float)count));
			}
			shootTrails.SetPosition(count - 1, to - forward * 0.25f);
			Color c3 = Color.red;
			Color c2 = new Color32(byte.MaxValue, 129, 6, byte.MaxValue);
			for (int i = 0; i < 15; i++)
			{
				c3.a = (c2.a = 1f - (float)(i + 1) * (71f / (339f * (float)Math.PI)));
				_shootTrails.startColor = c3;
				_shootTrails.endColor = c2;
				yield return new WaitForFixedUpdate();
			}
			shootTrails.enabled = false;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			if (_shootTrails != null)
			{
				_shootTrails.enabled = false;
				UnityEngine.Object.Destroy(_shootTrails.gameObject);
			}
		}
	}
}
