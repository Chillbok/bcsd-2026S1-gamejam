using Unity.VisualScripting;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("히트박스 인식 대상 태그")]
    [SerializeField]
    private string _targetTag = "Enemy";

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// if (collision.CompareTag(_targetTag)) Debug.Log("적 명중");
	}
}
