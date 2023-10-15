using UnityEngine;

[System.Serializable]
public class LevelInfo
{
	[SerializeField] private string _levelName = "";
	[SerializeField] private Sprite _levelImage;
	[SerializeField] private LevelLoader.LoadMethod _loadMethod = LevelLoader.LoadMethod.resources;

	public string LevelName { get => _levelName; set => _levelName = value; }
	public Sprite LevelImage { get => _levelImage; set => _levelImage = value; }
	public LevelLoader.LoadMethod LoadMethod { get => _loadMethod; set => _loadMethod = value; }

	/// <summary> Selects this level to LevelLoader. Will be loaded when next game is started.</summary>
	public void SelectLevel()
	{ LevelLoader.Instance.SelectLevel(LevelName, LoadMethod); }
}