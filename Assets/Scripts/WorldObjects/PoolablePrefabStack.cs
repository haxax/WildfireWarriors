using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabStack", menuName = "Custom/PrefabStack", order = 70)]
public class PoolablePrefabStack : ScriptableObject
{
	[Tooltip("Objects' int id from level data is converted to PoolableType, should match this.")]
	[SerializeField] private PoolableType _poolableType = PoolableType.none;
	[SerializeField] private List<Poolable> _prefabs = new List<Poolable>();

	public PoolableType PoolableType { get => _poolableType; private set => _poolableType = value; }
	public List<Poolable> Prefabs { get => _prefabs; private set => _prefabs = value; }


	/// <summary> Returns prefab from the stack based on id.</summary>
	/// <param name="id">id = number in list, -1 returns random prefab.</param>
	public Poolable GetPrefab(int id = -1)
	{
		// Check if Prefabs list is empty. If so, return null.
		if (Prefabs.Count == 0)
		{
			Debug.LogWarning($"Trying to spawn {PoolableType} but the Prefabs list is empty.");
			return null;
		}

		// If given id is negative, randomize new one.
		if (id < 0) { id = Random.Range(0, Prefabs.Count); }

		return Prefabs[id];
	}
}

public enum PoolableType
{
	// Use negative values to define non-spawnable things in level file.
	playerSpawnPoint = -1,

	none = 0,

	// Actually spawnable things.
	player = 1,
	fire = 2,
	forest = 3,
	powerup = 4,
	cameraman = 5,
	waterProjectile = 6,
}