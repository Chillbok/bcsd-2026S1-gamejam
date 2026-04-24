using System;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public event Action OnAttack;
    public event Action OnDodgeLeft;
    public event Action OnDodgeRight;
    public event Action OnDefend;
}
