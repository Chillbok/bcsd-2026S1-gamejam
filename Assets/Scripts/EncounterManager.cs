// 인카운터 전 필드에서 데이터 관리하는 메서드
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }
    
    // 보낼 변수
    public float PlayerMaxHp {get; set;}
    public float PlayerCurrentHp {get; set;}
    public float EnemyMaxHp {get; set;}
    public float EnemyCurrentHp {get; set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
