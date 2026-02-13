using Godot;
using System.Collections.Generic;
using System.Linq;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class Dice : Movable
	{
		[Export] private Sprite2D sprite;
		[Export] private Sprite2D rightFace;
		[Export] private Sprite2D backFace;
		[Export] private Sprite2D frontFace;
		[Export] private Sprite2D leftFace;
		public List<int> numberinface = new List<int>() { 1, 2, 3, 4, 5, 6 };
		[Export] public Texture2D[] faceTextures; //for later
		[Export] public int indexFaceUp = 0;
		[Export] public int indexFaceFront = 1;
        [Export] public int indexFaceRight = 2;

        public static Texture2D[] FaceTexturesSt {  get; private set; }

        public override void _Ready() 
		{
			base._Ready();
            FaceTexturesSt = faceTextures;
            UpdateVisuals();
        }

		public override bool Move(Vector2I pDirection)
		{
			if (base.Move(pDirection))
			{
				Roll(pDirection);
				return true;
			}
			return false;
		}
		public bool WhipPull(Vector2I pDirection)
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

            frontFace.Texture = faceTextures[indexFaceFront];
            rightFace.Texture = faceTextures[indexFaceRight];
            leftFace.Texture = faceTextures[5 - indexFaceRight];
            backFace.Texture = faceTextures[5 - indexFaceFront];

            sprite.Texture = faceTextures[indexFaceUp];
		}
	}
}