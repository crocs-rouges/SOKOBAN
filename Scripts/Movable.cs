// Author : Romain Chevalier
using Godot;
using System;
namespace Com.IsartDigital.SOKOBAN
{
    public partial class Movable : Node2D
    {
        [Export] public RayCast2D rayCast;
        public Vector2I logicPos;
        private Tween movementTween;
        public virtual bool Move(Vector2I pDirection)
        {
            GridManager lGrid = GridManager.GetInstance();
            Vector2I lEndPos = logicPos + pDirection;
            Node2D lObject = lGrid.GetObjectOnGrid(lEndPos);
            if (lObject is Movable lMovable)
            {
                if (!lMovable.Move(pDirection)) return false;
            }
            else if (lObject is FinishZone) { }
            else if (lObject is Casino_case) { }
            else if (lObject is null) { }
            else return false;
            if (lGrid.MoveOnGrid(logicPos.X, logicPos.Y, lEndPos.X, lEndPos.Y))
            {
                logicPos = lEndPos;
                Vector2I lTilePos = lGrid.LogicToTilemapPos(logicPos);
                Vector2 lTargetPos = lGrid.ToGlobal(lGrid.MapToLocal(lTilePos));
                if (movementTween != null && movementTween.IsRunning()) movementTween.Kill();
                movementTween = GetTree().CreateTween();
                movementTween.TweenProperty(this, "global_position", lTargetPos, 0.2f);
                return true;
            }
            return false;
        }
        private bool MoveByRaycast(Vector2I pDirection)
        {
            RotationPhysics(pDirection);
            if (rayCast.IsColliding())
            {
                Node2D lCollider = rayCast.GetCollider() as Node2D;
                Node2D lParent = lCollider.GetParent() as Node2D;
                if (lParent is Movable lMovable)
                {
                    if (!lMovable.Move(pDirection)) return false;
                }
                else if (lParent is FinishZone) { }
                else if (lParent is Casino_case) { }
                else return false;
            }
            return true;
        }
        public virtual void RotationPhysics(Vector2I pDirection)
        {
            GridManager lGrid = GridManager.GetInstance();
            Vector2 lCurrentLocal = lGrid.MapToLocal(logicPos);
            Vector2 lTargetLocal = lGrid.MapToLocal(logicPos + pDirection);
            rayCast.TargetPosition = lTargetLocal - lCurrentLocal;
            rayCast.ForceRaycastUpdate();
        }
        public virtual void RotateRaycastRight(bool pIsTurningRight)
        {
            float lRotation = pIsTurningRight ? 90 : -90;
            rayCast.TargetPosition = rayCast.TargetPosition.Rotated(Mathf.DegToRad(lRotation));
            rayCast.ForceRaycastUpdate();
        }
    }
}