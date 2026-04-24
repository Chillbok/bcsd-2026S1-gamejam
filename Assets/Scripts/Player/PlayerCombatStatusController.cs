using UnityEngine;

public class PlayerCombatStatusController : MonoBehaviour
{
    public bool IsDodging {get; set;}
    public bool IsDefending {get; set;}

    // 스탯들
    private float _damage = 1;
    public float Damage {get => _damage; set => _damage = value;}
    public float MaxHp {get; set;}
    public float CurrentHp {get; set;}

	private void Start()
	{
		MaxHp = EncounterManager.Instance.PlayerMaxHp;
        CurrentHp = EncounterManager.Instance.PlayerCurrentHp;
	}
}
