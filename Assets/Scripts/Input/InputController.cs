using System.Collections;
using System.Collections.Generic;
using System;

/// <summary> Abstract class for input source. To track certain inputs and forward them to subscribed Inputreceivers.</summary>
public abstract class InputController
{
	/// <summary> Called from InputManager every frame. </summary>
	public abstract void UpdateInputs();


	protected event Action<bool> OnShoot;
	protected event Action<bool> OnRoll;
	protected event Action<bool> OnSpecial;
	protected event Action<float> OnSpeed;
	protected event Action<float> OnBrake;
	protected event Action<float> OnSteer;


	protected virtual void Shoot(bool state) { OnShoot?.Invoke(state); }
	protected virtual void Roll(bool state) { OnRoll?.Invoke(state); }
	protected virtual void Special(bool state) { OnSpecial?.Invoke(state); }
	protected virtual void Speed(float state) { OnSpeed?.Invoke(state); }
	protected virtual void Brake(float state) { OnBrake?.Invoke(state); }
	protected virtual void Steer(float state) { OnSteer?.Invoke(state); }
	
	
	/// <summary> Subscribe InputReceiver to listen this InputController.</summary>
	public void Subscribe(InputReceiver receiver)
	{
		OnShoot += receiver.Shoot;
		OnRoll += receiver.Roll;
		OnSpecial += receiver.Special;
		OnSpeed += receiver.Speed;
		OnBrake += receiver.Brake;
		OnSteer += receiver.Steer;

		receiver.MyController = this;
	}

	/// <summary> Unsubscribe InputReceiver from listening this InputController.</summary>
	public void Unsubscribe(InputReceiver receiver)
	{
		OnShoot -= receiver.Shoot;
		OnRoll -= receiver.Roll;
		OnSpecial -= receiver.Special;
		OnSpeed -= receiver.Speed;
		OnBrake -= receiver.Brake;
		OnSteer -= receiver.Steer;

		receiver.MyController = null;
	}
}