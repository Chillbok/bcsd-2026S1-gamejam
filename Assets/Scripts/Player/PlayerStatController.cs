using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField]
    private float _maxHp = 100;
    public float MaxHp {get => _maxHp; private set => _maxHp = value;}
    [SerializeField]
    private float _currentHp;
    public float CurrentHp {get => _currentHp; set => _currentHp = value;}

	private void Start()
	{
        if (EncounterManager.Instance != null && EncounterManager.Instance.PlayerMaxHp > 0)
        {
            // 전투에서 돌아왔거나 이미 데이터가 저장되어 있는 경우
            MaxHp = EncounterManager.Instance.PlayerMaxHp;
            CurrentHp = EncounterManager.Instance.PlayerCurrentHp;
        }
        else
        {
            // 게임 최초 시작 시
            CurrentHp = MaxHp;
        }
	}
}
