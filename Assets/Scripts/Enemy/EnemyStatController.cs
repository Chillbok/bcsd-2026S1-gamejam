using UnityEngine;

public class EnemyStatController : MonoBehaviour
{
    [SerializeField]
    private float _maxHp = 100;
    public float MaxHp {get => _maxHp; private set => _maxHp = value;}
    [SerializeField]
    private float _currentHp;
    public float CurrentHp {get => _currentHp; set => _currentHp = value;}

	private void Start()
	{
        // TODO: 필드에 적이 여러 마리일 경우, 고유 ID를 부여하여 어떤 적의 체력인지 식별하는 로직이 추후 필요합니다.
        if (EncounterManager.Instance != null && EncounterManager.Instance.EnemyMaxHp > 0)
        {
            MaxHp = EncounterManager.Instance.EnemyMaxHp;
            CurrentHp = EncounterManager.Instance.EnemyCurrentHp;
        }
        else
        {
            CurrentHp = MaxHp;
        }
	}
}
