using System;
using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public event Action OnAttack;
    public event Action OnDodgeLeft;
    public event Action OnDodgeRight;
    public event Action OnDefend;
    
    private PlayerCombatStatusController _playerCombatStatusController;

	private void Awake()
	{
		_playerCombatStatusController = GetComponent<PlayerCombatStatusController>();
	}

    public void CallAttack()
    {
        OnAttack?.Invoke();
    }

    public void CallDodgeLeft()
    {
        OnDodgeLeft?.Invoke();
    }

    public void CallDodgeRight()
    {
        OnDodgeRight?.Invoke();
    }

    public void CallDefend()
    {
        OnDefend?.Invoke();
    }

    private IEnumerator WaitRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
    
    public void Attack()
    {
        
    }
}
