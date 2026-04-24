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
		CurrentHp = MaxHp;
	}
}
