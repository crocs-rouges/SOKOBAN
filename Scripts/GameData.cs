using Godot;
using System.Collections.Generic;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	[System.Serializable]
	public partial class GameData : Node
	{
public int indexLevel;






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
