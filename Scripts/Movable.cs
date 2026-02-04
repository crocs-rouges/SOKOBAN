using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
    public partial class Movable : Node2D
    {
        [Export] public RayCast2D rayCast;

        public override void _Ready()
        {
            base._Ready();
        }
        public virtual void Up()
        {
            rayCast.TargetPosition = Vector2.Up * Utils.MAP_CASE_SCALE;
            rayCast.ForceRaycastUpdate();
            if (rayCast.IsColliding())
            {
                GD.Print("hello");
                return;
            }
            GlobalPosition += Vector2I.Up * Utils.MAP_CASE_SCALE;
        }
        public virtual void Down()
        {
            rayCast.TargetPosition = Vector2.Down * Utils.MAP_CASE_SCALE;
            rayCast.ForceRaycastUpdate();
            if (rayCast.IsColliding())
            {
                GD.Print("hello");
                return;
            }
            GlobalPosition += Vector2I.Down * Utils.MAP_CASE_SCALE;
        }
        public virtual void Left()
        {
            rayCast.TargetPosition = Vector2.Left * Utils.MAP_CASE_SCALE;
            rayCast.ForceRaycastUpdate();
            if (rayCast.IsColliding())
            {
                GD.Print("hello");
                return;
            }
            GlobalPosition += Vector2I.Left * Utils.MAP_CASE_SCALE;
        }
        public virtual void Right()
        {
            rayCast.TargetPosition = Vector2.Right * Utils.MAP_CASE_SCALE;
            rayCast.ForceRaycastUpdate();
            if (rayCast.IsColliding())
            {
                Node2D lCollider = rayCast.GetCollider() as Node2D;
                if (lCollider.GetParent() is Dice)
                {
                    GD.Print("hello i'm dice");
                    Dice lDice = (Dice)lCollider.GetParent();
                    lDice.Right();
                }
                // if(lCollider.GetParent() is Wall)
                // {
                //     GD.Print("hello i'm wall");
                //     return;
                // }
                GD.Print("hello");

                return;
            }
            GlobalPosition += Vector2I.Right * Utils.MAP_CASE_SCALE;
        }
    }
}
