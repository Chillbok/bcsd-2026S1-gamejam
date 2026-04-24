using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    
    // 참조변수
    EncounterManager _encounterManager = EncounterManager.Instance;
    
    // 스탯
    public float PlayerMaxHp {get; set;}
    public float PlayerCurrentHp {get; set;}
    public float EnemyMaxHp {get; set;}
    public float EnemyCurrentHp {get; set;}
    
    [Header("플레이어 데미지")]
    [SerializeField]
    private float _playerDmg = 2;
    public float PlayerDmg {get => _playerDmg; set => _playerDmg = value;}

    [Header("적 데미지")]
    [SerializeField]
    private float _enemyDmg = 10;
    public float EnemyDmg {get => _enemyDmg; set => _enemyDmg = value;}

    [Header("적 강공격 데미지 배수")]
    [SerializeField]
    private float multiplier = 1.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        _encounterManager.EnemyMaxHp = EnemyCurrentHp;
        _encounterManager.EnemyCurrentHp = EnemyCurrentHp;

        SceneManager.LoadScene("GamePlayScene");
    }
}
