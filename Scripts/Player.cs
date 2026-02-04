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
		}
		public void WhipPull()
		{
			rayCast.TargetPosition *= 2;
			rayCast.ForceRaycastUpdate();
			if (rayCast.IsColliding())
			{
				Node2D lCollider = rayCast.GetCollider() as Node2D;
				Node2D lParent = lCollider.GetParent() as Node2D;
				if (lParent is Dice lDice) lDice.WhipPull();
			}
			rayCast.TargetPosition /= 2;
		}
	}
}