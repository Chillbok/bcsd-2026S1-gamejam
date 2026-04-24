// 전투에서 적을 관리하는 클래스
using UnityEngine;

public class CombatEnemyController : MonoBehaviour
{
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
    
    [Header("패링 설정")]
    [SerializeField]
    private float _parryWindow = 0.2f; // 적 공격 전/후로 패링을 허용할 시간 (초)
    private float _attackTime; // 적이 공격을 실행한(또는 실행할) 기준 시간

    // 1부터 6까지, 순서대로 좌/중앙/우 약공격, 좌/중앙/우 강공격
    private void DefineAttack() 
    {
        _attackNumber = GetRandomDiceNumber();
        _attackTime = Time.time; // 공격을 정의/시작하는 시점 기록 (필요에 따라 애니메이션 이벤트 시점으로 변경 가능)
    }

    // 1부터 6까지의 랜덤 숫자를 산출하는 메서드
    private int GetRandomDiceNumber()
    {
        return Random.Range(1, 7);
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

        // 회피 판정 (현재 회피 중이면 데미지를 입지 않음 - 임시 로직)
        if (player.IsDodgingLeft || player.IsDodgingRight)
        {
            Debug.Log("플레이어가 공격을 회피했습니다!");
            return;
        }

        // 패링 및 방어 판정
        // 적의 공격 판정 타이밍(_attackTime)과 현재 시간의 차이가 패링 윈도우 이내이고 플레이어가 방어 중일 때 패링 성공
        if (player.IsDefending && Mathf.Abs(Time.time - _attackTime) <= _parryWindow)
        {
            Debug.Log("플레이어가 패링에 성공했습니다! 데미지 0");
            damage = 0f; // 패링 시 데미지 무효화
            // TODO: 패링 이펙트, 적 스턴 등 추가 로직 구현
            return;
        }
        else if (player.IsDefending)
        {
            Debug.Log($"플레이어가 공격을 방어했습니다! 데미지 감소 비율: {player.DefenseDamageRatio}");
            damage *= player.DefenseDamageRatio; // 방어 시 플레이어에 설정된 비율만큼 데미지 감소
        }

        // 최종 데미지 적용
        player.PlayerCurrentHp -= damage;
        Debug.Log($"플레이어가 {damage}의 데미지를 입었습니다. 남은 체력: {player.PlayerCurrentHp}");

        // 플레이어 체력 고갈 시
        if (player.PlayerCurrentHp <= 0)
        {
            player.PlayerCurrentHp = 0;
            Debug.Log("플레이어 사망!");
            // TODO: 게임 오버 로직 등
        }
    }
}
