// DecompilerFi decompiler from Assembly-CSharp.dll class: Spine.Unity.BoundingBoxFollower
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteInEditMode]
	public class BoundingBoxFollower : MonoBehaviour
	{
		internal static bool DebugMessages = true;

		public SkeletonRenderer skeletonRenderer;

		[SpineSlot("", "skeletonRenderer", true, true)]
		public string slotName;

		public bool isTrigger;

		public bool clearStateOnDisable = true;

		private Slot slot;

		private BoundingBoxAttachment currentAttachment;

		private string currentAttachmentName;

		private PolygonCollider2D currentCollider;

		public readonly Dictionary<BoundingBoxAttachment, PolygonCollider2D> colliderTable = new Dictionary<BoundingBoxAttachment, PolygonCollider2D>();

		public readonly Dictionary<BoundingBoxAttachment, string> nameTable = new Dictionary<BoundingBoxAttachment, string>();

		public Slot Slot => slot;

		public BoundingBoxAttachment CurrentAttachment => currentAttachment;

		public string CurrentAttachmentName => currentAttachmentName;

		public PolygonCollider2D CurrentCollider => currentCollider;

		public bool IsTrigger => isTrigger;

		private void Start()
		{
			Initialize();
		}

		private void OnEnable()
		{
			if (skeletonRenderer != null)
			{
				skeletonRenderer.OnRebuild -= HandleRebuild;
				skeletonRenderer.OnRebuild += HandleRebuild;
			}
			Initialize();
		}

		private void HandleRebuild(SkeletonRenderer sr)
		{
			Initialize();
		}

		public void Initialize()
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			skeletonRenderer.Initialize(overwrite: false);
			if (string.IsNullOrEmpty(slotName) || (colliderTable.Count > 0 && slot != null && skeletonRenderer.skeleton == slot.Skeleton && slotName == slot.data.name))
			{
				return;
			}
			DisposeColliders();
			Skeleton skeleton = skeletonRenderer.skeleton;
			slot = skeleton.FindSlot(slotName);
			int slotIndex = skeleton.FindSlotIndex(slotName);
			if (slot == null)
			{
				if (DebugMessages)
				{
					UnityEngine.Debug.LogWarning($"Slot '{slotName}' not found for BoundingBoxFollower on '{base.gameObject.name}'. (Previous colliders were disposed.)");
				}
				return;
			}
			if (base.gameObject.activeInHierarchy)
			{
				foreach (Skin skin in skeleton.Data.Skins)
				{
					List<string> list = new List<string>();
					skin.FindNamesForSlot(slotIndex, list);
					foreach (string item in list)
					{
						Attachment attachment = skin.GetAttachment(slotIndex, item);
						BoundingBoxAttachment boundingBoxAttachment = attachment as BoundingBoxAttachment;
						if (DebugMessages && attachment != null && boundingBoxAttachment == null)
						{
							UnityEngine.Debug.Log("BoundingBoxFollower tried to follow a slot that contains non-boundingbox attachments: " + slotName);
						}
						if (boundingBoxAttachment != null)
						{
							PolygonCollider2D polygonCollider2D = SkeletonUtility.AddBoundingBoxAsComponent(boundingBoxAttachment, slot, base.gameObject, isTrigger);
							polygonCollider2D.enabled = false;
							polygonCollider2D.hideFlags = HideFlags.NotEditable;
							polygonCollider2D.isTrigger = IsTrigger;
							colliderTable.Add(boundingBoxAttachment, polygonCollider2D);
							nameTable.Add(boundingBoxAttachment, item);
						}
					}
				}
			}
			if (DebugMessages && colliderTable.Count == 0)
			{
				if (base.gameObject.activeInHierarchy)
				{
					UnityEngine.Debug.LogWarning("Bounding Box Follower not valid! Slot [" + slotName + "] does not contain any Bounding Box Attachments!");
				}
				else
				{
					UnityEngine.Debug.LogWarning("Bounding Box Follower tried to rebuild as a prefab.");
				}
			}
		}

		private void OnDisable()
		{
			if (clearStateOnDisable)
			{
				ClearState();
			}
		}

		public void ClearState()
		{
			if (colliderTable != null)
			{
				foreach (PolygonCollider2D value in colliderTable.Values)
				{
					value.enabled = false;
				}
			}
			currentAttachment = null;
			currentAttachmentName = null;
			currentCollider = null;
		}

		private void DisposeColliders()
		{
			PolygonCollider2D[] components = GetComponents<PolygonCollider2D>();
			if (components.Length == 0)
			{
				return;
			}
			if (Application.isEditor)
			{
				if (Application.isPlaying)
				{
					PolygonCollider2D[] array = components;
					foreach (PolygonCollider2D polygonCollider2D in array)
					{
						if (polygonCollider2D != null)
						{
							UnityEngine.Object.Destroy(polygonCollider2D);
						}
					}
				}
				else
				{
					PolygonCollider2D[] array2 = components;
					foreach (PolygonCollider2D polygonCollider2D2 in array2)
					{
						if (polygonCollider2D2 != null)
						{
							UnityEngine.Object.DestroyImmediate(polygonCollider2D2);
						}
					}
				}
			}
			else
			{
				PolygonCollider2D[] array3 = components;
				foreach (PolygonCollider2D polygonCollider2D3 in array3)
				{
					if (polygonCollider2D3 != null)
					{
						UnityEngine.Object.Destroy(polygonCollider2D3);
					}
				}
			}
			slot = null;
			currentAttachment = null;
			currentAttachmentName = null;
			currentCollider = null;
			colliderTable.Clear();
			nameTable.Clear();
		}

		private void LateUpdate()
		{
			if (slot != null && slot.Attachment != currentAttachment)
			{
				MatchAttachment(slot.Attachment);
			}
		}

		private void MatchAttachment(Attachment attachment)
		{
			BoundingBoxAttachment boundingBoxAttachment = attachment as BoundingBoxAttachment;
			if (DebugMessages && attachment != null && boundingBoxAttachment == null)
			{
				UnityEngine.Debug.LogWarning("BoundingBoxFollower tried to match a non-boundingbox attachment. It will treat it as null.");
			}
			if (currentCollider != null)
			{
				currentCollider.enabled = false;
			}
			if (boundingBoxAttachment == null)
			{
				currentCollider = null;
			}
			else
			{
				currentCollider = colliderTable[boundingBoxAttachment];
				currentCollider.enabled = true;
			}
			currentAttachment = boundingBoxAttachment;
			currentAttachmentName = ((currentAttachment != null) ? nameTable[boundingBoxAttachment] : null);
		}
	}
}
