using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCustomLevelActions : MonoBehaviour
{
	[SerializeField] private Button _playButton;

	public void OnEnable()
	{
		// User has to input something to InputField before Play can be performed.
		_playButton.enabled = false;
	}

	public void Play()
	{ GameManager.Instance.StartLevel(); }

	public void Back()
	{ GameManager.Instance.GoToMainMenu(); }

	public void SetLevelData(string levelData)
	{
		if (levelData.Length == 0) { _playButton.enabled = false; return; }

		LevelLoader.Instance.SelectLevel(levelData, LevelLoader.LoadMethod.copypaste);
		_playButton.enabled = true;
	}
}