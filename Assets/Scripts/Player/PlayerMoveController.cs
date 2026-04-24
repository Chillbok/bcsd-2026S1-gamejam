using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private Vector2 _jumpInput;
    private bool _isGrounded;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        
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
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 통해 물리적 이동을 처리합니다.
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }
}
