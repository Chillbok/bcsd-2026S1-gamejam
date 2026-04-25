// 전투에서 적을 관리하는 클래스
using System;
using System.Collections;
using UnityEngine;

public class CombatEnemyController : MonoBehaviour
{
    // 이벤트: 공격 방향(int)과 경고 시간(float)을 UI 매니저 등에 전달
    public event Action<int, float> OnAttackWarning;

    // 스탯
    int _attackNumber = 1;
    
    [Header("적 데미지")]
    [SerializeField]
    private float _enemyDmg = 10;
    public float EnemyDmg {get => _enemyDmg; set => _enemyDmg = value;}

    [Header("적 강공격 데미지 배수")]
    [SerializeField]
    private float _multiplier = 1.5f;
    public float Multiplier {get => _multiplier; set => _multiplier = value;}
    
    [Header("공격 패턴 설정")]
    [SerializeField]
    private float _attackInterval = 3.0f; // 공격과 다음 공격 사이 대기 시간
    [SerializeField]
    private float _warningDuration = 0.5f; // 공격 방향 예고 후 실제 타격까지의 선딜레이 시간

    private float _attackTime; // 적이 공격을 실행한 기준 시간
    private bool _canAttack = true; // 자동 공격 코루틴 제어 변수

    private void Start()
    {
        StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        // 전투 시작 전 잠시 대기
        yield return new WaitForSeconds(1.0f);

        while (_canAttack)
        {
            // 1. 공격 판정: 어떤 공격을 할지 정함
            DefineAttack();

            // 2. 공격 예고: 플레이어에게 방향 및 종류 알림
            ShowAttackWarning(_attackNumber);

            // UI 매니저 등 이벤트 구독자에게 경고 신호 발송
            OnAttackWarning?.Invoke(_attackNumber, _warningDuration);

            // 3. 짧은 시간 대기: 플레이어가 반응(회피/방어)할 시간
            yield return new WaitForSeconds(_warningDuration);

            // 4. 타격 시점 갱신 (패링의 기준 시간이 됨)
            _attackTime = Time.time;

            // 5. 플레이어에게 공격 시도
            DealDamageToPlayer();

            // 6. 다음 공격까지 대기
            yield return new WaitForSeconds(_attackInterval);
        }
    }

    // 1부터 6까지, 순서대로 좌/중앙/우 약공격, 좌/중앙/우 강공격
    private void DefineAttack() 
    {
        _attackNumber = GetRandomDiceNumber();
    }

    // 공격 방향과 종류를 예고하는 헬퍼 메서드
    private void ShowAttackWarning(int attackNum)
    {
        string direction = "";
        string type = (attackNum >= 4) ? "강공격" : "약공격";

        if (attackNum == 1 || attackNum == 4) direction = "왼쪽";
        else if (attackNum == 2 || attackNum == 5) direction = "중앙";
        else if (attackNum == 3 || attackNum == 6) direction = "오른쪽";

        Debug.Log($"<color=red>적 공격 예고: {direction} {type} !!</color>");
        // TODO: 향후 느낌표 이펙트, 예고 애니메이션 등을 이곳에 추가
    }

    // 1부터 6까지의 랜덤 숫자를 산출하는 메서드
    private int GetRandomDiceNumber()
    {
        return UnityEngine.Random.Range(1, 7);
    }

    // 플레이어에게 데미지를 입히는 메서드 (애니메이션 이벤트 등에서 호출)
    public void DealDamageToPlayer()
    {
        CombatPlayerController player = CombatPlayerController.Instance;
        if (player == null) return;

        // 기본 데미지 설정
        float damage = _enemyDmg;

        // 강공격(4, 5, 6)일 경우 데미지 배수 적용
        if (_attackNumber >= 4)
        {
            damage *= _multiplier;
        }

        // 플레이어에게 공격 정보 전달하여 피격/방어/회피 판정 위임
        player.TakeDamage(_attackNumber, damage, _attackTime);
    }
}
