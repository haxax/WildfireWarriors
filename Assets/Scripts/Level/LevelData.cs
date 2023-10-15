using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelData
{
	public LevelData(byte[][][] meshData, int[][] objectData, (int, int)[] treeSpawns, float tileSize, float unitHeight)
	{
		MeshData = meshData;
		ObjectData = objectData;
		TreeSpawns = treeSpawns;
		TileSize = tileSize;
		UnitHeight = unitHeight;

		CheckObjects();
	}


	#region Data
	/// <summary>
	/// [0][Y][X] = Vertex height.
	/// [1][Y][X] = Texture id.
	/// </summary>
	public byte[][][] MeshData { get; private set; }

	/// <summary>
	/// [Y][X] -location. X is inverted to negative in world space.
	/// </summary>
	public byte[][] HeightData => MeshData[0];

	/// <summary>
	/// [Y][X] -location. X is inverted to negative in world space.
	/// </summary>
	public byte[][] TextureData => MeshData[1];

	/// <summary>
	/// [i][0] = Object id.
	/// [i][1] = X-axis location.
	/// [i][2] = Y-axis location.
	/// [i][3] = Rotation (EulerAngle.Y).
	/// [i][4] = Chances to spawn, 0-1000 where: 0 = 0% & 1000 = 100%.
	/// [i][5...] = Custom data.
	/// </summary>
	public int[][] ObjectData { get; private set; }
	// Defined above. All from 0 to 4 are required.
	public readonly int NecessaryObjectDataCount = 5;

	/// <summary> Locations for trees to spawn. </summary>
	public (int, int)[] TreeSpawns { get; private set; }



	/// <summary> Width between mesh vertices. </summary>
	public float TileSize { get; private set; }

	/// <summary> Level's height data is limited to 256 steps. Height per step. </summary>
	public float UnitHeight { get; private set; }
	#endregion


	#region Level Info
	/// <summary>The amount of tiles per row (X-axis).</summary>
	public int WidthX => TextureData[0].Length;

	/// <summary>The amount of tiles per column (Y-axis).</summary>
	public int WidthY => TextureData.Length;

	/// <summary> Returns the height of the ground at the given coordinate.</summary>
	public float GetGroundHeightAt(int x, int y)
	{
		// Make sure coordinates are within world bounds.
		x = Mathf.Clamp(x, 0, WidthX);
		y = Mathf.Clamp(y, 0, WidthY);

		// Height at the middle of a tile is calculated by finding the higher average between diagonal corners.
		float diagonalA = ((float)HeightData[y][x] + HeightData[y + 1][x + 1]) / 2f;
		float diagonalB = ((float)HeightData[y + 1][x] + HeightData[y][x + 1]) / 2f;

		return Mathf.Max(diagonalA, diagonalB) * UnitHeight;
	}

	/// <summary> Returns the world location at the given coordinate. Note that x coordinate is negative in world space.</summary>
	public Vector3 GetGroundLocationAt((int, int) coordinate)
	{ return GetGroundLocationAt(coordinate.Item1, coordinate.Item2); }

	/// <summary> Returns the world location at the given coordinate, middle of the tile. Note that x coordinate is negative in world space.</summary>
	public Vector3 GetGroundLocationAt(int x, int y)
	{ return new Vector3((-(x + 0.5f)) * TileSize, GetGroundHeightAt(x, y), (y + 0.5f) * TileSize); }
	#endregion


	#region Sorted Objects
	/// <summary>(Y, X, rotation)</summary>
	public List<(int, int, int)> PlayerSpawnPoints { get; private set; } = new List<(int, int, int)>();


	/// <summary> This should be performed only once! Goes through objects, Validates values, sorts them to lists and such. </summary>
	private void CheckObjects()
	{
		for (int i = 0; i < ObjectData.Length; i++)
		{
			ValidateObject(i);
			SortObject(i);
		}
	}

	/// <summary> Makes sure object is defined correctly. </summary>
	private void ValidateObject(int i)
	{
		// Check if object has all necessary values defined. 
		if (ObjectData[i].Length < NecessaryObjectDataCount)
		{
			Debug.LogWarning($"Object #{i} not properly defined in level file, lacks data: {ObjectData[i]}");
			// If not, increase the array size keeping the existing values.
			int[] newArray = new int[NecessaryObjectDataCount];
			Array.Copy(ObjectData[i], newArray, ObjectData[i].Length);
			ObjectData[i] = newArray;
		}

		// Check if object type is defined in SpawnableTypes. If not, set to none (0).
		if (!Enum.IsDefined(typeof(PoolableType), ObjectData[i][0]))
		{
			Debug.LogWarning($"Object #{i} not properly defined in level file, invalid SpawnableType: {ObjectData[i][0]}");
			ObjectData[i][0] = 0;
		}

		// Set rotation to be within 0 to 360.
		ObjectData[i][3] = ObjectData[i][3] % 360;
		if (ObjectData[i][3] < 0) { ObjectData[i][3] += 360; }

		// Set chances to 0-1000 scale.
		ObjectData[i][4] = Mathf.Clamp(ObjectData[i][4], 0, 1000);
	}

	/// <summary> Sets object to a list by type if needed.</summary> <param name="i"></param>
	private void SortObject(int i)
	{
		switch ((PoolableType)ObjectData[i][0])
		{
			case PoolableType.playerSpawnPoint:
				PlayerSpawnPoints.Add((ObjectData[i][1], ObjectData[i][2], ObjectData[i][3]));
				return;
		}
	}

	#endregion
}