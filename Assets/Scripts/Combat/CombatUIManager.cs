using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Header("UI 슬라이더")]
    [SerializeField] private Slider _leftSlider;
    [SerializeField] private Slider _rightSlider;

    private CombatEnemyController _enemyController;

    private void Awake()
    {
        _enemyController = FindFirstObjectByType<CombatEnemyController>();
    }

    private void OnEnable()
    {
        if (_enemyController != null)
        {
            _enemyController.OnAttackWarning += HandleAttackWarning;
        }
    }

    private void OnDisable()
    {
        if (_enemyController != null)
        {
            _enemyController.OnAttackWarning -= HandleAttackWarning;
        }
    }

    private void HandleAttackWarning(int attackNum, float warningDuration)
    {
        StartCoroutine(WarningRoutine(attackNum, warningDuration));
    }

    private IEnumerator WarningRoutine(int attackNum, float warningDuration)
    {
        Slider targetSlider1 = null;
        Slider targetSlider2 = null;

        if (attackNum == 1 || attackNum == 4) 
        {
            targetSlider1 = _leftSlider;
        }
        else if (attackNum == 2 || attackNum == 5) 
        {
            targetSlider1 = _leftSlider;
            targetSlider2 = _rightSlider;
        }
        else if (attackNum == 3 || attackNum == 6) 
        {
            targetSlider1 = _rightSlider;
        }

        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            elapsed += Time.deltaTime;
            // 시간에 비례하여 0에서 1로 증가
            float currentValue = elapsed / warningDuration;
            
            if (targetSlider1 != null) targetSlider1.value = currentValue;
            if (targetSlider2 != null) targetSlider2.value = currentValue;
            
            yield return null;
        }

        if (targetSlider1 != null) targetSlider1.value = 0f;
        if (targetSlider2 != null) targetSlider2.value = 0f;
    }
}
