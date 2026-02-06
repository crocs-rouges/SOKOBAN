using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class Player : Movable
	{
		private static Player instance;


		public static Player GetInstance()
		{
			if (instance == null) instance = new Player();
			return instance;
		}
		public override void _Ready()
		{
			instance = this;
			base._Ready();
		}
		public override void _Process(double pDelta)
		{
			Vector2I lDirection = Vector2I.Zero;
			if (Input.IsActionJustPressed("Left")) lDirection = Vector2I.Left;
			else if (Input.IsActionJustPressed("Right")) lDirection = Vector2I.Right;
			else if (Input.IsActionJustPressed("Up")) lDirection = Vector2I.Up;
			else if (Input.IsActionJustPressed("Down")) lDirection = Vector2I.Down;
			if (lDirection != Vector2I.Zero) Move(lDirection);

			//whip feature / special feature
			if (Input.IsActionJustPressed("Whip")) WhipPull();

			//rotation
			if (Input.IsActionJustPressed("RotateLeft")) RotateRaycastRight(false);
			if (Input.IsActionJustPressed("RotateRight")) RotateRaycastRight(true);

		}
		public void WhipPull()
		{
			GD.Print("Whip pull");
			bool lIsMoving = false;
			rayCast.TargetPosition *= 2;
			rayCast.ForceRaycastUpdate();
			if (rayCast.IsColliding())
			{
				Node2D lCollider = rayCast.GetCollider() as Node2D;
				Node2D lParent = lCollider.GetParent() as Node2D;
				if (lParent is Dice lDice)
				{
					Vector2I lDirection = (Vector2I)((GlobalPosition - lDice.GlobalPosition) / Utils.MAP_CASE_SCALE);
					// lDirection /= Utils.MAP_CASE_SCALE;
					GD.Print("lDirection : " + lDirection + " direction : " + (GlobalPosition - lDice.GlobalPosition));
					if (lDirection.X > 1) lDirection.X = 1;
					else if (lDirection.X < -1) lDirection.X = -1;
					if (lDirection.Y > 1) lDirection.Y = 1;
					else if (lDirection.Y < -1) lDirection.Y = -1;
					lIsMoving = Move(lDirection);
					if (lIsMoving) lDice.WhipPull(lDirection);
				}
			}
			rayCast.TargetPosition *= -1;
			rayCast.ForceRaycastUpdate();
			if (!lIsMoving) rayCast.TargetPosition /= 2;
		}
	}
}