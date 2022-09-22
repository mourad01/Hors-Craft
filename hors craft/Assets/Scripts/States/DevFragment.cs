// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DevFragment
using com.ootii.Cameras;
using com.ootii.Input;
using Common.ImageEffects;
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DevFragment : Fragment
	{
		public GameObject buttonGO;

		public Transform buttonParent;

		private void Awake()
		{
			AddButton(OnDebugTestWorld, "Test World");
			AddButton(OnDebugUnlockAll, "Unlock All");
			AddButton(OnDebugEditMode, "Edit Mode");
			AddButton(OnDebugAutoSave, "Auto Save " + ((!Manager.Contains<AutoSavingManager>() || !Manager.Get<AutoSavingManager>().enabled) ? "OFF" : "ON"));
			AddButton(OnDebugFastMovement, "Fast Move " + PlayerPrefs.GetFloat("fastMovement", 1f) + "x");
			AddButton(OnDebugDisableAds, "Disable Ads");
			AddButton(OnDebugFpsCounter, GetDebugFpsCounterText(PlayerPrefs.GetInt("debugFpsCounter", 0)));
			AddButton(OnDebugCrafting, "Unlock crafting");
			AddButton(OnDebugEnableColliders, "Enable Colliders " + ((GameObject.FindGameObjectWithTag("Player").layer != 17) ? "ON" : "OFF"));
			AddButton(OnDebugRateUs, "Rate us");
			AddButton(OnAddResources, "Add resources");
			AddButton(OnSpawnAnimals, "Spawn animals");
			AddButton(PrintFacts, "Print facts " + ((PlayerPrefs.GetInt("print.facts", 0) != 1) ? "OFF" : "ON"));
			if (Manager.Contains<AdventureQuestManager>() || Manager.Contains<DailyEventManager>())
			{
				AddButton(OnSwitchLevel, "Switch level");
			}
			if (Manager.Get<ModelManager>().adsFree.IsWatchAdsToRemoveAdsEnabled())
			{
				AddButton(OnDebugWatchOneAdInsteadOfTen, "One ad for ten");
			}
			if (Manager.Contains<AbstractSoftCurrencyManager>())
			{
				AddButton(OnAddCurrency, "Add geld (20k)");
			}
			if (Manager.Contains<HardCurrencyManager>())
			{
				AddButton(OnAddHardCurrency, "Add hard currency");
			}
			if (Manager.Contains<FishingManager>())
			{
				AddButton(OnDebugFishing, "Debug Fishing " + ((!Manager.Get<FishingManager>().debugMode) ? "Off" : "On"));
			}
			if (Manager.Contains<DynamicOfferPackManager>())
			{
				AddButton(OnShowDynamicOfferPackClicked, "Show Dynamic Offer");
				AddButton(OnForceDynamicOfferPackEnabledClicked, "Force Enable Offerpacks " + ((!Manager.Get<DynamicOfferPackManager>().forceDynamicStarterPacksEnabled) ? "OFF" : "ON"));
			}
			AddButton(OnSwitchSteeringType, "Steering " + ((!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING)) ? "Normal" : "MCPE"));
			AddButton(OnSwitchTouchSensitivity, GetTouchSensitivityText());
			if (Manager.Contains<ProgressManager>())
			{
				AddButton(AddManyExp, "Add exp");
			}
			AddButton(SetNot, "Set notification");
			AddButton(CancelNot, "Cancel notification");
		}

		private void AddButton(Action<GameObject> onClick, string text)
		{
			GameObject instance = UnityEngine.Object.Instantiate(buttonGO);
			instance.transform.SetParent(buttonParent, worldPositionStays: false);
			instance.SetActive(value: true);
			instance.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				onClick(instance);
			});
			instance.GetComponentInChildren<Text>().text = text;
		}

		private void OnDebugEditMode(GameObject go)
		{
			Engine.EditMode = true;
			((UnityInputSource)CameraController.instance.InputSource).ViewActivator = 4;
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.MCPE_STEERING);
		}

		private void OnDebugTestWorld(GameObject go)
		{
			ChunkData.testWorld = true;
			string id = Manager.Get<SavedWorldManager>().AddNewUserWorld("test");
			Manager.Get<SavedWorldManager>().TryToSelectWorldById(id);
			SavedWorldManager.ResetCurrentWorld();
		}

		private void OnDebugUnlockAll(GameObject go)
		{
			PlayerPrefs.SetInt("numberOfWatchedRewardedAds", 99999);
			PlayerPrefs.SetInt("numberOfWatchedRewardedAdsBlocks", 99999);
			PlayerPrefs.SetInt("numberOfWatchedRewardedAdsClothes", 99999);
			PlayerPrefs.SetInt("numberOfWatchedRewardedAdsPets", 99999);
			IEnumerator enumerator = Enum.GetValues(typeof(Voxel.Category)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerPrefs.SetInt("numberOfWatchedRewardedAds" + ((Voxel.Category)enumerator.Current).ToString().ToLower(), 99999);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			foreach (Voxel.RarityCategory rarityCategory in Singleton<BlocksController>.get.GetRarityCategories())
			{
				PlayerPrefs.SetInt("Max.blocks.Unlimited." + rarityCategory.ToString(), 1);
				foreach (Voxel item in Singleton<BlocksController>.get.GetVoxelsForRarityCategory(rarityCategory))
				{
					Singleton<BlocksController>.get.UnlockBlock(item);
				}
			}
			PlayerPrefs.SetInt("levelsFinished", 99999);
			go.GetComponentInChildren<Text>().text = "Unlocked";
			Manager.Get<PetManager>().UnlockAll();
		}

		private void OnDebugAutoSave(GameObject go)
		{
			bool flag = !Manager.Get<AutoSavingManager>().enabled;
			Manager.Get<AutoSavingManager>().enabled = flag;
			go.GetComponentInChildren<Text>().text = "AutoSave " + ((!flag) ? "OFF" : "ON");
		}

		private void OnAddCurrency(GameObject go)
		{
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(20000);
		}

		private void OnAddHardCurrency(GameObject go)
		{
			Manager.Get<HardCurrencyManager>().OnCurrencyAmountChange(20000);
		}

		private void OnDebugFastMovement(GameObject go)
		{
			float current = PlayerPrefs.GetFloat("fastMovement", 1f);
			List<float> list = new List<float>();
			list.Add(1f);
			list.Add(3f);
			list.Add(10f);
			list.Add(16f);
			List<float> list2 = list;
			current = list2[(list2.FindIndex((float s) => s == current) + 1) % list2.Count];
			PlayerPrefs.SetFloat("fastMovement", current);
			go.GetComponentInChildren<Text>().text = "FastMove " + current + "x";
		}

		private void OnDebugDisableAds(GameObject go)
		{
			bool flag = !Manager.Get<ModelManager>().modulesContext.isAdsFree;
			PlayerPrefs.SetInt("debugAdsFree", flag ? 1 : 0);
			Manager.Get<ModelManager>().modulesContext.isAdsFree = flag;
			go.GetComponentInChildren<Text>().text = "AdsFree " + ((!flag) ? "OFF" : "ON");
		}

		private void OnDebugFpsCounter(GameObject go)
		{
			int @int = PlayerPrefs.GetInt("debugFpsCounter", 0);
			@int = (@int + 1) % 6;
			PlayerPrefs.SetInt("debugFpsCounter", @int);
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.FPS_ENABLED, @int > 0);
			go.GetComponentInChildren<Text>().text = GetDebugFpsCounterText(@int);
			switch (@int)
			{
			case 2:
				MonoBehaviourSingleton<FrameLatencyStats>.get.SetActiveCounters(new List<FrameLatencyStats.Counter>
				{
					FrameLatencyStats.Counter.FRAME,
					FrameLatencyStats.Counter.UPDATE,
					FrameLatencyStats.Counter.INITIALIZATIONS,
					FrameLatencyStats.Counter.RENDER_WHOLE,
					FrameLatencyStats.Counter.ON_GUI,
					FrameLatencyStats.Counter.PHYSICS
				});
				break;
			case 5:
				MonoBehaviourSingleton<FrameLatencyStats>.get.SetActiveCounters(new List<FrameLatencyStats.Counter>
				{
					FrameLatencyStats.Counter.RENDER_WHOLE,
					FrameLatencyStats.Counter.RENDER,
					FrameLatencyStats.Counter.CULLING,
					FrameLatencyStats.Counter.POST_PROCESS,
					FrameLatencyStats.Counter.FRAME
				});
				break;
			}
		}

		private string GetDebugFpsCounterText(int current)
		{
			switch (current)
			{
			case 0:
				return "FpsCounter - OFF";
			case 1:
				return "FpsCounter - Simple";
			case 2:
				return "FpsCounter - Advanced";
			case 3:
				return "Memory - Simple";
			case 4:
				return "Memory - Advanced";
			case 5:
				return "Rendering process";
			default:
				return "???";
			}
		}

		private void OnDebugWatchOneAdInsteadOfTen(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("debugOneAdForTen", 0) + 1) % 2;
			PlayerPrefs.SetInt("debugOneAdForTen", num);
			go.GetComponentInChildren<Text>().text = "OneAdForTen " + ((num != 1) ? "OFF" : "ON");
		}

		private void OnDebugCrafting(GameObject go)
		{
			foreach (Resource resources in Manager.Get<CraftingManager>().GetResourcesList())
			{
				Singleton<PlayerData>.get.playerItems.AddToResources(resources.id, 1000);
			}
			foreach (Craftable craftable in Manager.Get<CraftingManager>().GetCraftableList())
			{
				Singleton<PlayerData>.get.playerItems.AddToBlocks(craftable.id, 99);
			}
			foreach (Quest quest in Manager.Get<QuestManager>().GetQuestList())
			{
				Singleton<PlayerData>.get.playerQuests.OnQuestFinish(quest.id);
			}
			go.GetComponentInChildren<Text>().text = "Unlocked";
		}

		private void OnAddResources(GameObject go)
		{
			foreach (Resource resources in Manager.Get<CraftingManager>().GetResourcesList())
			{
				Singleton<PlayerData>.get.playerItems.AddToResources(resources.id, 1000);
			}
			go.GetComponentInChildren<Text>().text = "Added";
		}

		private void OnDebugEnableColliders(GameObject go)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject != null)
			{
				int layer = gameObject.layer;
				bool flag = false;
				if (layer == 17)
				{
					flag = true;
					gameObject.SetLayerRecursively(2);
				}
				else
				{
					flag = false;
					gameObject.SetLayerRecursively(17);
				}
				go.GetComponentInChildren<Text>().text = "Enable colliders " + ((!flag) ? "OFF" : "ON");
			}
		}

		private void OnDebugRateUs(GameObject go)
		{
			Manager.Get<StateMachineManager>().PushState<RateUsState>();
		}

		private void OnDebugFishing(GameObject go)
		{
			FishingManager fishingManager = Manager.Get<FishingManager>();
			if (fishingManager.debugMode)
			{
				fishingManager.debugMode = false;
				go.GetComponentInChildren<Text>().text = "Debug Fishing Off";
			}
			else
			{
				fishingManager.debugMode = true;
				go.GetComponentInChildren<Text>().text = "Debug Fishing On";
			}
		}

		private void AddManyExp(GameObject go)
		{
			ProgressManager progressManager = Manager.Get<ProgressManager>();
			progressManager.IncreaseExperience(10000);
		}

		private void OnSpawnAnimals(GameObject go)
		{
			MobsManager.MobSpawnConfig[] spawnConfigs = Manager.Get<MobsManager>().spawnConfigs;
			PrintMobConfigsToLog(spawnConfigs);
			Transform transform = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>().transform;
			Vector3 a = transform.position + transform.forward * 5f;
			MobsManager.MobSpawnConfig[] array = spawnConfigs;
			foreach (MobsManager.MobSpawnConfig mobSpawnConfig in array)
			{
				if (mobSpawnConfig.weight > 0f)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(mobSpawnConfig.prefab);
					gameObject.transform.position = a + UnityEngine.Random.insideUnitSphere * 5f;
				}
			}
		}

		private void OnSwitchLevel(GameObject go)
		{
			Manager.Get<StateMachineManager>().PushState<ChooseWorldState>();
		}

		private void OnShowDynamicOfferPackClicked(GameObject go)
		{
			Manager.Get<DynamicOfferPackManager>().DebugShowDynamicStarterPackState();
		}

		private void OnForceDynamicOfferPackEnabledClicked(GameObject go)
		{
			DynamicOfferPackManager dynamicOfferPackManager = Manager.Get<DynamicOfferPackManager>();
			if (dynamicOfferPackManager.forceDynamicStarterPacksEnabled)
			{
				dynamicOfferPackManager.forceDynamicStarterPacksEnabled = false;
				go.GetComponentInChildren<Text>().text = "Force Enable Offerpacks OFF";
			}
			else
			{
				dynamicOfferPackManager.forceDynamicStarterPacksEnabled = true;
				go.GetComponentInChildren<Text>().text = "Force Enable Offerpacks ON";
			}
		}

		private void OnSwitchSteeringType(GameObject go)
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.MCPE_STEERING);
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MCPE_STEERING);
			}
			go.GetComponentInChildren<Text>().text = "Steering " + ((!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING)) ? "Normal" : "MCPE");
		}

		private string GetTouchSensitivityText()
		{
			string str = "Touch sensitivity: ";
			if (!(CameraController.instance.InputSource is TouchInputSource))
			{
				return str + "error";
			}
			TouchInputSource touchInputSource = (TouchInputSource)CameraController.instance.InputSource;
			return str + touchInputSource.touchSensitivity.ToString();
		}

		private void OnSwitchTouchSensitivity(GameObject go)
		{
			if (CameraController.instance.InputSource is TouchInputSource)
			{
				TouchInputSource touchInputSource = (TouchInputSource)CameraController.instance.InputSource;
				touchInputSource.touchSensitivity -= 0.2f;
				if (touchInputSource.touchSensitivity <= 0f)
				{
					touchInputSource.touchSensitivity = 3f;
				}
				go.GetComponentInChildren<Text>().text = GetTouchSensitivityText();
			}
		}

		private void PrintMobConfigsToLog(MobsManager.MobSpawnConfig[] configs)
		{
			StringWriter stringWriter = new StringWriter();
			float num = configs.Sum((MobsManager.MobSpawnConfig c) => c.weight);
			foreach (MobsManager.MobSpawnConfig mobSpawnConfig in configs)
			{
				AnimalMob component = mobSpawnConfig.prefab.GetComponent<AnimalMob>();
				if (component == null)
				{
					if (mobSpawnConfig.prefab == null)
					{
						stringWriter.WriteLine("Null mob on list, tell it to your programmer");
					}
					else
					{
						stringWriter.WriteLine("Prefab with name " + mobSpawnConfig.prefab.name + " is not an animal! Ask your programmer for further advice");
					}
				}
				else
				{
					stringWriter.WriteLine(string.Concat(str2: (mobSpawnConfig.weight / num).ToString("f2"), str0: mobSpawnConfig.prefab.name, str1: ", spawn chance: "));
				}
			}
			UnityEngine.Debug.Log(stringWriter.ToString());
			stringWriter.Flush();
			stringWriter.Close();
		}

		private void PrintFacts(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("print.facts", 0) + 1) % 2;
			PlayerPrefs.SetInt("print.facts", num);
			go.GetComponentInChildren<Text>().text = "Print facts " + ((num != 1) ? "OFF" : "ON");
		}

		private void ShowAchievements(GameObject go)
		{
		}

		private void SetNot(GameObject go)
		{
			Manager.Get<LocalNotificationsManager>().ScheduleNotificationQuick("Some", "Test", "Test KURWA Test", DateTime.Now.AddAMinute(), string.Empty);
		}

		private void CancelNot(GameObject go)
		{
			Manager.Get<LocalNotificationsManager>().CancelAllNotifications();
		}

		private void OnDebugEnableMultiThreading(GameObject go)
		{
			GreedyMeshCreator greedyMeshCreator = Engine.MeshCreator as GreedyMeshCreator;
			if (greedyMeshCreator == null)
			{
				go.GetComponentInChildren<Text>().text = "Error";
				return;
			}
			greedyMeshCreator.useThreading = !greedyMeshCreator.useThreading;
			go.GetComponentInChildren<Text>().text = "Enable multi-thread " + ((!greedyMeshCreator.useThreading) ? "OFF" : "ON");
		}

		private void OnDebugEnableEdgeNormals(GameObject go)
		{
			GreedyMeshCreator greedyMeshCreator = Engine.MeshCreator as GreedyMeshCreator;
			if (greedyMeshCreator == null)
			{
				go.GetComponentInChildren<Text>().text = "Error";
				return;
			}
			greedyMeshCreator.edgeNormals = !greedyMeshCreator.edgeNormals;
			go.GetComponentInChildren<Text>().text = "Enable edge normals " + ((!greedyMeshCreator.edgeNormals) ? "OFF" : "ON");
		}

		private void OnDebugEnableAmbientOcclusion(GameObject go)
		{
			GreedyMeshCreator greedyMeshCreator = Engine.MeshCreator as GreedyMeshCreator;
			if (greedyMeshCreator == null)
			{
				go.GetComponentInChildren<Text>().text = "Error";
				return;
			}
			greedyMeshCreator.ambientOcclusion = !greedyMeshCreator.ambientOcclusion;
			go.GetComponentInChildren<Text>().text = "Enable AO " + ((!greedyMeshCreator.ambientOcclusion) ? "OFF" : "ON");
		}

		private void OnDebugRebuildChunks(GameObject go)
		{
			ChunkManager chunkManagerInstance = Engine.ChunkManagerInstance;
			if (chunkManagerInstance == null)
			{
				go.GetComponentInChildren<Text>().text = "Error";
			}
			else if (!(go.GetComponentInChildren<Text>().text == "Done"))
			{
				chunkManagerInstance.RebuildChunks();
				go.GetComponentInChildren<Text>().text = "Done";
			}
		}

		private void SwitchPostEffects(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("post.effects", 1) + 1) % 2;
			PlayerPrefs.SetInt("post.effects", num);
			go.GetComponentInChildren<Text>().text = "Post Effects " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				MonoBehaviourSingleton<ImageEffectsController>.get.TryEnableAll();
			}
			else
			{
				MonoBehaviourSingleton<ImageEffectsController>.get.DisableAll();
			}
		}

		private void SwitchChunkColliders(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("chunk.colliders", 1) + 1) % 2;
			PlayerPrefs.SetInt("chunk.colliders", num);
			go.GetComponentInChildren<Text>().text = "Chunk colliders " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				foreach (Chunk value in ChunkManager.Chunks.Values)
				{
					value.gameObject.GetComponentInChildren<MeshCollider>(includeInactive: true).enabled = true;
				}
				Rigidbody[] array = Resources.FindObjectsOfTypeAll<Rigidbody>();
				foreach (Rigidbody rigidbody in array)
				{
					rigidbody.useGravity = true;
				}
			}
			else
			{
				foreach (Chunk value2 in ChunkManager.Chunks.Values)
				{
					value2.gameObject.GetComponentInChildren<MeshCollider>(includeInactive: true).enabled = false;
				}
				Rigidbody[] array2 = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
				foreach (Rigidbody rigidbody2 in array2)
				{
					rigidbody2.useGravity = false;
				}
			}
		}

		private void SwitchAnimals(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("animals.enabled", 1) + 1) % 2;
			PlayerPrefs.SetInt("animals.enabled", num);
			go.GetComponentInChildren<Text>().text = "Animals " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				Mob[] source = Resources.FindObjectsOfTypeAll<Mob>();
				foreach (Mob item in from m in source
					where !m.enabled
					select m)
				{
					IEnumerator enumerator2 = item.transform.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							Transform transform = (Transform)enumerator2.Current;
							transform.gameObject.SetActive(value: true);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					Array.ForEach(item.GetComponents<Rigidbody>(), delegate(Rigidbody r)
					{
						r.isKinematic = false;
					});
					Array.ForEach(item.GetComponents<Collider>(), delegate(Collider c)
					{
						c.enabled = true;
					});
					Array.ForEach(item.GetComponents<MobNavigator>(), delegate(MobNavigator c)
					{
						c.enabled = true;
					});
					Array.ForEach(item.GetComponents<Pettable>(), delegate(Pettable c)
					{
						c.enabled = true;
					});
					Array.ForEach(item.GetComponents<SaveTransform>(), delegate(SaveTransform c)
					{
						c.enabled = true;
					});
					Array.ForEach(item.GetComponents<Mountable>(), delegate(Mountable c)
					{
						c.enabled = true;
					});
					Array.ForEach(item.GetComponents<Danceable>(), delegate(Danceable c)
					{
						c.enabled = true;
					});
					Array.ForEach(item.GetComponents<Mob>(), delegate(Mob c)
					{
						c.enabled = true;
					});
				}
				return;
			}
			Mob[] array = UnityEngine.Object.FindObjectsOfType<Mob>();
			Mob[] array2 = array;
			foreach (Mob mob in array2)
			{
				IEnumerator enumerator3 = mob.transform.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						Transform transform2 = (Transform)enumerator3.Current;
						transform2.gameObject.SetActive(value: false);
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator3 as IDisposable)) != null)
					{
						disposable2.Dispose();
					}
				}
				Array.ForEach(mob.GetComponents<Rigidbody>(), delegate(Rigidbody r)
				{
					r.isKinematic = true;
				});
				Array.ForEach(mob.GetComponents<Collider>(), delegate(Collider c)
				{
					c.enabled = false;
				});
				Array.ForEach(mob.GetComponents<MobNavigator>(), delegate(MobNavigator c)
				{
					c.enabled = false;
				});
				Array.ForEach(mob.GetComponents<Pettable>(), delegate(Pettable c)
				{
					c.enabled = false;
				});
				Array.ForEach(mob.GetComponents<SaveTransform>(), delegate(SaveTransform c)
				{
					c.enabled = false;
				});
				Array.ForEach(mob.GetComponents<Mountable>(), delegate(Mountable c)
				{
					c.enabled = false;
				});
				Array.ForEach(mob.GetComponents<Danceable>(), delegate(Danceable c)
				{
					c.enabled = false;
				});
				Array.ForEach(mob.GetComponents<Mob>(), delegate(Mob c)
				{
					c.enabled = false;
				});
			}
		}

		private void SwitchChunkMeshRenderers(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("chunk.mesh.renderers", 1) + 1) % 2;
			PlayerPrefs.SetInt("chunk.mesh.renderers", num);
			go.GetComponentInChildren<Text>().text = "Chunk renderers " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				foreach (Chunk value in ChunkManager.Chunks.Values)
				{
					Array.ForEach(value.gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true), delegate(MeshRenderer m)
					{
						m.enabled = true;
					});
				}
			}
			else
			{
				foreach (Chunk value2 in ChunkManager.Chunks.Values)
				{
					Array.ForEach(value2.gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true), delegate(MeshRenderer m)
					{
						m.enabled = false;
					});
				}
			}
		}

		private void SwitchChunks(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("chunk.enabled", 1) + 1) % 2;
			PlayerPrefs.SetInt("chunk.enabled", num);
			go.GetComponentInChildren<Text>().text = "Chunks GOs " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				foreach (Chunk value in ChunkManager.Chunks.Values)
				{
					value.gameObject.SetActive(value: true);
				}
			}
			else
			{
				foreach (Chunk value2 in ChunkManager.Chunks.Values)
				{
					value2.gameObject.SetActive(value: false);
				}
			}
		}

		private void SwitchSkybox(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("skybox.enabled", 1) + 1) % 2;
			PlayerPrefs.SetInt("skybox.enabled", num);
			go.GetComponentInChildren<Text>().text = "Skybox " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				RenderSettings.skybox.shader = Shader.Find("Skybox/Farland Skies/Cloudy Crown");
			}
			else
			{
				RenderSettings.skybox.shader = Shader.Find("Unlit/Color");
			}
		}

		private void SwitchAnimators(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("animators.enabled", 1) + 1) % 2;
			PlayerPrefs.SetInt("animators.enabled", num);
			go.GetComponentInChildren<Text>().text = "Animators " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				Array.ForEach(UnityEngine.Object.FindObjectsOfType<Animator>(), delegate(Animator a)
				{
					a.enabled = true;
				});
			}
			else
			{
				Array.ForEach(UnityEngine.Object.FindObjectsOfType<Animator>(), delegate(Animator a)
				{
					a.enabled = false;
				});
			}
		}

		private void SwitchParticles(GameObject go)
		{
			int num = (PlayerPrefs.GetInt("particles.enabled", 1) + 1) % 2;
			PlayerPrefs.SetInt("particles.enabled", num);
			go.GetComponentInChildren<Text>().text = "Particles " + ((num != 1) ? "OFF" : "ON");
			if (num == 1)
			{
				Array.ForEach(UnityEngine.Object.FindObjectsOfType<ParticleSystem>(), delegate(ParticleSystem a)
				{
					a.enableEmission = true;
				});
			}
			else
			{
				Array.ForEach(UnityEngine.Object.FindObjectsOfType<ParticleSystem>(), delegate(ParticleSystem a)
				{
					a.enableEmission = false;
				});
			}
		}
	}
}
