using Godot;
using System;
using System.Collections.Generic;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class GridManager : Node2D
	{
		private static GridManager instance;
		public List<List<Node2D>> grid;

		public static GridManager GetInstance()
		{
			if (instance == null) instance = new GridManager();
			return instance;
		}
		public override void _Ready()
		{
			instance = this;
			base._Ready();
		}
		public void PlaceObjectOnGrid()
		{
			int lFirstListCount = grid.Count;
			for (int i = 0; i < lFirstListCount; i++)
			{
				int lSecondListCount = grid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					Node2D lNode = grid[i][j];
					lNode.GlobalPosition = new Vector2I(i * Utils.MAP_CASE_SCALE, j * Utils.MAP_CASE_SCALE);
				}
			}

		}
	}
}
