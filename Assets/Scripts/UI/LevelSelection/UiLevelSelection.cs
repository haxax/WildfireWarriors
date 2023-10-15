using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Spawns selectable levels to the UI.</summary>
public class UiLevelSelection : MonoBehaviour
{
	[SerializeField] private RectTransform _targetSection;
	[SerializeField] private UiLevelElement _levelElementPrefab;
	[SerializeField] private int _levelsPerRow = 3;
	[SerializeField] private Vector2 _areaBorderSize = new Vector2(0.2f, 0.2f);
	[Tooltip("Levels available normally.")]
	[SerializeField] private List<LevelInfo> _selectableLevels = new List<LevelInfo>();
	[Tooltip("Levels available if using WebGL.")]
	[SerializeField] private List<LevelInfo> _selectableLevelsWebGL = new List<LevelInfo>();

	public List<LevelInfo> SelectableLevels
	{
		get
		{
#if UNITY_WEBGL
			return _selectableLevelsWebGL;
#else
			return _selectableLevels;
#endif
		}
	}

	/// <summary> Created level buttons.</summary>
	public List<UiLevelElement> Levels { get; } = new List<UiLevelElement>();

	/// <summary> Currently selected level in Levels.</summary>
	public int SelectedLevel { get; private set; } = 0;


	private void Awake()
	{
		GameManager.Instance.OnMainMenu += (() => SelectNextLevel(0));
		SpawnLevelElements();
		SelectNextLevel(0);
	}

	/// <summary> Creates a select button for each selectable level.</summary>
	private void SpawnLevelElements()
	{
		// Destroy possible previously loaded level buttons.
		if (Levels.Count > 0)
		{
			for (int i = Levels.Count - 1; i >= 0; i--)
			{ Destroy(Levels[i]); }
			SelectedLevel = 0;
		}
		Levels.Clear();


		// Calculate how much anchor space each button should take.
		float sizeX = (1f - ((1f + _levelsPerRow) * _areaBorderSize.x)) / _levelsPerRow;
		float sizeY = (1f - ((1f + Mathf.CeilToInt(SelectableLevels.Count / (float)_levelsPerRow)) * _areaBorderSize.y)) / Mathf.CeilToInt(SelectableLevels.Count / (float)_levelsPerRow);

		// Spawn each level button.
		for (int i = 0; i < SelectableLevels.Count; i++)
		{
			UiLevelElement newElement = Instantiate(_levelElementPrefab, _targetSection);

			// Set anchors so that the buttons are evenly distributed on the target area.
			newElement.RectTransform.anchorMin = new Vector2(
				(((i % _levelsPerRow) + 1f) * _areaBorderSize.x) + (((i % _levelsPerRow) + 0.5f) * sizeX),
				1f - (((Mathf.FloorToInt(i / (float)_levelsPerRow) + 1f) * _areaBorderSize.y) + ((Mathf.FloorToInt(i / (float)_levelsPerRow) + 0.5f) * sizeY)));

			newElement.RectTransform.anchorMax = newElement.RectTransform.anchorMin;
			newElement.RectTransform.anchoredPosition = Vector2.zero;

			// Set which level is linked to the button.
			newElement.SetLevel(SelectableLevels[i], i);
			Levels.Add(newElement);
		}
	}

	public void SelectNextLevel(int direction = 1)
	{
		SelectedLevel = (int)Mathf.Repeat(SelectedLevel + Mathf.Sign(direction), Levels.Count - 1);

		Levels[SelectedLevel].SelectLevel();
	}
}