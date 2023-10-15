using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarBurnParticles : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem;

	[Space(10)] // Particle systems variables
	[SerializeField] private float _pMaxParticlesPerScale = 30;
	[SerializeField] private float _pRateOverTimePerScale = 6;
	[SerializeField] private Gradient _pStartColor = new Gradient();
	[SerializeField] private float _healtScale = 1f;
	[SerializeField] private AnimationCurve _healthScaleCurve = new AnimationCurve();

	ParticleSystem.MainModule _pMainModule;
	ParticleSystem.EmissionModule _pEmissionModule;

	// Current state of car's health, 0f = max, 1f = 0 hp.
	private float _healthState = 1f;

	private void Awake()
	{
		_pMainModule = _particleSystem.main;
		_pEmissionModule = _particleSystem.emission;
	}

	public void OnSpawn() { _healthState = 0f; _particleSystem.Stop(); }
	public void OnDespawn() { _particleSystem.Stop(); }


	public void UpdateHealthState(Health health)
	{
		_healthState = 1f - (health.CurrentHealth / health.MaxHealth);

		// If health is max or above, don't play particle effect.
		if (_healthState <= 0f)
		{
			if (_particleSystem.isPlaying) { _particleSystem.Stop(); }
			return;
		}

		if (!_particleSystem.isPlaying) { _particleSystem.Play(); }

		// Change ParticleSystem variables based on health amount.
		transform.localScale = Vector3.one * _healthScaleCurve.Evaluate(_healthState) * _healtScale;
		_pMainModule.startColor = _pStartColor.Evaluate(_healthState);

		_pMainModule.maxParticles = Mathf.RoundToInt(_pMaxParticlesPerScale * transform.localScale.x);
		_pEmissionModule.rateOverTime = _pRateOverTimePerScale * transform.localScale.x;
	}
}
