using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    [Header("Settings")]
    public float MoveSpeed = 5f;

    private PlayerBaseState _currentState;

    // States
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        PlayerInput = GetComponent<PlayerInput>();

        // Initialize States
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
    }

    private void Start()
    {
        ChangeState(IdleState);
    }

    private void Update()
    {
        _currentState?.HandleInput();
        _currentState?.Update();
    }

    private void FixedUpdate()
    {
        _currentState?.PhysicsUpdate();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    // Animation Event Callbacks
    public void OnAttackEnd()
    {
        if (_currentState == AttackState)
        {
            AttackState.OnAttackFinished();
        }
    }
}
