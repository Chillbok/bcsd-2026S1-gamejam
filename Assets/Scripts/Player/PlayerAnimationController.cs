using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerFSM))]
public class PlayerAnimationController : MonoBehaviour
{
    // 컴포넌트 변수들
    private PlayerFSM _fsm;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private PlayerInput _playerInput;
    
    // 플레이어 입력 관련 변수들
    private InputAction _moveAction;

    // 애니메이터 컨트롤러 변수들
    private string[] _animatorParam = {"isRunning", "isAttacking"};
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _fsm = GetComponent<PlayerFSM>();

        _moveAction = _playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        EventAdd();
    }

    private void OnDisable()
    {
        EventRemove();
    }

    private void EventAdd()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
    }

    private void EventRemove()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.x == 0)
        {
            _fsm.isMoving = false;
        }
        else
        {
            _fsm.isMoving = true;
            if (input.x < 0) _spriteRenderer.flipX = true;
            else _spriteRenderer.flipX = false;
        }
    }
}
