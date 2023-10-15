using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
	[SerializeField] private InputSetup _setup;
	public InputSetup Setup => _setup;

	/// <summary> Active controllers: connected joysticks and player specific keyboards.</summary>
	public List<PlayerInputController> Controllers { get; set; } = new List<PlayerInputController>();

	protected override void Awake()
	{
		_setup.SetupControls();
		base.Awake();
	}

	private void Update()
	{ Controllers.ForEach(x => x.UpdateInputs()); }
}