using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private Vector2 _moveInput;

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Anim.SetBool("isRunning", true);
    }

    public override void HandleInput()
    {
        _moveInput = stateMachine.PlayerInput.actions["Move"].ReadValue<Vector2>();
        
        if (Mathf.Abs(_moveInput.x) < 0.1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

        if (stateMachine.PlayerInput.actions["Attack"].triggered)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }

        // Sprite Flip Logic
        if (_moveInput.x < 0) stateMachine.SpriteRenderer.flipX = true;
        else if (_moveInput.x > 0) stateMachine.SpriteRenderer.flipX = false;
    }

    public override void PhysicsUpdate()
    {
        stateMachine.Rb.linearVelocity = new Vector2(_moveInput.x * stateMachine.MoveSpeed, stateMachine.Rb.linearVelocity.y);
    }
}
