using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
    public partial class Movable : Node2D
    {
        [Export] public RayCast2D rayCast;

        public virtual bool Move(Vector2 pDirection)
        {
            rayCast.TargetPosition = pDirection * Utils.MAP_CASE_SCALE;
            rayCast.ForceRaycastUpdate();

            if (rayCast.IsColliding())
            {
                Node2D lCollider = rayCast.GetCollider() as Node2D;
                Node2D lParent = lCollider.GetParent() as Node2D;
                // Check if the obstacle is Movable (like a Dice) and try to push it
                if (lParent is Movable lMovable)
                {
                    if (!lMovable.Move(pDirection)) return false;
                }
                else
                {
                    // Blocked by a wall or static object
                    return false;
                }
            }
            GlobalPosition += (Vector2I)(pDirection * Utils.MAP_CASE_SCALE);
            return true;
        }
    }
}
