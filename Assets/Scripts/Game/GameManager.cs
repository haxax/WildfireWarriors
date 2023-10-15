using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Tooltip("The level which is loaded as background of main menu.")]
	[SerializeField] private string _menuLevelName = "menu001";

	[Space(10)]
	[Tooltip("The TrackKey which is used to track when game ends, when all fire is extinguished.")]
	[SerializeField] private string _endKey = "Fire";
	[Tooltip("Game ends when this amount of *_endKey* is left.")]
	[SerializeField] private int _endAmount = 0;

	public event Action OnStartGame;
	public event Action OnEndGame;
	public event Action OnMainMenu;

	/// <summary> Currently loaded LevelData. </summary>
	public LevelData LevelData { get; private set; }

	/// <summary> Game is running if this is false.</summary>
	public bool GameEnded { get; private set; } = true;


	private void Start()
	{
		ObjectManager.Instance.OnSpawnCountChange += CheckSpawnBasedGameState;
	}

	/// <summary> Core events of entering to the Main Menu. </summary>
	public void GoToMainMenu()
	{
		UnloadLevel();

		// Load background level.
		LevelLoader.Instance.SelectLevel(_menuLevelName, LevelLoader.LoadMethod.resources);
		DisplayLoadLevel(false, false);

		// Enable Main Menu UI and behaviour.
		MenuManager.Instance.SetMenuState(MenuStateType.mainMenu);
		OnMainMenu?.Invoke();
	}

	public void StartLevel()
	{
		UnloadLevel();
		LoadLevel();
		MenuManager.Instance.SetMenuState(MenuStateType.gameUI);

		GameEnded = false;
		OnStartGame?.Invoke();
	}

	/// <summary> Wipes previously loaded stuff. Pool keeps poolables pooled.</summary>
	public void UnloadLevel()
	{
		// PlayerManager has to reset before ObjectManager, to reset cars first.
		PlayerManager.Instance.Reset();

		ObjectManager.Instance.Reset();
		LevelMesh.Instance.Reset();

		LevelData = null;
		GameEnded = true;
	}

	/// <summary> Loads a level without starting a game. You Should UnloadLevel first.</summary>
	public void DisplayLoadLevel(bool spawnTrees = true, bool spawnObjects = true)
	{
		// Load level and create mesh.
		LevelData = LevelLoader.Instance.LoadLevelData();
		LevelMesh.Instance.GenerateLevel();

		// Spawn wanted objects.
		if (spawnTrees) { ObjectManager.Instance.SpawnLevelTrees(); }
		if (spawnObjects) { ObjectManager.Instance.SpawnLevelObjects(); }
	}

	/// <summary> Loads level from previously selected level file/source and generates all objects.</summary>
	private void LoadLevel()
	{
		// Load level and create mesh.
		LevelData = LevelLoader.Instance.LoadLevelData();
		LevelMesh.Instance.GenerateLevel();

		// Spawn objects.
		ObjectManager.Instance.SpawnLevelTrees();
		ObjectManager.Instance.SpawnLevelObjects();
		PlayerManager.Instance.SpawnPlayers();
	}

	/// <summary>
	/// Checks active game's state based on tracked poolable count. Ends game if defined goal is reached.
	/// </summary>
	private void CheckSpawnBasedGameState(string key, int amount)
	{
		if (GameEnded) { return; }

		if (key == _endKey && amount <= _endAmount)
		{
			GameEnded = true;
			OnEndGame?.Invoke();
			Debug.Log("Game ended");
		}
	}
}