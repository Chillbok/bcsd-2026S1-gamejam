using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }
    
    // 보낼 변수
    public PlayerStatController PlayerStatController {get; set;}
    public EnemyStatController EnemyStatController {get; set;}

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
