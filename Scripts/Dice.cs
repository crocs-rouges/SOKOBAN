using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class Dice : Movable
	{
		[Export] private Sprite2D sprite;
		public List<int> numberinface = new List<int>() { 1, 2, 3, 4, 5, 6 };
		[Export] public Texture2D[] faceTextures; //for later
		public int indexFaceUp = 0;
		public int indexFaceFront = 1;
		public int indexFaceRight = 2;

		public override bool Move(Vector2 pDirection)
		{
			if (base.Move(pDirection))
			{
				Roll(pDirection);
				return true;
			}
			return false;
		}
		public bool WhipPull(Vector2 pDirection)
		{
			return base.Move(pDirection);
		}
		private void Roll(Vector2 pDirection)
		{
			if (pDirection == Vector2.Up) RotateFaces(ref indexFaceUp, ref indexFaceFront);
			else if (pDirection == Vector2.Down) RotateFaces(ref indexFaceFront, ref indexFaceUp);
			else if (pDirection == Vector2.Left) RotateFaces(ref indexFaceUp, ref indexFaceRight);
			else if (pDirection == Vector2.Right) RotateFaces(ref indexFaceRight, ref indexFaceUp);

			UpdateVisuals();

			GD.Print($"la face du dessus {numberinface[indexFaceUp]}");
			GD.Print($"la face avant {numberinface[indexFaceFront]}");
			GD.Print($"la face droite {numberinface[indexFaceRight]}");
		}
		private void RotateFaces(ref int pFaceA, ref int pFaceB)
		{
			int lOldA = pFaceA;
			pFaceA = pFaceB;
			pFaceB = numberinface.Count - 1 - lOldA;
		}
		private void UpdateVisuals()
		{
			if (faceTextures == null || faceTextures.Count() <= indexFaceUp) return;
			sprite.Texture = faceTextures[indexFaceUp];
		}
	}
}
