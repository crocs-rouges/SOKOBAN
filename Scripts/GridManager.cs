using Godot;
using System;
using System.Collections.Generic;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class GridManager : Node2D
	{
		//for test
		private List<List<int>> testLoadingGrid = new List<List<int>>(){
		new List<int> { 0, 0, 0, 0, 0, 0, 0},
		new List<int> { 0, 3, 9, 9, 9, 9, 0},
		new List<int> { 0, 9, 9, 9, 9, 9, 0},
		new List<int> { 0, 9, 9, 9, 9, 9, 0},
		new List<int> { 0, 9, 2, 9, 9, 9, 0},
		new List<int> { 0, 1, 9, 9, 9, 9, 0},
		new List<int> { 0, 0, 0, 0, 0, 0, 0}
		};
		//


		private static GridManager instance;
		public static List<List<Node2D>> staticGrid = new List<List<Node2D>>();
		public List<List<Node2D>> movableGrid = new List<List<Node2D>>();

		[Export] private PackedScene wallscene;
		[Export] private PackedScene dicescene;
		[Export] private PackedScene playerscene;
		[Export] private PackedScene finishzonescene;

		public static GridManager GetInstance()
		{
			if (instance == null) instance = new GridManager();
			return instance;
		}
		public override void _Ready()
		{
			instance = this;
			base._Ready();
			ConvertListIntToNodeList(testLoadingGrid);
			// PlaceObjectOnGrid();
			GridSnap();
			movableGrid = staticGrid;
		}
		public void ConvertListIntToNodeList(List<List<int>> pGrid)
		{
			//convert the list of int into a list of node2D easier to move
			int lFirstListCount = pGrid.Count;
			if (lFirstListCount == 0) return;
			Vector2 lPosition;
			for (int i = 0; i < lFirstListCount; i++)
			{
				staticGrid.Add(new List<Node2D>());

				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					staticGrid[i].Add(null);
				}
				GD.Print(staticGrid[i].Count);
			}
			for (int i = 0; i < lFirstListCount; i++)
			{
				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					GD.Print(pGrid[j][i]);
					lPosition = new Vector2(i * Utils.MAP_CASE_SCALE, j * Utils.MAP_CASE_SCALE);
					switch (pGrid[j][i])
					{
						case 0:
							staticGrid[j][i] = CreateObject(wallscene, lPosition);
							break;
						case 1:
							staticGrid[j][i] = CreateObject(playerscene, lPosition);
							break;
						case 2:
							staticGrid[j][i] = CreateObject(dicescene, lPosition);
							break;
						case 3:
							staticGrid[j][i] = CreateObject(finishzonescene, lPosition);
							break;
					}
				}
				GD.Print("end line");
			}
			GD.Print(staticGrid);
		}
		public void PlaceFromGrid(List<List<Node2D>> pGrid)
		{
			// place object on the game based on the grid placement
			int lFirstListCount = pGrid.Count;
			if (lFirstListCount == 0) return;
			Vector2 lPosition;
			for (int i = 0; i < lFirstListCount; i++)
			{
				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					lPosition = new Vector2(i * Utils.MAP_CASE_SCALE, j * Utils.MAP_CASE_SCALE);
					pGrid[i][j].GlobalPosition = lPosition;
				}
			}
		}
		// public void PlaceObjectOnGrid()
		// {
		// 	//place movable objects on the game based on the grid
		// 	foreach (Node2D lObject in GetChildren())
		// 	{
		// 		if (lObject is not Movable) continue;
		// 	}
		// }
		#region Move
		public void MoveFromPos(Vector2 pPosition, Vector2I pDirection)
		{
			Vector2I lPos = PositionToGrid(pPosition);
			Vector2I lEndPos = lPos + pDirection;
			MoveOnGrid(lPos.X, lPos.Y, lEndPos.X, lEndPos.Y);
		}
		public Vector2I PositionToGrid(Vector2 pPosition)
		{
			//convert
			int lPosX = (int)Mathf.Round(pPosition.X / Utils.MAP_CASE_SCALE);
			int lPosY = (int)Mathf.Round(pPosition.Y / Utils.MAP_CASE_SCALE);
			return new Vector2I(lPosX, lPosY);
		}
		public void MoveOnGrid(int pStartX, int pStartY, int pEndX, int pEndY)
		{
			//move object from start position to end position on the movable grid
			movableGrid[pEndY][pEndX] = movableGrid[pStartY][pStartX];
			movableGrid[pStartY][pStartX] = staticGrid[pStartY][pStartX];
			GD.Print($"Y first move from {pStartY} {pStartX} to {pEndY} {pEndX}");
			// GD.Print($"X first move from {pStartX} {pStartY} to {pEndX} {pEndY}");
			GD.Print(movableGrid[pEndY][pEndX].Name);
		}
		#endregion

		public void GridSnap()
		{
			foreach (Node2D lObject in GetChildren())
			{
				float lPosX = Mathf.Round(lObject.GlobalPosition.X / Utils.MAP_CASE_SCALE) * Utils.MAP_CASE_SCALE;
				float lPosY = Mathf.Round(lObject.GlobalPosition.Y / Utils.MAP_CASE_SCALE) * Utils.MAP_CASE_SCALE;
				lObject.GlobalPosition = new Vector2(lPosX, lPosY);
			}
		}
		private Node2D CreateObject(PackedScene pScene, Vector2 pPosition)
		{
			Node2D lObject = pScene.Instantiate() as Node2D;
			AddChild(lObject);
			lObject.GlobalPosition = pPosition;
			return lObject;
		}
	}
}
