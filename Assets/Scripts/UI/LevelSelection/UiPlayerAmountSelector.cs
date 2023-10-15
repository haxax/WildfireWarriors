using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> To select how many players is spawned to the game.</summary>
public class UiPlayerAmountSelector : MonoBehaviour
{
	[SerializeField] private Slider _slider;
	[SerializeField] private TMP_Text _textComponent;
	[SerializeField] private string _playerAmountTxt = "Players: {0}";

	private void Start()
	{
		// Listen slider.
		_slider.onValueChanged.AddListener(UpdatePlayerAmount);

		// Set range.
		_slider.minValue = 1;
		_slider.maxValue = PlayerManager.Instance.SupportedPlayerCount;
		_slider.value = 1;

		UpdatePlayerAmount(_slider.value);
	}

	public void UpdatePlayerAmount(float amount)
	{
		// Update PlayerManager.
		PlayerManager.Instance.ActivePlayerCount = Mathf.RoundToInt(amount);

		// Update text.
		_textComponent.text = string.Format(_playerAmountTxt, PlayerManager.Instance.ActivePlayerCount);
	}
}
