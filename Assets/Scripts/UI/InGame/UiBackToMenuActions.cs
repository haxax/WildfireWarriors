using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiBackToMenuActions : MonoBehaviour
{
	[Tooltip("MenuState assosiated the same UI state.")]
	[SerializeField] private MenuState _menuState;

	[Space(10)]
	[Tooltip("Description is updated to this component.")]
	[SerializeField] private TMP_Text _descriptionComponent;
	[TextArea][SerializeField] private string _description;
	[Tooltip("Use {0} to insert tracked amount of poolables of this TrackKey to description.")]
	[SerializeField] private string _descriptionCountKey = "Tree";

	[Space(10)]
	[Tooltip("Time.timeScale is set to 0 when this UI is open, if set to true.")]
	[SerializeField] private bool _stopTimeScale = true;

	[Tooltip("This UI becomes active when game ends if set to true.")]
	[SerializeField] private bool _openOnEndGame = false;

	private void Awake()
	{
		// Track given key.
		if (_descriptionCountKey != "")
		{ ObjectManager.Instance.OnSpawnCountChange += UpdateDescriptionToIncludeTreeCount; }

		// Track game state.
		if (_openOnEndGame)
		{ GameManager.Instance.OnEndGame += (() => MenuManager.Instance.SetMenuState(_menuState.StateType)); }
	}

	/// <summary> Returns back to game UI state.</summary>
	public void BackToGame()
	{ MenuManager.Instance.SetMenuState(MenuStateType.gameUI); }

	/// <summary> Returns to main menu UI state.</summary>
	public void BackToMenu()
	{ GameManager.Instance.GoToMainMenu(); }

	/// <summary> Updates the description with the tracked amount of poolables.</summary>
	public void UpdateDescriptionToIncludeTreeCount(string key, int amount)
	{
		if (_descriptionCountKey == key)
		{ _descriptionComponent.text = string.Format(_description, amount).Replace("\\n", "\r\n"); ; }
	}

	private void OnEnable()
	{
		if (_stopTimeScale) { Time.timeScale = 0f; }
	}
	private void OnDisable()
	{
		if (_stopTimeScale) { Time.timeScale = 1.0f; }
	}
}