using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticles : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem;

	[Space(10)]
	[SerializeField] private float _pMaxParticlesPerScale = 30;
	[SerializeField] private float _pRateOverTimePerScale = 6;
	[SerializeField] private float _pStartLifeTimePerScale = 0.5f;
	[SerializeField] private Gradient _pStartColor = new Gradient();

	ParticleSystem.MainModule _pMainModule;
	ParticleSystem.EmissionModule _pEmissionModule;

	private void Awake()
	{
		_pMainModule = _particleSystem.main;
		_pEmissionModule = _particleSystem.emission;
	}

	public void OnSpawn() { _particleSystem.Play(); }
	public void OnDespawn() { _particleSystem.Stop(); }

	public void UpdateParticles(float scale)
	{
		// Update ParticleSystem, update values based on scale.
		_pMainModule.startLifetime = _pStartLifeTimePerScale * scale;
		_pMainModule.maxParticles = Mathf.RoundToInt(_pMaxParticlesPerScale * scale);
		_pEmissionModule.rateOverTime = _pRateOverTimePerScale * scale;
		_particleSystem.transform.localScale = Vector3.one * scale;
	}

	public void UpdateHealthState(Health health)
	{
		// Change color based on health.
		_pMainModule.startColor = _pStartColor.Evaluate(health.CurrentHealth / health.MaxHealth);
	}
}
