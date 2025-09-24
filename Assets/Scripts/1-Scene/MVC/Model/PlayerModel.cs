using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public enum PlayerState
    {
        Idle, // 站立
        Moving, // 移动
        Interacting, // 交互中
        Disabled, // 被禁用/过场/对话等
    }

    public enum Direction
    {
        Down,
        Left,
        Right,
        Up,
    }

    public class PlayerModel
    {
        public PlayerState State { get; private set; } = PlayerState.Idle;
        public Vector2 MoveInput { get; private set; } = Vector2.zero;
        public bool IsSprinting { get; private set; } = false;
        public bool CanMove => State == PlayerState.Idle || State == PlayerState.Moving;
        public bool IsLocked => State == PlayerState.Interacting || State == PlayerState.Disabled;
        public Direction Direction { get; private set; } = Direction.Down;
        public Vector2 FacingDir
        {
            get
            {
                switch (Direction)
                {
                    case Direction.Left:
                        return Vector2.left;
                    case Direction.Right:
                        return Vector2.right;
                    case Direction.Up:
                        return Vector2.up;
                    default:
                        return Vector2.down;
                }
            }
        }

        public void SetMoveInput(Vector2 input)
        {
            // 若被锁定（交互/禁用），不接受移动输入
            if (IsLocked)
            {
                // 清空移动输入
                MoveInput = Vector2.zero;
                return;
            }
            MoveInput = input.sqrMagnitude > 1e-6f ? input.normalized : Vector2.zero;
            // 若无输入，切换到idle
            if (MoveInput == Vector2.zero)
            {
                SetState(PlayerState.Idle);
            }
            else
            {
                // 更新朝向
                if (MoveInput != Vector2.zero)
                {
                    UpdateFacingFromVector(MoveInput);
                }
                SetState(PlayerState.Moving);
            }
        }

        private void UpdateFacingFromVector(Vector2 v)
        {
            // 4 向量化：根据 x/y 绝对值决定主方向
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            {
                Direction = v.x >= 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                Direction = v.y >= 0 ? Direction.Up : Direction.Down;
            }
        }

        public void SetSprinting(bool v)
        {
            IsSprinting = v && CanMove;
        }

        public bool TryBeginInteract()
        {
            if (IsLocked)
            {
                return false;
            }
            MoveInput = Vector2.zero;
            SetState(PlayerState.Interacting);
            return true;
        }

        public void EndInteract()
        {
            if (State == PlayerState.Interacting)
            {
                SetState(PlayerState.Idle);
            }
        }

        public void SetDisabled(bool disabled)
        {
            if (disabled)
            {
                MoveInput = Vector2.zero;
                SetState(PlayerState.Disabled);
            }
            else
            {
                SetState(PlayerState.Idle);
            }
        }

        private void SetState(PlayerState s)
        {
            if (s == State)
            {
                return;
            }
            State = s;
        }
    }
}
