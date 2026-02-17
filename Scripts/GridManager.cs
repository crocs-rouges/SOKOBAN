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

		[Export] private PackedScene wallScene;
		[Export] private PackedScene diceScene;
		[Export] private PackedScene playerScene;
		[Export] private PackedScene finishZoneScene;
		[Export] private PackedScene casinoScene;

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
		private void InitMovableGrid()
		{
			movableGrid = new List<List<Node2D>>();
			int lCount = staticGrid.Count;
			for (int i = 0; i < lCount; i++)
				movableGrid.Add(new List<Node2D>(staticGrid[i]));
		}
		private void InitStaticGrid(List<List<int>> pGrid)
		{
			int lFirstListCount = pGrid.Count;
			staticGrid = new List<List<Node2D>>();
			for (int i = 0; i < lFirstListCount; i++)
			{
				staticGrid.Add(new List<Node2D>());
				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
					staticGrid[i].Add(null);
			}
		}
		private void GenerateStaticGrid(List<List<int>> pGrid)
		{
			staticGrid.Clear();
			int lRows = pGrid.Count;
			for (int j = 0; j < lRows; j++)
			{
				List<Node2D> lRowNodes = new List<Node2D>();
				int lCols = pGrid[j].Count;
				for (int i = 0; i < lCols; i++)
				{
					Vector2 lPos = new Vector2(i * Utils.MAP_CASE_SCALE, j * Utils.MAP_CASE_SCALE);
					lRowNodes.Add(SpawnObject(pGrid[j][i], lPos));
				}
				staticGrid.Add(lRowNodes);
			}
		}
		private Node2D SpawnObject(int pType, Vector2 pPos)
		{
			PackedScene lScene = pType switch
			{
				0 => wallScene,
				1 => playerScene,
				2 => diceScene,
				3 => finishZoneScene,
				_ => null
			};
			return lScene != null ? Utils.CreateObject(lScene, pPos, this) : null;
		}
		public void PlaceObjectFromList(List<List<Node2D>> pGrid)
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
					if (pGrid[j][i] != null)
					{
						// GD.Print(pGrid[j][i].Name + " " + i + " " + j);
						pGrid[j][i].GlobalPosition = lPosition;
					}
					// else GD.Print($"null on {i} {j}");
				}
				// GD.Print("end line");
			}
		}
		public void GridSnap()
		{
			foreach (Node2D lObject in GetChildren())
			{
				float lPosX = Mathf.Round(lObject.GlobalPosition.X / Utils.MAP_CASE_SCALE) * Utils.MAP_CASE_SCALE;
				float lPosY = Mathf.Round(lObject.GlobalPosition.Y / Utils.MAP_CASE_SCALE) * Utils.MAP_CASE_SCALE;
				lObject.GlobalPosition = new Vector2(lPosX, lPosY);
			}
		}
		#region GetObject
		public Node2D GetObjectOnPosition(Vector2 pPosition)
		{
			Vector2I lPos = Utils.PositionToGridPosition(pPosition);
			return GetObjectOnGrid(lPos);
		}
		public Node2D GetObjectOnGrid(Vector2I pPosition)
		{
			return movableGrid[pPosition.Y][pPosition.X];
		}
		#endregion
		#region Move
		public bool MoveFromPos(Vector2 pPosition, Vector2I pDirection)
		{
			Vector2I lPos = Utils.PositionToGridPosition(pPosition);
			Vector2I lEndPos = lPos + pDirection;
			return MoveOnGrid(lPos.X, lPos.Y, lEndPos.X, lEndPos.Y);
		}
		public bool MoveOnGrid(int pStartX, int pStartY, int pEndX, int pEndY)
		{
			// GD.Print(movableGrid[pEndY][pEndX]);
			movableGrid[pEndY][pEndX] = movableGrid[pStartY][pStartX];
			movableGrid[pStartY][pStartX] = null;
			GD.Print($"X first move from {pStartX} {pStartY} to {pEndX} {pEndY}");
			GD.Print(movableGrid[pEndY][pEndX].Name);
			return true;
		}
		#endregion
		#region Reset
		public void LoadGrid(List<List<int>> pGrid)
		{
			EraseGrid();
			GenerateStaticGrid(pGrid);
			InitMovableGrid();
			GridSnap();
		}
		public void ResetGrid()
		{
			PlaceObjectFromList(staticGrid);
			GridSnap();
			InitMovableGrid();
		}
		public void EraseGrid()
		{
			foreach (Node2D lObject in GetChildren()) lObject.QueueFree();
			staticGrid.Clear();
			movableGrid.Clear();
		}
		#endregion
	}
}
