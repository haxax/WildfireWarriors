using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Added to a poolable when despawning. Automatically returns back to pool after the delay.</summary>
public class PoolableDespawnDelay : MonoBehaviour
{
	private float _timer = 0f;
	private event Action _returnToPool;

	public void StartDespawn(Action returnToPool, float duration)
	{
		_returnToPool += returnToPool;
		_timer = duration;
	}

	private void FixedUpdate()
	{
		_timer -= Time.fixedDeltaTime;
		if (_timer <= 0f)
		{
			_returnToPool.Invoke();
			Destroy(this);
		}
	}
}
