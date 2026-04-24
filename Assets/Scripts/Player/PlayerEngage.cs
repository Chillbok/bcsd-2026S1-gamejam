using UnityEngine;

public class PlayerEngage : MonoBehaviour
{
    [Header("인카운터 설정")]
    [SerializeField]
    private GameObject attackHitbox;

	private void Start()
	{
        DisableHitbox();
	}
    
    private void EnableHitbox()
    {
        attackHitbox.SetActive(true);
    }
    private void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }
}
