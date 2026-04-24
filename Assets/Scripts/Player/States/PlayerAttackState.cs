using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Anim.SetBool("isAttacking", true);
        stateMachine.Rb.linearVelocity = Vector2.zero; // 공격 중 이동 멈춤
    }

    public override void Exit()
    {
        stateMachine.Anim.SetBool("isAttacking", false);
    }

    // 애니메이션 이벤트나 외부 호출을 통해 상태를 변경할 수 있도록 함
    public void OnAttackFinished()
    {
        // 상태 전이 전에 애니메이터 파라미터를 먼저 해제하여 AnyState 무한 루프를 방지합니다.
        stateMachine.Anim.SetBool("isAttacking", false);

        Vector2 moveInput = stateMachine.PlayerInput.actions["Move"].ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
