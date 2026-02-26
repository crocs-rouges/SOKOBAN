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
		"### $   #",
		"#@      #",
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


		private string playerVisualizer = "res://Scenes/PlayerVisualizer.tscn";

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
				movableGrid.Add(new List<Node2D>(staticGrid[i]));
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
		public Vector2I LogicToTilemapPos(Vector2I pLogicPos)
		{
			// Rotate grid 90 degrees left
			if (tileGoUp) return new Vector2I(pLogicPos.Y, -pLogicPos.X);
			return pLogicPos;
		}
		public void PlaceObjectFromList(List<List<Node2D>> pGrid)
		{
			int lFirstListCount = pGrid.Count;
			if (lFirstListCount == 0) return;
			for (int i = 0; i < lFirstListCount; i++)
			{
				int lSecondListCount = pGrid[i].Count;
				for (int j = 0; j < lSecondListCount; j++)
				{
					Vector2I lLogicPos = new Vector2I(j, i);
					Vector2I lTilePos = LogicToTilemapPos(lLogicPos);
					if (pGrid[i][j] != null)
					{
						pGrid[i][j].GlobalPosition = ToGlobal(MapToLocal(lTilePos));

						if (pGrid[i][j] is Bloc)
						{
							if (!logicGridVisible) pGrid[i][j].Visible = false;
							SetCell(1, lTilePos, 0, new Vector2I(2, 12));
						}
						if (pGrid[i][j] is Dice) SetCell(1, lTilePos, 0, new Vector2I(3, 13));
						if (pGrid[i][j] is Movable lMovable) lMovable.logicPos = lLogicPos;
					}
					SetCell(0, lTilePos + new Vector2I(1, 1), 0, new Vector2I(0, 12));
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
			if (!IsInsideGrid(pPosition.X, pPosition.Y)) return null;
			return movableGrid[pPosition.Y][pPosition.X];
		}
		private bool IsInsideGrid(int pX, int pY)
		{
			if (pY < 0 || pY >= movableGrid.Count) return false;
			if (pX < 0 || pX >= movableGrid[pY].Count) return false;
			return true;
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
			// Safety checks
			if (!IsInsideGrid(pStartX, pStartY) || !IsInsideGrid(pEndX, pEndY)) return false;
			Node2D lMovedObject = movableGrid[pStartY][pStartX];
			if (lMovedObject == null)
			{
				GD.PrintErr($"MoveOnGrid: No object found at start pos {pStartX},{pStartY}");
				return false;
			}
			movableGrid[pEndY][pEndX] = lMovedObject;
			movableGrid[pStartY][pStartX] = null;
			GD.Print($"Moved {lMovedObject.Name} from {pStartX},{pStartY} to {pEndX},{pEndY}");
			return true;
		}
		#endregion
		#region Reset
		public void LoadGrid(List<string> pGrid)
		{
			EraseGrid();
			GenerateStaticGrid(pGrid);
			InitMovableGrid();
		}
		public void ResetGrid()
		{
			PlaceObjectFromList(staticGrid);
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