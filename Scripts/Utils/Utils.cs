using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
	public partial class Utils : Node
	{
		private static Utils instance;

		public const int MAP_CASE_SCALE = 128;

		public static Utils GetInstance()
		{
			if (instance == null) instance = new Utils();
			return instance;
		}
		public override void _Ready()
		{
			instance = this;
			base._Ready();
		}
		public static Timer CreateOneSecTimer(Node pNode)
		{
			return CreateTimer(pNode, 1f);
		}
		public static Timer CreateTimer(Node pNode, float pTime)
		{
			Timer lTimer = new Timer();
			pNode.AddChild(lTimer);
			lTimer.WaitTime = pTime;
			lTimer.OneShot = true;
			lTimer.Start();
			return lTimer;
		}
	}
}
