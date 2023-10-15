using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Button for every selectable level.</summary>
public class UiLevelElement : MonoBehaviour
{
	[SerializeField] private Image _levelImage;
	[SerializeField] private TMP_Text _levelName;
	[SerializeField] private RectTransform _rectTransform;

	public RectTransform RectTransform => _rectTransform;
	public LevelInfo LevelInfo { get; private set; }
	public int LevelId { get; private set; } = -1;


	/// <summary> Sets level info to this button.</summary>
	public void SetLevel(LevelInfo levelInfo, int levelId)
	{
		LevelInfo = levelInfo;
		LevelId = levelId;
		_levelImage.sprite = LevelInfo.LevelImage;
		_levelName.text = LevelInfo.LevelName;
	}

	/// <summary> Selects this level to LevelLoader. Will be loaded when next game is started.</summary>
	public void SelectLevel()
	{ LevelInfo.SelectLevel(); }
}
