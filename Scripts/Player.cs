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
			float lDelta = (float)pDelta;
			base._Process(pDelta);
			if (Input.IsActionJustPressed("left")) Left();
			if (Input.IsActionJustPressed("right")) Right();
			if (Input.IsActionJustPressed("up")) Up();
			if (Input.IsActionJustPressed("down")) Down();
		}
	}
}
