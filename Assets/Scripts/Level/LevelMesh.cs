using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMesh : Singleton<LevelMesh>
{
	#region Variables

	// Component references.
	[SerializeField] private MeshRenderer _renderer;
	[SerializeField] private MeshFilter _filter;
	[SerializeField] private Mesh _mesh;
	[SerializeField] private Material _material;
	[SerializeField] private MeshCollider _collider;


	[Tooltip("Used when adding normals to the mesh. Default direction they are facing at.")]
	[HideInInspector][SerializeField] private Vector3 _defaultNormalDirection = new Vector3(0f, 0f, 1f);


	// Material texture variables.
	[Space(10)]
	[Tooltip("Pixel size of the tile textures on the material.")]
	[SerializeField] private int _texturePixelSize = 128;

	[Tooltip("Pixel size of the color squares at the top of the material.")]
	[SerializeField] private int _textureColorSize = 16;

	// Amount of tile textures per row on the material. Automatically calculated in code.
	private int _texturesPerRow = 8;


	// Raw mesh data, loaded from the level file.
	private List<Vector3> _vertices = new List<Vector3>();
	private List<Vector2> _uvs = new List<Vector2>();
	private List<Vector3> _normals = new List<Vector3>();
	private List<int> _triangles = new List<int>();

	// The amount of vertices each tile has.
	private readonly int _tileVerticeCount = 4;

	// Percentage of each pixel on the material texture. Used to calculate UVs.
	private float _pixelPercentageX, _pixelPercentageY = 0f;

	#endregion


	public void Reset()
	{
		// Set components correctly
		_mesh = new Mesh();
		_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		_filter.mesh = _mesh;
		_renderer.material = _material;

		// Wipe previously loaded data.
		_vertices = new List<Vector3>();
		_uvs = new List<Vector2>();
		_normals = new List<Vector3>();
		_triangles = new List<int>();
	}


	[SerializeField] private string _testLvlName = "test01";
	[ContextMenu("Test Load Level")]
	public void TestLoad()
	{
		Reset();
		LevelLoader.Instance.SelectLevel(_testLvlName, LevelLoader.LoadMethod.resources);

		GameManager.Instance.UnloadLevel();
		GameManager.Instance.DisplayLoadLevel(false,false);
	}

	/// <summary> Generates new mesh based on previously loaded GameManager.Instance.LevelData.</summary>
	public void GenerateLevel()
	{
		// Calculate percentage for each pixel on the material texture.
		_pixelPercentageX = 1.00000f / _material.mainTexture.width;
		_pixelPercentageY = 1.00000f / _material.mainTexture.height;

		_texturesPerRow = Mathf.FloorToInt(_material.mainTexture.width / _texturePixelSize);

		LoadData();
		GenerateLevelMesh();
		SetupCollider();
	}



	#region Level Data To Mesh Data

	// Shortcuts to get LevelData.
	private LevelData Data => GameManager.Instance.LevelData;
	private byte[][] HeightData => Data.HeightData;
	private byte[][] TextureData => Data.TextureData;
	private float TileSize => Data.TileSize;
	private float UnitHeight => Data.UnitHeight;

	/// <summary> Loads data from LevelData to lists for mesh.</summary>
	private void LoadData()
	{
		for (int x = 0; x < Data.WidthX; x++)
		{
			for (int y = 0; y < Data.WidthY; y++)
			{
				AddTileVertices(x, y);
				AddTileUvs(x, y);
				AddTileNormals();
				AddTileTriangles(x, y);
			}
		}
	}

	private void AddTileVertices(int x, int y)
	{
		// Set vertices to the world based on the _tileSize and _unitHeight multipliers. 4 per tile.
		// Multiply X-axis with -1f to flip world correctly.
		_vertices.Add(new Vector3(x * -TileSize, HeightData[y][x] * UnitHeight, y * TileSize));
		_vertices.Add(new Vector3((x + 1) * -TileSize, HeightData[y][x + 1] * UnitHeight, y * TileSize));
		_vertices.Add(new Vector3(x * -TileSize, HeightData[y + 1][x] * UnitHeight, (y + 1) * TileSize));
		_vertices.Add(new Vector3((x + 1) * -TileSize, HeightData[y + 1][x + 1] * UnitHeight, (y + 1) * TileSize));
	}

	private void AddTileUvs(int x, int y)
	{
		// Check if tile is using special texture or default colors.
		// 0 = default color at the top of texture sheet based on the tile height.
		if (TextureData[y][x] == 0)
		{
			// Set each vertice at the middle of the color equal to the height.
			AddUv((HeightData[y][x] + 0.5f) * _textureColorSize, 0.5f * _textureColorSize);
			AddUv((HeightData[y][x + 1] + 0.5f) * _textureColorSize, 0.5f * _textureColorSize);
			AddUv((HeightData[y + 1][x] + 0.5f) * _textureColorSize, 0.5f * _textureColorSize);
			AddUv((HeightData[y + 1][x + 1] + 0.5f) * _textureColorSize, 0.5f * _textureColorSize);
		}
		else
		{
			// Calculate index in the texture.
			int textureColumn = TextureData[y][x] % _texturesPerRow;
			int textureRow = Mathf.FloorToInt(TextureData[y][x] / _texturesPerRow);

			// Calculate pixel location of top left corner of the texture.
			int uvStartx = (textureColumn * _texturePixelSize);
			int uvStarty = (textureRow * _texturePixelSize) + _textureColorSize;

			// Add UVs (top left) -> (top right) -> (bottom left) -> (bottom right).
			AddUv(uvStartx + 1, uvStarty + 1);
			AddUv(uvStartx - 1 + _texturePixelSize, uvStarty + 1);
			AddUv(uvStartx + 1, uvStarty - 1 + _texturePixelSize);
			AddUv(uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
		}
	}
	/// <summary> Adds new UV. Converts pixel location into float. </summary>
	/// <param name="x">Location within texture in pixels. X-axis.</param>
	/// <param name="y">Location within texture in pixels. Y-axis.</param>
	private void AddUv(float x, float y)
	{ _uvs.Add(new Vector2(x * _pixelPercentageX, 1.0000f - (y * _pixelPercentageY))); }

	private void AddTileNormals()
	{
		for (int i = 0; i < _tileVerticeCount; i++)
		{ _normals.Add(_defaultNormalDirection); }
	}

	private void AddTileTriangles(int x, int y)
	{
		int startVertex = TileStartVertexId(x, y);
		// Top left triangle. (Top left) -> (Top right) -> (Bottom left).
		_triangles.Add(startVertex + 0);
		_triangles.Add(startVertex + 1);
		_triangles.Add(startVertex + 2);

		// Bottom right triangle. (Bottom right) -> (Bottom left) -> (Top right).
		_triangles.Add(startVertex + 3);
		_triangles.Add(startVertex + 2);
		_triangles.Add(startVertex + 1);
	}



	/// <summary> Return the start index of a tile's vertices.</summary>
	private int TileStartVertexId(int row, int column)
	{ return ((Data.WidthY * row) + column) * _tileVerticeCount; }


	#endregion



	#region Mesh Generating
	public void GenerateLevelMesh()
	{
		UpdateVertices();
		UpdateUvs();
		UpdateNormals();
		UpdateTriangles();
		RecalculateMesh();
	}

	private void UpdateVertices()
	{ _mesh.vertices = _vertices.ToArray(); }
	private void UpdateUvs()
	{ _mesh.uv = _uvs.ToArray(); }
	private void UpdateNormals()
	{ _mesh.normals = _normals.ToArray(); }
	private void UpdateTriangles()
	{ _mesh.triangles = _triangles.ToArray(); }

	private void RecalculateMesh()
	{
		_mesh.RecalculateNormals();
		_mesh.RecalculateUVDistributionMetrics();
	}
	#endregion




	private void SetupCollider()
	{
		_collider.sharedMesh = _mesh;
		_collider.convex = false;
	}
}