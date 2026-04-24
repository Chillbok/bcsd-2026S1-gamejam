using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Anim.SetBool("isRunning", false);
        stateMachine.Rb.linearVelocity = new Vector2(0, stateMachine.Rb.linearVelocity.y);
    }

    public override void HandleInput()
    {
        Vector2 moveInput = stateMachine.PlayerInput.actions["Move"].ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }

        if (stateMachine.PlayerInput.actions["Attack"].triggered)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
    }
}
