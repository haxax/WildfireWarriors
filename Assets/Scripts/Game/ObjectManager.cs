using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
	[Tooltip("Prefab lists which are used if something is spawned by type.")]
	[SerializeField] private List<PoolablePrefabStack> _prefabs = new List<PoolablePrefabStack>();
	[Tooltip("Same as Prefabs but used instead if on WebGL.")]
	[SerializeField] private List<PoolablePrefabStack> _prefabsWebGl = new List<PoolablePrefabStack>();

	public List<PoolablePrefabStack> Prefabs => _prefabs;
	public List<PoolablePrefabStack> PrefabsWebGl => _prefabsWebGl;


	public void Reset()
	{
		SpawnTracker.Clear();

		// Despawn all active poolables in the scene.
		List<string> keys = new List<string>(ActivePoolables.Keys);
		for (int i = keys.Count - 1; i >= 0; i--)
		{
			List<Poolable> instances = _activePoolables[keys[i]];

			for (int j = instances.Count - 1; j >= 0; j--)
			{ instances[j].DespawnWithoutDelay(); }
		}
	}





	#region Spawn tracker

	/// <summary> Tracked keys with their current amount. Multiple different poolables can have same TrackKey, therefore this instead of ActivePoolables.</summary>
	public Dictionary<string, int> SpawnTracker { get; private set; } = new Dictionary<string, int>();

	/// <summary> Call when tracked object is spawned.</summary>
	public void TrackSpawn(string key)
	{
		if (SpawnTracker.ContainsKey(key))
		{ SpawnTracker[key]++; }
		else { SpawnTracker.Add(key, 1); }
		OnSpawnCountChange?.Invoke(key, SpawnTracker[key]);
	}

	/// <summary> Call when tracked object is despawned.</summary>
	public void TrackDespawn(string key)
	{
		if (SpawnTracker.ContainsKey(key))
		{ SpawnTracker[key]--; }
		else { SpawnTracker.Add(key, 0); }
		OnSpawnCountChange?.Invoke(key, SpawnTracker[key]);
	}

	/// <summary> Invoked whenever tracked object is spawned or despawned. Contains the key and current amount.</summary>
	public event Action<string, int> OnSpawnCountChange;

	#endregion





	#region Pool

	private Dictionary<string, List<Poolable>> _activePoolables = new Dictionary<string, List<Poolable>>();
	/// <summary> Poolable objects which are currently active and NOT in pool. String key = object's name./// </summary>
	public Dictionary<string, List<Poolable>> ActivePoolables { get { return _activePoolables; } set { _activePoolables = value; } }


	private Dictionary<string, Queue<Poolable>> _pool = new Dictionary<string, Queue<Poolable>>();
	/// <summary> Poolable objects which are currently inactive and in pool. String key = object's name./// </summary>
	private Dictionary<string, Queue<Poolable>> Pool { get { return _pool; } set { _pool = value; } }


	/// <summary> Returns a new instance of the reference poolable from the pool. Can be null.</summary>
	public Poolable GetFromPool(Poolable reference, Vector3 position, Quaternion rotation)
	{
		Poolable result = null;

		ValidateDictionaryContent(reference);

		// Get poolable from pool or instantiate new if pool is empty.
		if (Pool[reference.name].Count == 0)
		{
			result = Instantiate(reference, position, rotation);

			// Set name correctly, Dictionary is based on the poolable's name.
			result.name = reference.name;
		}
		else
		{
			result = Pool[reference.name].Dequeue();

			result.transform.SetPositionAndRotation(position, rotation);
			result.gameObject.SetActive(true);
		}

		// Add to ActivePoolables.
		ActivePoolables[reference.name].Add(result);

		return result;
	}

	/// <summary> Returns an active poolable back to pool.</summary>
	public void ReturnToPool(Poolable poolable)
	{
		ValidateDictionaryContent(poolable);

		// Reset transform, disable and set to pool queue.
		poolable.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		poolable.gameObject.SetActive(false);

		ActivePoolables[poolable.name].Remove(poolable);
		Pool[poolable.name].Enqueue(poolable);
	}

	/// <summary> Check if both Pool and ActivePoolables contains the poolable reference key.</summary>
	private void ValidateDictionaryContent(Poolable key)
	{
		// Make sure Pool and ActivePoolables contains the key.
		if (!Pool.ContainsKey(key.name))
		{ Pool[key.name] = new Queue<Poolable>(); }

		if (!ActivePoolables.ContainsKey(key.name))
		{ ActivePoolables[key.name] = new List<Poolable>(); }
	}

	#endregion





	#region Spawning an object by type.

	/// <summary> Spawns all objects defined in level the file.</summary>
	public void SpawnLevelObjects()
	{
		foreach (int[] obj in GameManager.Instance.LevelData.ObjectData)
		{
			// Don't spawn objects with negative id.
			if (obj[0] < 0) { continue; }

			Spawn((PoolableType)obj[0], (obj[1], obj[2]), obj[3], obj[4]);
		}
	}

	/// <summary> Spawns trees to all empty tiles defined in level file. </summary>
	public void SpawnLevelTrees()
	{
		foreach ((int, int) coordinate in GameManager.Instance.LevelData.TreeSpawns)
		{ Spawn(PoolableType.forest, coordinate); }
	}

	/// <summary> Spawns an object based on data defined in the level file. Coordinate is in level data format (x,y). </summary>
	public Poolable Spawn(PoolableType type, (int, int) coordinate, float direction = 0f, int odds = 1000, int id = -1)
	{
		// Convert tile coordinate to world location.
		return Spawn(type, GameManager.Instance.LevelData.GetGroundLocationAt(coordinate), direction, odds, id);
	}

	/// <summary> Spawns an object of given PoolableType.</summary>
	/// <param name="type">Type of object wanted to be spawned.</param>
	/// <param name="position">Position in world space.</param>
	/// <param name="direction">Rotation in Y-axis.</param>
	/// <param name="odds">Chances to be spawned. 1000 = 100%, 1 = 0.1%, 0 = 0%.</param>
	/// <param name="id">List number in PrefabStack. Use -1 to get random.</param>
	/// <returns>Return an instance of the type from the pool. Can be null, if failed to spawn, especially in case of odds.</returns>
	public Poolable Spawn(PoolableType type, Vector3 position, float direction = 0f, int odds = 1000, int id = -1)
	{
		// Do randomization, whether object should be spawned.
		if (UnityEngine.Random.Range(0, 1000) >= odds) { return null; }

		// Get matching PrefabStack. Should never be null.
		PoolablePrefabStack prefabs = GetPrefabStack(type);
		if (prefabs == null) { SpawnError(position); return null; }

		// Get prefab from the PrefabStack. Should never be null.
		Poolable prefab = prefabs.GetPrefab(id);
		if (prefab == null) { SpawnError(position); return null; }

		// Return matching poolable from pool.
		return prefab.GetFromPool(position, Quaternion.Euler(new Vector3(0f, direction, 0f)));
	}

	/// <summary> Spawns an Error object to the given position in world space.</summary>
	public void SpawnError(Vector3 position)
	{ Spawn(PoolableType.none, position); }

	/// <summary> Returns a PrefabStack of given type. PrefabStack contains a list of available prefabs of a type.</summary>
	private PoolablePrefabStack GetPrefabStack(PoolableType type)
	{
		// Get proper stack variant based on platform.
		List<PoolablePrefabStack> stack;
#if UNITY_WEBGL
		// Lighter prefabs for WebGL build.
		stack = PrefabsWebGl;
#else
		// Normal prefabs otherwise.
		stack = Prefabs;
#endif

		// Find matching PrefabStack based on type.
		for (int i = 0; i < stack.Count; i++)
		{
			if (stack[i].PoolableType == type) { return stack[i]; }
		}

		Debug.LogWarning($"Spawnable type {type} not found in Prefabs stack.");
		return null;
	}

	#endregion
}