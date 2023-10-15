using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary> Tracks if an Input button is pressed and invokes events based on that.</summary>
public class InputButtonPressTracker : MonoBehaviour
{
	[Tooltip("Button name defined in editor's InputManager.")]
	[SerializeField] private string _buttonName = "";

	[SerializeField] private UnityEvent _onButtonDown;
	[SerializeField] private UnityEvent _onButtonUp;

	public UnityEvent OnButtonDown => _onButtonDown;
	public UnityEvent OnButtonUp => _onButtonUp;


	private void Update()
	{
		if (_buttonName == "") { this.enabled = false; return; }

		if (Input.GetButtonDown(_buttonName)) { OnButtonDown?.Invoke(); }
		if (Input.GetButtonUp(_buttonName)) { OnButtonUp?.Invoke(); }
	}
}