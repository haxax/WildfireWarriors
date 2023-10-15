using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBarrelGun : Gun
{
	[Tooltip("Projectile is spawned at the same position with ShootPoint.")]
	[SerializeField] protected Transform _shootPoint;

	[Tooltip("Prefab of the shot projectile. Should be IProjectile.")]
	[SerializeField] protected Poolable _projectilePrefab;

	[Tooltip("Force applied to projectile. Direction is same as ShootPoint.forward.")]
	[SerializeField] protected float _shootForce = 1f;

	protected override void Shoot()
	{
		IProjectile newProjectile = _projectilePrefab.GetFromPool(_shootPoint.position, Quaternion.identity) as IProjectile;
		newProjectile?.Shoot(_shootPoint.forward * _shootForce);
	}
}