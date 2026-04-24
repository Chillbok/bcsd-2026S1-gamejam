using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerFSM : MonoBehaviour
{
    // 상태 변수들
    public bool isMoving {get; set;}
    public bool isAttacking {get; set;}

    // 컴포넌트 변수들
    Animator _animator;

    // 애니메이션 관련 변수들
    string[] _animParam = {"isRunning", "isAttacking"};

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	private void Update()
    {

        WhileMoving();
    }

    private void WhileMoving()
    {
        if (isMoving) _animator.SetBool(_animParam[0], true);
        else _animator.SetBool(_animParam[0], false);
    }

    public void OnAttackStart() => isAttacking = true;
    public void OnAttackEnd() => isAttacking = false;
}
