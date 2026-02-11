using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class Casino_case : Node2D
	{

		private Area2D area;
		[Export] private PackedScene blocscene;
		[Export] private int maxNumberPassage = 3;
		private int numberOfPassage;


		public override void _Ready()
		{
			base._Ready();
			area = GetNode<Area2D>("Area2D");
			if (area != null)
				area.AreaEntered += (Area2D pArea) => AddPassage();
		}
		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			base._Process(pDelta);
		}
		private void AddPassage()
		{

			numberOfPassage++;
			GD.Print(numberOfPassage);
			if (numberOfPassage >= maxNumberPassage)
			{
				Node2D lBloc = blocscene.Instantiate() as Node2D;
				GridManager.GetInstance().AddChild(lBloc);
				lBloc.GlobalPosition = GlobalPosition;
				QueueFree();
			}
		}
	}
}
