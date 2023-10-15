using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelLoader : Singleton<LevelLoader>
{
	// Mesh world size variables.
	[Tooltip("Width between mesh vertices.")]
	[SerializeField] private float _tileSize = 1f;

	[Tooltip("Level's height data is limited to 256 steps. Height per step.")]
	[SerializeField] private float _unitHeight = 1f;

	// For loading.
	[Space(10)]
	[Tooltip("Defines how the level is loaded.")]
	[SerializeField] private LoadMethod _method = LoadMethod.resources;

	[Tooltip("Level name, path, raw text, whatever is used to choose what is loaded.")]
	[SerializeField] private string _loadParam = "";


	private LoadMethod Method { get => _method; set => _method = value; }
	private string LoadParam { get => _loadParam; set => _loadParam = value; }


	/// <summary> Use to select which level is loaded when game is started.</summary>
	/// <param name="loadParam">File name, source path etc.</param>
	/// <param name="loadMethod">Where the level is loaded from.</param>
	public void SelectLevel(string loadParam, LoadMethod loadMethod)
	{ LoadParam = loadParam; Method = loadMethod; }


	/// <summary> Loads level data based on set LoadMethod and LoadParam.</summary>
	/// <returns> Usable level data byte[0 = ground height points, 1 = texture ids][world row][world column].</returns>
	public LevelData LoadLevelData()
	{
		switch (Method)
		{
			case LoadMethod.resources:
				return LoadFromResources(LoadParam);

			case LoadMethod.copypaste:
				return LoadFromString(LoadParam);

			case LoadMethod.local:
			case LoadMethod.url:
			case LoadMethod.none:
			default:
				Debug.LogError($"Failed to load level, invalid SelectMethod: {_method}.");
				return null;
		}
	}

	public enum LoadMethod
	{
		none = 0,
		resources = 1, // Files included in build.
		local = 2, // Files in user's computer.
		copypaste = 3, // Text copied to input field.
		url = 4, // Text loaded from url.
	}



	#region Load Methods
	/// <summary> Loads level data from a text file located in the Resources folder.</summary>
	/// <param name="fileName"> Input the file name without .txt, located in Assets/Resources/Levels/ folder.</param>
	public LevelData LoadFromResources(string fileName)
	{
		TextAsset txtFile = Resources.Load<TextAsset>($"Levels/{fileName}");
		return StringToLevelData(txtFile.text);
	}


	/// <summary> Loads level data from copy pasted text.</summary>
	/// <param name="levelTxtData"> Raw text data containing line breaks.</param>
	public LevelData LoadFromString(string levelTxtData)
	{
		return StringToLevelData(levelTxtData);
	}


	/// <summary> Converts string, loaded from a file, to a usable level data format.</summary>
	/// <param name="txt"> Raw text data containing line breaks.</param>
	/// <returns> Usable level data byte[0 = ground height points, 1 = texture ids][world row][world column]</returns>
	public LevelData StringToLevelData(string txt)
	{
		// Split raw text into rows.
		return StringToLevelData(txt.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
	}


	/// <summary> Converts string, loaded from a file, to a usable level data format.</summary>
	/// <param name="txt"> Raw text data split into rows.</param>
	/// <returns> Usable level data byte[0 = ground height points, 1 = texture ids][world row][world column]</returns>
	public LevelData StringToLevelData(string[] txt)
	{
		// Find the shortest row in the txt, before the change the texture data, in case they all aren't equal. Cut them based on the shortest one.
		int width = int.MaxValue;

		for (int i = 0; i < txt.Length; i++)
		{
			if (txt[i].Length <= 1) { break; }
			if (txt[i].Length < width) { width = txt[i].Length; }
		}

		// Find the amount of ground height rows before single character row which indicates change to texture data.
		int rows = 0;
		for (int i = 0; i < txt.Length; i++)
		{
			if (txt[i].Length == 1) { break; }
			rows++;
		}


		// byte[0 = ground height points, 1 = texture ids][world row][world column].
		byte[][][] meshData = new byte[2][][];

		// Ground height points.
		// First rows before a one character row indicating change.
		meshData[0] = new byte[rows][];

		// Texture ids.
		// Rows after the first one character row.
		meshData[1] = new byte[rows - 1][];


		// Convert string format into individual bytes of ground height data.
		for (int i = 0; i < meshData[0].Length; i++)
		{
			meshData[0][i] = new byte[width];
			for (int j = 0; j < width; j++)
			{
				// Ignore first 32 characters from the ASCII table, so that space = 0, ! = 1 etc.
				meshData[0][i][j] = (byte)(Convert.ToByte(txt[i][j]) - 32);
			}
		}

		// Get treespawns from texture data, each empty tile should be possible tree spawn.
		List<(int, int)> treeSpawns = new List<(int, int)>();

		// Reduce width by one match the tile count, height vertex count is one higher.
		width--;

		// Convert string format into individual bytes of texture id data.
		for (int i = 0; i < meshData[1].Length; i++)
		{
			meshData[1][i] = new byte[width];
			for (int j = 0; j < width; j++)
			{
				// Ignore first 32 characters from the ASCII table, so that space = 0, ! = 1 etc.
				meshData[1][i][j] = (byte)(Convert.ToByte(txt[i + rows + 1][j]) - 32);
				if (meshData[1][i][j] == 0) { treeSpawns.Add((j, i)); }
			}
		}


		// Object data.
		// Rows after the second one character row (2 one char rows, and textures have one less than height = -3).
		int[][] objectData = new int[txt.Length - ((rows * 2) + 1)][];

		// Convert string format into object-location data.
		for (int i = 0; i < objectData.Length; i++)
		{
			// Split text row into separate values.
			string[] values = txt[i + (rows * 2) + 1].Split(",").Select(s => s.Trim()).ToArray();

			objectData[i] = new int[values.Length];

			for (int j = 0; j < values.Length; j++)
			{
				// Convert string to int.
				int.TryParse(values[j], out objectData[i][j]);
			}
		}

		return new LevelData(meshData, objectData, treeSpawns.ToArray(), _tileSize, _unitHeight);
	}
	#endregion
}