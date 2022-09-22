// DecompilerFi decompiler from Assembly-CSharp.dll class: MonsterSpawnerEvents
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class MonsterSpawnerEvents : DefaultVoxelEvents, ISpawnableVoxelEvent
{
	public GameObject[] monsters;

	public float delay = 1f;

	public int maxMonsters = 5;

	private float time;

	private int currentMonster;

	private List<GameObject>[] aliveMonstersBuffer = new List<GameObject>[2];

	private int currentBuffer;

	public Vector3 position;

	private bool initialized;

	private void Start()
	{
	}

	public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
	{
		position = chunk.VoxelIndexToPosition(x, y, z);
		if (!initialized)
		{
			aliveMonstersBuffer[0] = new List<GameObject>();
			aliveMonstersBuffer[1] = new List<GameObject>();
			for (int i = 0; i < maxMonsters; i++)
			{
				Spawn();
			}
			initialized = true;
		}
	}

	public override void OnBlockPlace(VoxelInfo voxelInfo)
	{
		position = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
	}

	private void Update()
	{
		int num = CountAlive();
		if (maxMonsters >= 0)
		{
			if (num < maxMonsters)
			{
				time += Time.deltaTime;
				if (time >= delay)
				{
					time -= delay;
					Spawn();
				}
			}
			else
			{
				time = 0f;
			}
		}
		else
		{
			time += Time.deltaTime;
			if (time >= delay)
			{
				time -= delay;
				Spawn();
			}
		}
		SwapBuffers();
	}

	private void SwapBuffers()
	{
		aliveMonstersBuffer[1 - currentBuffer].Clear();
		for (int i = 0; i < aliveMonstersBuffer[currentBuffer].Count; i++)
		{
			aliveMonstersBuffer[1 - currentBuffer].Add(aliveMonstersBuffer[currentBuffer][i]);
		}
		aliveMonstersBuffer[currentBuffer].Clear();
		currentBuffer = 1 - currentBuffer;
	}

	private int CountAlive()
	{
		int num = 0;
		for (int i = 0; i < aliveMonstersBuffer[currentBuffer].Count; i++)
		{
			if (aliveMonstersBuffer[currentBuffer][i] != null)
			{
				num++;
			}
		}
		return num;
	}

	private void Spawn()
	{
		if (monsters.Length > 0)
		{
			GameObject gameObject = Object.Instantiate(monsters[currentMonster]);
			aliveMonstersBuffer[currentBuffer].Add(gameObject);
			Vector3 a = position;
			if (Physics.Raycast(new Vector3(a.x, 1000f, a.z), Vector3.down, out RaycastHit hitInfo, 2000f))
			{
				Vector3 point = hitInfo.point;
				a.y = point.y + 2f;
			}
			gameObject.transform.position = a + Random.insideUnitSphere.normalized * 5f;
			currentMonster = (currentMonster + 1) % monsters.Length;
		}
	}

	public GameObject[] GetPrefabs()
	{
		return monsters;
	}
}
