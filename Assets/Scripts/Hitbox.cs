using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hitbox : MonoBehaviour
{
    [Header("히트박스 주인 태그")]
    [SerializeField]
    private string _masterTag = "Player";

	private void OnTriggerEnter2D(Collider2D collision)
	{
        string combatSceneName = "CombatScene";
        // 히트박스가 플레이어 소유일 때
        if (_masterTag == "Player" && collision.CompareTag("Enemy"))
        {
            EnemyStatController _enemyStatController = collision.GetComponent<EnemyStatController>();
            PlayerStatController _playerStatController = GetComponentInParent<PlayerStatController>();
            
            GivePlayerStats(_playerStatController, _enemyStatController);

            SceneManager.LoadScene(combatSceneName);
        }
        // 히트박스가 적 소유일 때
        else if (_masterTag == "Enemy" && collision.CompareTag("Player"))
        {
            EnemyStatController _enemyStatController = GetComponentInParent<EnemyStatController>();
            PlayerStatController _playerStatController = collision.GetComponent<PlayerStatController>();
            
            GivePlayerStats(_playerStatController, _enemyStatController);

            SceneManager.LoadScene(combatSceneName);
        }
	}
    
    private void GivePlayerStats(PlayerStatController playerStatController, EnemyStatController enemyStatController)
    {
        EncounterManager.Instance.PlayerMaxHp = playerStatController.MaxHp;
        EncounterManager.Instance.PlayerCurrentHp = playerStatController.CurrentHp;
        
        EncounterManager.Instance.EnemyMaxHp = enemyStatController.MaxHp;
        EncounterManager.Instance.EnemyCurrentHp = enemyStatController.CurrentHp;
    }
}
