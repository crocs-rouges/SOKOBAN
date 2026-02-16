using Godot;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class FinishZone : Node2D
	{
		private Area2D area;
		[Export] public int finishIndex;
		[Export] public Sprite2D sprite;

		public override void _Ready()
		{
			base._Ready();
			Utils.CreateOneSecTimer(this).Timeout
			+= () => sprite.Texture = Dice.FaceTexturesSt[finishIndex];
			
			area = GetNode<Area2D>("Area2D");
			if (area != null)
				area.AreaEntered += (Area2D pArea) => CheckDiceFace(pArea);
		}
		private void CheckDiceFace(Area2D pArea)
		{
			if (pArea.GetParent() is not Dice) return;
			Dice lDice = pArea.GetParent() as Dice;
			// if (lDice.GlobalPosition != GlobalPosition) return;
			if (lDice.indexFaceUp != finishIndex)
			{
				GD.Print("Wrong face");
				return;
			}
			GD.Print("Finish the game GG");
		}
	}
}
