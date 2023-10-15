using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Subscribe to one of the InputControllers to receive inputs. Use actions to connect inputs with behaviour.</summary>
public class InputReceiver : MonoBehaviour
{
	public event Action<bool> OnShoot;
	public event Action<bool> OnRoll;
	public event Action<bool> OnSpecial;
	public event Action<float> OnSpeed;
	public event Action<float> OnBrake;
	public event Action<float> OnSteer;

	public virtual void Shoot(bool state) { OnShoot?.Invoke(state); }
	public virtual void Roll(bool state) { OnRoll?.Invoke(state); }
	public virtual void Special(bool state) { OnSpecial?.Invoke(state); }
	public virtual void Speed(float state) { OnSpeed?.Invoke(state); }
	public virtual void Brake(float state) { OnBrake?.Invoke(state); }
	public virtual void Steer(float state) { OnSteer?.Invoke(state); }


	/// <summary> InputController this InputReceiver currently listens.</summary>
	public InputController MyController { get; set; }

	/// <summary> Stop listening current InputController.</summary>
	public void Unsubscribe() { MyController?.Unsubscribe(this); }
}