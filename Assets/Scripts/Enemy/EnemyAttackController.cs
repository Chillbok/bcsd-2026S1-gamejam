using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    int _attackNumber = 1;
    
    private void Attack()
    {
        
    }
    
    // 1부터 6까지, 순서대로 좌/중앙/우 약공격, 좌/중앙/우 강공격
    private void DefineAttack() => _attackNumber = GetRandomDiceNumber();

    // 1부터 6까지의 랜덤 숫자를 산출하는 메서드
    private int GetRandomDiceNumber()
    {
        return Random.Range(1, 7);
    }
}
