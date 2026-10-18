using Godot;
using Moba.Shared.UdpServer.Packets.Game;
using Moba.Shared.UdpServer.Teams;
using System;

public partial class PlayerController : Node
{   

	public event Action<Vector3> OnClickMove;
	
	[Export] private RayCast3D _cameraRaycast;
	private LocalPlayerEntity _localPlayer;
	private MatchTeams _localTeam;

	// FOR CAMERA MOVE
	private bool _isPanning = false;
	private Camera3D _camera;
	[Export] private float _panSpeed = 0.04f;

	public override void _Ready()
	{
		_camera = GetTree().GetFirstNodeInGroup("MainCamera") as Camera3D;
	}


	public void Init(LocalPlayerEntity locPlayer)
	{
		
		_localPlayer = locPlayer;
		if(_camera is null) _camera = GetTree().GetFirstNodeInGroup("MainCamera") as Camera3D;
		_localTeam = _localPlayer.Team;

	}

	public override void _Process(double delta)
	{
		UpdateMouseRaycast();
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right)
		{
			
			Vector3 clickPoint = GetClickPoint();
			clickPoint.Y = 0f;
			
			OnClickMove?.Invoke(clickPoint);

			C2S_MoveRequestPacket pck = new C2S_MoveRequestPacket
			{
				X = clickPoint.X, Z = clickPoint.Z
			};
			ClientUdp.Instance.GAME_SendMoveRequest(pck);
		}

		if(@event is InputEventMouseButton middleEvent && middleEvent.ButtonIndex == MouseButton.Middle)
		{
			_isPanning = middleEvent.Pressed;
		}

		if(_isPanning && @event is InputEventMouseMotion motionEvent)
		{
			MoveCamera(motionEvent.Relative);
		}

	}


	private Vector3 GetClickPoint()
	{
		
		_cameraRaycast.ForceRaycastUpdate();

		if(_cameraRaycast.IsColliding())
		{
			var point = _cameraRaycast.GetCollisionPoint();
			return point;
		}

		return Vector3.Zero;

	}

	private void UpdateMouseRaycast()
	{
		var viewport = GetViewport();
		var camera = viewport.GetCamera3D();

		var mousePos = viewport.GetMousePosition();

		float rayLength = 1000.0f;
		Vector3 from = camera.ProjectRayOrigin(mousePos);
		Vector3 to = from + camera.ProjectRayNormal(mousePos) * rayLength;

		_cameraRaycast.GlobalPosition = from;
		_cameraRaycast.TargetPosition = _cameraRaycast.ToLocal(to);
	}

	private void MoveCamera(Vector2 relative)
	{

		Vector3 right = _camera.GlobalBasis.X;

		Vector3 forward = -_camera.GlobalBasis.Z;
		forward.Y = 0;
		forward = forward.Normalized();

		Vector3 movement = (right * -relative.X + forward * relative.Y) * _panSpeed;

		_camera.GlobalPosition += movement;

	}

	
}
