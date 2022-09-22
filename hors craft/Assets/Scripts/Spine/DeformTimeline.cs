// DecompilerFi decompiler from Assembly-CSharp.dll class: Spine.DeformTimeline
using System;

namespace Spine
{
	public class DeformTimeline : CurveTimeline
	{
		internal int slotIndex;

		internal float[] frames;

		internal float[][] frameVertices;

		internal VertexAttachment attachment;

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				slotIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public float[][] Vertices
		{
			get
			{
				return frameVertices;
			}
			set
			{
				frameVertices = value;
			}
		}

		public VertexAttachment Attachment
		{
			get
			{
				return attachment;
			}
			set
			{
				attachment = value;
			}
		}

		public override int PropertyId => 100663296 + attachment.id + slotIndex;

		public DeformTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount];
			frameVertices = new float[frameCount][];
		}

		public void SetFrame(int frameIndex, float time, float[] vertices)
		{
			frames[frameIndex] = time;
			frameVertices[frameIndex] = vertices;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			VertexAttachment vertexAttachment = slot.attachment as VertexAttachment;
			if (vertexAttachment == null || !vertexAttachment.ApplyDeform(attachment))
			{
				return;
			}
			ExposedList<float> attachmentVertices = slot.attachmentVertices;
			float[][] array = frameVertices;
			int num = array[0].Length;
			if (attachmentVertices.Count != num && pose != 0)
			{
				alpha = 1f;
			}
			if (attachmentVertices.Capacity < num)
			{
				attachmentVertices.Capacity = num;
			}
			attachmentVertices.Count = num;
			float[] items = attachmentVertices.Items;
			float[] array2 = frames;
			if (time < array2[0])
			{
				switch (pose)
				{
				case MixPose.Setup:
					attachmentVertices.Clear();
					break;
				case MixPose.Current:
					alpha = 1f - alpha;
					for (int i = 0; i < num; i++)
					{
						items[i] *= alpha;
					}
					break;
				}
				return;
			}
			if (time >= array2[array2.Length - 1])
			{
				float[] array3 = array[array2.Length - 1];
				if (alpha == 1f)
				{
					Array.Copy(array3, 0, items, 0, num);
				}
				else if (pose == MixPose.Setup)
				{
					VertexAttachment vertexAttachment2 = vertexAttachment;
					if (vertexAttachment2.bones == null)
					{
						float[] vertices = vertexAttachment2.vertices;
						for (int j = 0; j < num; j++)
						{
							float num2 = vertices[j];
							items[j] = num2 + (array3[j] - num2) * alpha;
						}
					}
					else
					{
						for (int k = 0; k < num; k++)
						{
							items[k] = array3[k] * alpha;
						}
					}
				}
				else
				{
					for (int l = 0; l < num; l++)
					{
						items[l] += (array3[l] - items[l]) * alpha;
					}
				}
				return;
			}
			int num3 = Animation.BinarySearch(array2, time);
			float[] array4 = array[num3 - 1];
			float[] array5 = array[num3];
			float num4 = array2[num3];
			float curvePercent = GetCurvePercent(num3 - 1, 1f - (time - num4) / (array2[num3 - 1] - num4));
			if (alpha == 1f)
			{
				for (int m = 0; m < num; m++)
				{
					float num5 = array4[m];
					items[m] = num5 + (array5[m] - num5) * curvePercent;
				}
			}
			else if (pose == MixPose.Setup)
			{
				VertexAttachment vertexAttachment3 = vertexAttachment;
				if (vertexAttachment3.bones == null)
				{
					float[] vertices2 = vertexAttachment3.vertices;
					for (int n = 0; n < num; n++)
					{
						float num6 = array4[n];
						float num7 = vertices2[n];
						items[n] = num7 + (num6 + (array5[n] - num6) * curvePercent - num7) * alpha;
					}
				}
				else
				{
					for (int num8 = 0; num8 < num; num8++)
					{
						float num9 = array4[num8];
						items[num8] = (num9 + (array5[num8] - num9) * curvePercent) * alpha;
					}
				}
			}
			else
			{
				for (int num10 = 0; num10 < num; num10++)
				{
					float num11 = array4[num10];
					items[num10] += (num11 + (array5[num10] - num11) * curvePercent - items[num10]) * alpha;
				}
			}
		}
	}
}
