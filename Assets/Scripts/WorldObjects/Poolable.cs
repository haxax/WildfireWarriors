using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Poolable : MonoBehaviour
{
	[Tooltip("If not empty, tracks the spawned amount of object with the same TrackKey in ObjectManager.")]
	[SerializeField] private string _trackKey = "";

	[Tooltip("Amount of delay between despawning and returning to pool. The delay can be used for death effects etc.")]
	[Min(0f)][SerializeField] private float _despawnDelay = 0f;

	[Tooltip("Automatically called when the object is spawned from pool.")]
	[SerializeField] private UnityEvent<Poolable> _onSpawn = new UnityEvent<Poolable>();
	[Tooltip("Automatically called when Despawn is called. Use to disable things you don't want to interact with world before OnReturnToPool is called. Or to activate death effects etc.")]
	[SerializeField] private UnityEvent<Poolable> _onDespawn = new UnityEvent<Poolable>();
	[Tooltip("Automatically called when Despawning has ended. Object will disappear right after this. Use to disable final things.")]
	[SerializeField] private UnityEvent<Poolable> _onReturnToPool = new UnityEvent<Poolable>();

	public string TrackKey { get => _trackKey; private set => _trackKey = value; }
	public float DespawnDelay { get => _despawnDelay; set => _despawnDelay = value; }
	public UnityEvent<Poolable> OnSpawn => _onSpawn;
	public UnityEvent<Poolable> OnDespawn => _onDespawn;
	public UnityEvent<Poolable> OnReturnToPool => _onReturnToPool;


	/// <summary> Returns new instance of this poolable from pool. Only call to get a copy of a prefab.</summary>
	public Poolable GetFromPool(Vector3 position, Quaternion rotation)
	{
		Poolable newInstance = ObjectManager.Instance.GetFromPool(this, position, rotation);

		newInstance?.Spawn();

		return newInstance;
	}

	/// <summary> Is called whenever taken from pool. Use OnSpawn to reset values and enable things.</summary>
	protected virtual void Spawn()
	{ 
		OnSpawn?.Invoke(this);
		if(TrackKey != "") { ObjectManager.Instance.TrackSpawn(TrackKey); }
	}

	/// <summary>
	/// Call to start the process of despawning. Use OnDespawn to disable colliders etc. which you don't want to interact with the world. 
	/// OnDespawn can be used to start 'death' effects etc. Disable them in OnReturnToPool.
	/// </summary>
	public virtual void Despawn()
	{
		OnDespawn?.Invoke(this);
		if (TrackKey != "") { ObjectManager.Instance.TrackDespawn(TrackKey); }

		// DespawnDelay is set higher than 0, add temporal delay component.
		if (DespawnDelay > 0f)
		{ gameObject.AddComponent<PoolableDespawnDelay>().StartDespawn(ReturnToPool, DespawnDelay); }
		else { ReturnToPool(); }
	}

	/// <summary> You may want to use Despawn instead. Despawns immediately. Both OnDespawn and OnReturnToPool will be called.</summary>
	public void DespawnWithoutDelay()
	{
		// Make sure delay isn't kept if this method is used.
		PoolableDespawnDelay delay = gameObject.GetComponent<PoolableDespawnDelay>();
		if (delay != null) { Destroy(delay); }

		OnDespawn?.Invoke(this);
		ReturnToPool();

	}

	/// <summary> Returns this poolable back to the pool after despawning is finished.</summary>
	private void ReturnToPool()
	{
		OnReturnToPool?.Invoke(this);
		ObjectManager.Instance.ReturnToPool(this);
	}
}