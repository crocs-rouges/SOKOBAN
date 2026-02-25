using Godot;
using System;
using System.Collections.Generic;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class GridManager : TileMap
	{
		//for test
		// private List<string> testLoadingGrid = new List<string>(){
		// "#########",
		// "#.      #",
		// "#  @$   #",
		// "#       #",
		// "#       #",
		// "#########"
		// };

		private List<string> testLoadingGrid = new List<string>(){
		"#########",
		"#      ##",
		"#       #",
		"###     #",
		"#  @    #",
		"#########"
		};
		//


		private static GridManager instance;
		[Export] private bool logicGridVisible;
		[Export] private bool tileGoUp;
		public static List<List<Node2D>> staticGrid = new List<List<Node2D>>();
		public List<List<Node2D>> movableGrid = new List<List<Node2D>>();

		private string wallScenePath = "res://Scenes/Bloc.tscn";
		private string diceScenePath = "res://Scenes/Dice.tscn";
		private string playerScenePath = "res://Scenes/Player.tscn";
		private string finishZoneScenePath = "res://Scenes/Finish.tscn";
		private string casinoScenePath = "res://Scenes/casino_case.tscn";

		public static GridManager GetInstance()
		{
			if (instance == null) instance = new GridManager();
			return instance;
		}
		public override void _Ready()
		{
			instance = this;
			base._Ready();
			LoadGrid(testLoadingGrid);
			PlaceObjectFromList(staticGrid);
		}
		private void InitMovableGrid()
		{
			movableGrid = new List<List<Node2D>>();
			int lCount = staticGrid.Count;
			for (int i = 0; i < lCount; i++)
			{
				movableGrid.Add(new List<Node2D>(staticGrid[i]));
			}
		}
		private void GenerateStaticGrid(List<string> pGrid)
		{
			staticGrid.Clear();
			int lRows = pGrid.Count;
			for (int j = 0; j < lRows; j++)
			{
				List<Node2D> lRowNodes = new List<Node2D>();
				int lCols = pGrid[j].Length;
				for (int i = 0; i < lCols; i++)
				{
					Vector2 lPos = new Vector2(i * Utils.MAP_CASE_SCALE, j * Utils.MAP_CASE_SCALE);
					lRowNodes.Add(SpawnObject(pGrid[j][i].ToString(), lPos));
				}
				staticGrid.Add(lRowNodes);
			}
		}
		private Node2D SpawnObject(string pType, Vector2 pPos)
		{
			string lScenePath = pType switch
			{
				"c" => casinoScenePath,
				"#" => wallScenePath,
				"@" => playerScenePath,
				"$" => diceScenePath,
				"." => finishZoneScenePath,
				_ => null
			};
			if (lScenePath == null) return null;
			return Utils.SpawnObject(lScenePath, pPos, GetParent());
		}
		public void PlaceObjectFromList(List<List<Node2D>> pGrid)
		{
			int lFirstListCount = pGrid.Count;
			if (lFirstListCount == 0) return;
			Vector2I lPosIJ;
			Vector2 lPosition;
			for (int i = 0; i < lFirstListCount; i++)
			{
				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					if (tileGoUp) lPosIJ = new Vector2I(i, j) * -1;
					else lPosIJ = new Vector2I(j, i);
					lPosition = (new Vector2I(j, i) * Utils.MAP_CASE_SCALE) + (Vector2I.One * Utils.MAP_CASE_SCALE / 2);
					if (pGrid[i][j] != null)
					{
						pGrid[i][j].GlobalPosition = lPosition;
						if (!logicGridVisible) pGrid[i][j].Visible = false;
						SetCell(1, lPosIJ, 0, new Vector2I(1, 0));
					}
					SetCell(0, lPosIJ + new Vector2I(1, 1), 0, new Vector2I(0, 0));
				}
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
			if (pPosition.Y < 0 || pPosition.Y >= movableGrid.Count) return null;
			if (pPosition.X < 0 || pPosition.X >= movableGrid[pPosition.Y].Count) return null;
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
			Node2D lMovedObject = movableGrid[pStartY][pStartX];
			movableGrid[pEndY][pEndX] = lMovedObject;
			movableGrid[pStartY][pStartX] = null;
			GD.Print($"Moved {lMovedObject?.Name} from {pStartX},{pStartY} to {pEndX},{pEndY}");
			return true;
		}
		#endregion
		#region Reset
		public void LoadGrid(List<string> pGrid)
		{
			EraseGrid();
			GenerateStaticGrid(pGrid);
			PlaceObjectFromList(staticGrid);
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