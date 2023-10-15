using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
	[Tooltip("Camera profiles for multiplayer games.")]
	[SerializeField] private NestedList<NestedList<CameraSplitProfile>> _cameraProfiles = new NestedList<NestedList<CameraSplitProfile>>();


	/// <summary> Amount of players currently playing.</summary>
	public int ActivePlayerCount { get; set; } = 0;

	/// <summary> Amount of players that can be set active.</summary>
	public int SupportedPlayerCount => Math.Min(_cameraProfiles.List.Count, InputManager.Instance.Controllers.Count);

	/// <summary> Spawned player following cameras with their target car.</summary>
	public List<(Car, CameraMan)> SpawnedCarCameras { get; private set; } = new List<(Car, CameraMan)>();

	/// <summary> Tracks which spawn point was used previously. To spread players.</summary>
	private int _spawnPointTracker = 0;


	public void Reset()
	{
		_spawnPointTracker = 0;

		// Make sure cars don't respawn when they are despawned by reset.
		SpawnedCarCameras.ForEach(x => x.Item1.OnDespawn.RemoveListener(Respawn));
	}

	/// <summary> Spawns all player cameras and cars.</summary>
	public void SpawnPlayers()
	{
		// Despawn possible previous cars and cameras.
		for (int i = SpawnedCarCameras.Count - 1; i >= 0; i--)
		{
			SpawnedCarCameras[i].Item1.OnDespawn.RemoveListener(Respawn);
			SpawnedCarCameras[i].Item1.Despawn();
			SpawnedCarCameras[i].Item2.Despawn();
			SpawnedCarCameras.RemoveAt(i);
		}

		// Spawn cameras and cards based on ActivePlayerCount.
		for (int i = 0; i < ActivePlayerCount; i++)
		{
			Car newPlayer = SpawnCar();
			if (newPlayer == null) { continue; }

			// Register car to a InputController.
			InputManager.Instance.Controllers[i].Subscribe(newPlayer.Input);

			// Spawn cameraman for the car.
			CameraMan newCamera = ObjectManager.Instance.Spawn(PoolableType.cameraman, newPlayer.transform.position) as CameraMan;
			if (newCamera == null) { Debug.LogError($"Spawned null cameraman {i}. ObjectManager missing something or odds fail?"); continue; }

			// Set camera target and screen size & position.
			newCamera.TargetObject = newPlayer.transform;
			newCamera.SetCameraProfile(_cameraProfiles.List[ActivePlayerCount - 1].List[i]);

			SpawnedCarCameras.Add((newPlayer, newCamera));
		}
	}

	/// <summary> Spawn new car for a player and transfer camera to track it.</summary>
	public void Respawn(Poolable oldCar)
	{
		for (int i = SpawnedCarCameras.Count - 1; i >= 0; i--)
		{
			if (SpawnedCarCameras[i].Item1 != oldCar) { continue; }

			// Deregister old car from InpurController and remove respawn listener.
			InputManager.Instance.Controllers[i].Unsubscribe((oldCar as Car).Input);
			oldCar.OnDespawn.RemoveListener(Respawn);

			// Spawn new car and combine with camera.
			SpawnedCarCameras[i] = (SpawnCar(), SpawnedCarCameras[i].Item2);

			// Register new car to InpurController and set as camera target.
			InputManager.Instance.Controllers[i].Subscribe(SpawnedCarCameras[i].Item1.Input);
			SpawnedCarCameras[i].Item2.SetTargetObject(SpawnedCarCameras[i].Item1.transform, 0.5f);

			break;
		}
	}

	/// <summary> Spawns new car.</summary>
	private Car SpawnCar()
	{
		// Get next spawn point to spread players.
		_spawnPointTracker++;
		if (_spawnPointTracker >= GameManager.Instance.LevelData.PlayerSpawnPoints.Count) { _spawnPointTracker = 0; }
		(int, int, int) spawnPoint = GameManager.Instance.LevelData.PlayerSpawnPoints[_spawnPointTracker];

		// Spawn player car.
		Car newPlayer = ObjectManager.Instance.Spawn(PoolableType.player, (spawnPoint.Item1, spawnPoint.Item2), spawnPoint.Item3, 1000, -1) as Car;
		if (newPlayer == null) { Debug.LogError($"Spawned null car. ObjectManager missing something or odds fail?"); return null; }

		// Automatically respawn if car despawns.
		newPlayer.OnDespawn.AddListener(Respawn);

		return newPlayer;
	}
}