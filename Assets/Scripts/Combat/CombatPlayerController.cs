// 플레이어 공격 관리하는 클래스
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class CombatPlayerController : MonoBehaviour
{
    public static CombatPlayerController Instance { get; private set; }
    
    // 이벤트
    public event Action OnAttack;
    public event Action OnDodgeLeft;
    public event Action OnDodgeRight;
    public event Action OnDefend;
    
    // 참조변수
    EncounterManager _encounterManager = EncounterManager.Instance;
    PlayerInput _playerInput;
    
    // 스탯
    [Header("플레이어 체력 정보 (읽기 전용)")]
    [SerializeField] private float _playerMaxHp;
    [SerializeField] private float _playerCurrentHp;
    [Header("적 체력 정보 (읽기 전용)")]
    [SerializeField] private float _enemyMaxHp;
    [SerializeField] private float _enemyCurrentHp;

    public float PlayerMaxHp 
    {
        get => _playerMaxHp; 
        set => _playerMaxHp = value;
    }
    public float PlayerCurrentHp 
    {
        get => _playerCurrentHp; 
        set => _playerCurrentHp = value;
    }
    public float EnemyMaxHp 
    {
        get => _enemyMaxHp; 
        set => _enemyMaxHp = value;
    }
    public float EnemyCurrentHp 
    {
        get => _enemyCurrentHp; 
        set => _enemyCurrentHp = value;
    }
    
    [Header("플레이어 데미지")]
    [SerializeField]
    private float _playerDmg = 2;
    public float PlayerDmg {get => _playerDmg; set => _playerDmg = value;}

    [Header("방어 데미지 감소 비율")]
    [SerializeField]
    [Tooltip("방어 성공 시 받는 데미지 비율 (예: 0.5면 50%의 데미지만 받음)")]
    private float _defenseDamageRatio = 0.5f;
    public float DefenseDamageRatio {get => _defenseDamageRatio; set => _defenseDamageRatio = value;}

    [Header("공격 설정")]
    [SerializeField]
    private float _attackCooldown = 0.7f;
    private float _lastAttackTime = -Mathf.Infinity;

    [Header("플레이어 전투 상태")]
    [SerializeField]
    private bool _isAttacking = false;
    public bool IsAttacking {get => _isAttacking; set => _isAttacking = value;}
    [SerializeField]
    private bool _isDodgingLeft = false;
    public bool IsDodgingLeft {get => _isDodgingLeft; set => _isDodgingLeft = value;}
    [SerializeField]
    private bool _isDodgingRight = false;
    public bool IsDodgingRight {get => _isDodgingRight; set => IsDodgingRight = value;}
    [SerializeField]
    private bool _isDefending = false;
    public bool IsDefending {get => _isDefending; set => _isDefending = value;}

    private InputAction _moveAction;
    private bool _actionTriggered = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions.FindAction("Move");
    }

    private void OnEnable()
    {
        if (_moveAction != null)
        {
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
        }
    }

    private void OnDisable()
    {
        if (_moveAction != null)
        {
            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        if (moveInput == Vector2.zero) 
        {
            _actionTriggered = false;
            if (IsDefending)
            {
                Debug.Log("방어 (Defend) 종료 감지");
                IsDefending = false;
            }
            return;
        }

        if (IsAttacking) return;

        if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            if (moveInput.y > 0)
            {
                if (!_actionTriggered && Time.time >= _lastAttackTime + _attackCooldown)
                {
                    Debug.Log("공격 (Attack) 입력 감지");
                    _actionTriggered = true;
                    _lastAttackTime = Time.time;
                    StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                if (!IsDefending)
                {
                    Debug.Log("방어 (Defend) 시작 감지");
                    IsDefending = true;
                    OnDefend?.Invoke();
                }
            }
        }
        else
        {
            if (moveInput.x < 0)
            {
                if (!_actionTriggered)
                {
                    Debug.Log("왼쪽 회피 (Dodge Left) 입력 감지");
                    OnDodgeLeft?.Invoke();
                    _actionTriggered = true;
                }
            }
            else
            {
                if (!_actionTriggered)
                {
                    Debug.Log("오른쪽 회피 (Dodge Right) 입력 감지");
                    OnDodgeRight?.Invoke();
                    _actionTriggered = true;
                }
            }
        }

        if (moveInput.y >= 0 || Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y))
        {
            if (IsDefending)
            {
                Debug.Log("방어 (Defend) 종료 감지");
                IsDefending = false;
            }
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _actionTriggered = false;

        if (IsDefending)
        {
            Debug.Log("방어 (Defend) 종료 감지");
            IsDefending = false;
        }
    }

    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        OnAttack?.Invoke();
        yield return new WaitForSeconds(_attackCooldown);
        IsAttacking = false;
    }

    private void Start()
    {
        PlayerMaxHp = _encounterManager.PlayerMaxHp;
        PlayerCurrentHp = _encounterManager.PlayerCurrentHp;
        EnemyMaxHp = _encounterManager.EnemyMaxHp;
        EnemyCurrentHp = _encounterManager.EnemyCurrentHp;
    }
    
    private void EndFight()
    {
        _encounterManager.PlayerMaxHp = PlayerMaxHp;
        _encounterManager.PlayerCurrentHp = PlayerCurrentHp;
        _encounterManager.EnemyMaxHp = EnemyMaxHp; // Fix: 적의 최대 체력 변수 수정
        _encounterManager.EnemyCurrentHp = EnemyCurrentHp;

        SceneManager.LoadScene("GamePlayScene");
    }
}
