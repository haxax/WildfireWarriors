using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBarrelGun : Gun
{
	[Tooltip("Projectile is spawned at the same position with one of the ShootPoints.")]
	[SerializeField] protected List<Transform> _shootPoints;

	[Tooltip("Prefab of the shot projectile. Should be IProjectile.")]
	[SerializeField] protected Poolable _projectilePrefab;

	[Tooltip("Force applied to projectile. Direction is same as ShootPoint.forward.")]
	[SerializeField] protected float _shootForce = 1f;

	// Tracks which _shootPoint is used next.
	private int _barrelTracker = 0;

	public override void OnSpawn()
	{
		_barrelTracker = 0;
		base.OnSpawn();
	}

	protected override void Shoot()
	{
		// Loop through _shootPoints.
		_barrelTracker++;
		if (_barrelTracker >= _shootPoints.Count)
		{ _barrelTracker = 0; }

		// Spawn at the location of next ShootPoint.
		IProjectile newProjectile = _projectilePrefab.GetFromPool(_shootPoints[_barrelTracker].position, Quaternion.identity) as IProjectile;
		newProjectile.Shoot(_shootPoints[_barrelTracker].forward * _shootForce);
	}
}