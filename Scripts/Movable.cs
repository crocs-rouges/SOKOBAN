using Godot;
using System;

// Author : Romain Chevalier

namespace Com.IsartDigital.SOKOBAN
{
    public partial class Movable : Node2D
    {
        [Export] public RayCast2D rayCast;

        public virtual bool Move(Vector2I pDirection)
        {
            RotationPhysics(pDirection);
            if (rayCast.IsColliding())
            {
                Node2D lCollider = rayCast.GetCollider() as Node2D;
                Node2D lParent = lCollider.GetParent() as Node2D;
                // Check if the obstacle is Movable (like a Dice) and try to push it
                if (lParent is Movable lMovable)
                {
                    if (!lMovable.Move(pDirection)) return false;
                }
                else if (lParent is FinishZone) { }
                else if (lParent is Casino_case) { }
                else
                {
                    // Blocked by a wall or static object
                    return false;
                }
            }
            GridManager.GetInstance().MoveFromPos(GlobalPosition, pDirection);

            GlobalPosition += pDirection * Utils.MAP_CASE_SCALE;

            return true;
        }
        public virtual void RotationPhysics(Vector2I pDirection)
        {
            rayCast.TargetPosition = pDirection * Utils.MAP_CASE_SCALE;
            rayCast.ForceRaycastUpdate();
            // LookAt(pDirection);
        }
        public virtual void RotateRaycastRight(bool pIsTurningRight)
        {
            float lRotation = pIsTurningRight ? 90 : -90;
            rayCast.TargetPosition = rayCast.TargetPosition.Rotated(Mathf.DegToRad(lRotation));
            rayCast.ForceRaycastUpdate();
        }
    }
}
