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
		public static Vector2I PositionToGridPosition(Vector2 pPosition)
		{
			int lPosX = (int)Mathf.Round(pPosition.X / MAP_CASE_SCALE);
			int lPosY = (int)Mathf.Round(pPosition.Y / MAP_CASE_SCALE);
			return new Vector2I(lPosX, lPosY);
		}
		public static Node2D CreateObject(PackedScene pScene, Vector2 pPosition, Node pAddTo)
		{
			Node2D lObject = pScene.Instantiate() as Node2D;
			if (pAddTo != null) pAddTo.AddChild(lObject);
			lObject.GlobalPosition = pPosition;
			return lObject;
		}
	}
}
