using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class DataManager : Node
	{
		private static DataManager instance;
		private const string SAVE_PATH = "save.json";





		public static DataManager GetInstance()
		{
			if (instance == null) instance = new DataManager();
			return instance;
		}
		public override void _Ready()
		{
			instance = this;
			base._Ready();
		}
		public void Save()
		{
		}
		public void Load()
		{
		}
	}
}
