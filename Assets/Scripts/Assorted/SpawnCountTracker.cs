using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnCountTracker : MonoBehaviour
{
	[Tooltip("Same TrackKey as defined in the tracked poolable object.")]
	[SerializeField] private string _key = "";
	public string Key { get { return _key; } }

	protected virtual void Start()
	{
		if (_key == "") { return; }

		// Track at ObjectManager's OnSpawnCountChange.
		ObjectManager.Instance.OnSpawnCountChange += UpdateAmount;

		// Check if Tracked objects are already spawned.
		if (ObjectManager.Instance.SpawnTracker.ContainsKey(_key))
		{
			OnUpdateAmount(ObjectManager.Instance.SpawnTracker[_key]);
		}
		else
		{ OnUpdateAmount(0); }
	}

	public void UpdateAmount(string key, int amount)
	{
		// Update only if received key matches.
		if (Key == key)
		{ OnUpdateAmount(amount); }
	}

	public abstract void OnUpdateAmount(int amount);

	protected virtual void OnDestroy()
	{
		// In case destroyed, remove the listener.
		if (ObjectManager.Instance == null) { return; }
		ObjectManager.Instance.OnSpawnCountChange -= UpdateAmount;
	}
}
