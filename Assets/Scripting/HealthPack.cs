using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}