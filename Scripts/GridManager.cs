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
			GridSnap();
			// PlaceObjectFromList calls SetCell/Position already, no need to duplicate logic here immediately
		}
		#region Grid Generation
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

					// Center the object in the tile
					lPosition = (new Vector2I(j, i) * Utils.MAP_CASE_SCALE) + (Vector2I.One * Utils.MAP_CASE_SCALE / 2);

					if (pGrid[i][j] != null)
					{
						pGrid[i][j].GlobalPosition = lPosition;
						if (!logicGridVisible) pGrid[i][j].Visible = false;
						SetCell(1, lPosIJ, 0, new Vector2I(1, 0));
						GD.Print(pGrid[i][j]?.Name + " " + lPosIJ);
					}
					SetCell(0, lPosIJ + new Vector2I(1, 1), 0, new Vector2I(0, 0));
				}
			}
		}
		public void GridSnap()
		{
			// CORRECTED: Snap to CENTER of tile to match PlaceObjectFromList
			foreach (Node2D lObject in GetChildren())
			{
				float lPosX = Mathf.Floor(lObject.GlobalPosition.X / Utils.MAP_CASE_SCALE) * Utils.MAP_CASE_SCALE;
				float lPosY = Mathf.Floor(lObject.GlobalPosition.Y / Utils.MAP_CASE_SCALE) * Utils.MAP_CASE_SCALE;

				// Add half scale to center
				lObject.GlobalPosition = new Vector2(lPosX, lPosY) + (Vector2.One * Utils.MAP_CASE_SCALE / 2);
			}
		}
		#endregion
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
			if (!IsInsideGrid(pStartX, pStartY) || !IsInsideGrid(pEndX, pEndY))
			{
				GD.PrintErr($"MoveOnGrid Out of Bounds: {pStartX},{pStartY} to {pEndX},{pEndY}");
				return false;
			}

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
			PlaceObjectFromList(staticGrid);
			InitMovableGrid();
			// GridSnap(); // Removed: PlaceObjectFromList already positions correctly. Snapping might cause drift if math differs.
		}
		public void ResetGrid()
		{
			PlaceObjectFromList(staticGrid);
			// GridSnap(); 
			InitMovableGrid();
		}
		public void EraseGrid()
		{
			// Warning: This kills ALL children. Ensure Utils.SpawnObject adds them as children of GridManager or GetParent() correctly.
			// Based on your spawn logic: Utils.SpawnObject(..., GetParent()) -> They are siblings, not children?
			// If siblings, GetChildren() here returns nothing or wrong things.
			// Assuming you manage logic lists mostly.

			// Simple clear for lists
			staticGrid.Clear();
			movableGrid.Clear();
			// Note: Visual cleanup depends on how Utils spawns them. 
			// If they are children of GridManager, QueueFree works.
			foreach (Node lChild in GetParent().GetChildren())
			{
				if (lChild is Movable || lChild is Dice || lChild is FinishZone || lChild.Name.ToString().Contains("Bloc"))
				{
					lChild.QueueFree();
				}
			}
		}
		#endregion
	}
}