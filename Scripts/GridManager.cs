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
			GridSnap();
			InitMovableGrid();
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
		public void ConvertListIntToNodeList(List<List<int>> pGrid)
		{
			//convert the list of int into a list of node2D
			int lFirstListCount = pGrid.Count;
			if (lFirstListCount == 0) return;
			Vector2 lPosition;
			InitStaticGrid(pGrid);
			for (int i = 0; i < lFirstListCount; i++)
			{
				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					// GD.Print(pGrid[j][i]);
					lPosition = new Vector2(i * Utils.MAP_CASE_SCALE, j * Utils.MAP_CASE_SCALE);
					switch (pGrid[j][i])
					{
						case 0:
							staticGrid[j][i] = Utils.CreateObject(wallscene, lPosition, this);
							break;
						case 1:
							staticGrid[j][i] = Utils.CreateObject(playerscene, lPosition, this);
							break;
						case 2:
							staticGrid[j][i] = Utils.CreateObject(dicescene, lPosition, this);
							break;
						case 3:
							staticGrid[j][i] = Utils.CreateObject(finishzonescene, lPosition, this);
							break;
					}
				}
				// GD.Print("end line");
			}
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
		public void ResetGrid()
		{
			PlaceFromGrid(staticGrid);
			GridSnap();
			InitMovableGrid();
		}
		public void HardResetGrid()
		{
			EraseGrid();
			ConvertListIntToNodeList(testLoadingGrid);
			InitMovableGrid();
		}
		public void EraseGrid()
		{
			foreach (Node2D lObject in GetChildren())
			{
				lObject.QueueFree();
			}
		}
		#endregion
	}
}
