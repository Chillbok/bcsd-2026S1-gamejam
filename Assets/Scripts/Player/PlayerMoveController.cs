using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("이동 관련 변수")]
    [SerializeField] private float moveSpeed = 5f;
    private PlayerInput _playerInput;
    private Vector2 _moveInput;

    // 컴포넌트 변수들
    private Rigidbody2D _rb;
    private InputAction _moveAction;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    // 애니메이터 컨트롤러 변수들
    string[] _animatorParam = {"isRunning"};

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        
        // PlayerInput 컴포넌트에서 Move 액션을 가져옵니다.
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
        // OnMove() 이벤트
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
    }
    
    private void EventRemove()
    {
        // OnMove() 이벤트
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = new Vector2(context.ReadValue<Vector2>().x, 0).normalized;

        if (_moveInput.x < 0) _spriteRenderer.flipX = true;
        else if (_moveInput.x > 0) _spriteRenderer.flipX = false;

        if (_moveInput.x != 0) _animator.SetBool(_animatorParam[0], true);
        else _animator.SetBool(_animatorParam[0], false);
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 통해 물리적 이동을 처리합니다.
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }
}
