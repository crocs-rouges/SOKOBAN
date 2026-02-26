using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class Bloc : Node2D
	{

		public override void _Ready()
		{
			base._Ready();
		}
		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			base._Process(pDelta);
		}		
	}
}
