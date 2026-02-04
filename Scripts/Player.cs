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
			Vector2 lDirection = Vector2.Zero;
			if (Input.IsActionJustPressed("left")) lDirection = Vector2.Left;
			else if (Input.IsActionJustPressed("right")) lDirection = Vector2.Right;
			else if (Input.IsActionJustPressed("up")) lDirection = Vector2.Up;
			else if (Input.IsActionJustPressed("down")) lDirection = Vector2.Down;
			if (lDirection != Vector2.Zero) Move(lDirection);
		}
	}
}
