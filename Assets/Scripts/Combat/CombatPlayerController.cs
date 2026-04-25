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

    [Header("방어/회피 설정")]
    [SerializeField]
    private float _parryWindow = 0.1f; // 타격 시점 기준 ±0.1초 (총 0.2초 윈도우)
    [SerializeField]
    private float _dodgeDuration = 0.5f; // 회피 무적 시간 (초)

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
    public bool IsDodgingRight {get => _isDodgingRight; set => _isDodgingRight = value;}
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
                if (!_actionTriggered && !IsDodgingLeft && !IsDodgingRight)
                {
                    Debug.Log("왼쪽 회피 (Dodge Left) 입력 감지");
                    OnDodgeLeft?.Invoke();
                    _actionTriggered = true;
                    StartCoroutine(DodgeLeftRoutine());
                }
            }
            else
            {
                if (!_actionTriggered && !IsDodgingLeft && !IsDodgingRight)
                {
                    Debug.Log("오른쪽 회피 (Dodge Right) 입력 감지");
                    OnDodgeRight?.Invoke();
                    _actionTriggered = true;
                    StartCoroutine(DodgeRightRoutine());
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

    private IEnumerator DodgeLeftRoutine()
    {
        IsDodgingLeft = true;
        Debug.Log($"[회피] 왼쪽 회피 무적 시간 시작 ({_dodgeDuration}초)");
        yield return new WaitForSeconds(_dodgeDuration);
        IsDodgingLeft = false;
        Debug.Log("[회피] 왼쪽 회피 무적 시간 종료");
    }

    private IEnumerator DodgeRightRoutine()
    {
        IsDodgingRight = true;
        Debug.Log($"[회피] 오른쪽 회피 무적 시간 시작 ({_dodgeDuration}초)");
        yield return new WaitForSeconds(_dodgeDuration);
        IsDodgingRight = false;
        Debug.Log("[회피] 오른쪽 회피 무적 시간 종료");
    }

    // 적의 공격 판정을 처리하는 메서드
    public void TakeDamage(int attackNumber, float incomingDamage, float attackTime)
    {
        Debug.Log($"--- [피격 판정 시작] 공격 번호: {attackNumber}, 들어오는 데미지: {incomingDamage} ---");
        bool isHeavyAttack = attackNumber >= 4; // 4, 5, 6은 강공격

        // 1. 회피 판정
        bool dodged = false;
        if (attackNumber == 1 || attackNumber == 4) // 왼쪽 공격
        {
            if (IsDodgingRight) dodged = true;
        }
        else if (attackNumber == 3 || attackNumber == 6) // 오른쪽 공격
        {
            if (IsDodgingLeft) dodged = true;
        }
        else if (attackNumber == 2 || attackNumber == 5) // 중앙 공격
        {
            if (IsDodgingLeft || IsDodgingRight) dodged = true;
        }

        if (dodged)
        {
            Debug.Log("<color=cyan>[회피 성공] 올바른 방향으로 회피하여 데미지를 입지 않습니다!</color>");
            return; // 데미지 0
        }
        else
        {
            Debug.Log($"[회피 실패] 회피하지 않았거나 방향이 틀렸습니다. (현재 상태 - 좌회피:{IsDodgingLeft}, 우회피:{IsDodgingRight})");
        }

        // 2. 패링 판정 (강/약공격 모두 패링 가능)
        if (IsDefending)
        {
            float timeDiff = Mathf.Abs(Time.time - attackTime);
            Debug.Log($"[패링 판정] 방어 중 - 타격 시점과의 오차 시간: {timeDiff:F3}초 (패링 허용치: {_parryWindow}초)");

            if (timeDiff <= _parryWindow)
            {
                Debug.Log("<color=green>[패링 성공] 완벽한 타이밍에 방어했습니다! 데미지 0</color>");
                // TODO: 패링 이펙트, 적 스턴 등 추가 로직 구현
                return; // 데미지 0
            }
            else
            {
                Debug.Log("[패링 실패] 타이밍이 어긋나 일반 방어로 넘어갑니다.");
            }
        }

        // 3. 방어 판정 및 강공격 패널티
        float finalDamage = incomingDamage;

        if (IsDefending)
        {
            if (isHeavyAttack)
            {
                Debug.LogWarning("<color=orange>[방어 실패] 강공격을 방어하려고 했으나 뚫렸습니다! 100% 데미지가 들어옵니다.</color>");
                // 강공격은 일반 방어 불가, 데미지 경감 없음
            }
            else
            {
                Debug.Log($"<color=yellow>[방어 성공] 일반 공격을 방어했습니다! 데미지 감소 비율 적용 (x{_defenseDamageRatio})</color>");
                finalDamage *= _defenseDamageRatio;
            }
        }
        else
        {
            Debug.Log("[무방비] 방어 및 회피를 하지 않아 타격을 그대로 허용했습니다.");
        }

        // 4. 최종 데미지 적용
        PlayerCurrentHp -= finalDamage;
        Debug.Log($"<color=red>[피격] 플레이어가 {finalDamage}의 데미지를 입었습니다. 남은 체력: {PlayerCurrentHp}</color>");

        // 플레이어 체력 고갈 시
        if (PlayerCurrentHp <= 0)
        {
            PlayerCurrentHp = 0;
            Debug.Log("플레이어 사망!");
            // TODO: 게임 오버 로직 등
        }
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
