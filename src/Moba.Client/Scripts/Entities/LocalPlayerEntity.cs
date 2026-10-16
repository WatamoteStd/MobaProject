using Godot;
using System;

public partial class LocalPlayerEntity : RemotePlayerEntity
{
	private PlayerController _controller;

	public override void _Ready()
	{

		_controller = GetNode<PlayerController>("PlayerController");
		_controller.Init(this);
		_controller.OnClickMove += CSP_SetMoveTarget;

	}

	public void CSP_SetMoveTarget(Vector3 point)
	{
		GD.Print($"LocalPlayer : SetMoveTargetTriggered. X:{point.X}, Z:{point.Z}");
		
		_moveTarget = point;
		CurrentState = States.Move;

	}

	public override void _PhysicsProcess(double delta)
	{
		
		switch(CurrentState)
		{
			
			case States.Idle:

			break;

			case States.Move:
				{
					Move((float)delta);
				}
			break;

		}

	}

	private void Move(float delta)
	{

		if(GlobalPosition.DistanceSquaredTo(_moveTarget) < 0.15f) { CurrentState = States.Idle; return;}


		Vector3 direction = (_moveTarget - GlobalPosition).Normalized();

		Vector3 velocity = direction * _moveSpeed;
		Velocity = velocity;

		MoveAndSlide();


	}


}
