using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    private bool pickupHandled;

    private void OnTriggerEnter(Collider other)
    {
        if (pickupHandled)
            return;

        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            return;

        pickupHandled = true;
        health.TakeDamage(damage);

        Inventory inventory = other.GetComponentInParent<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem("Damage Pack");
        }

        Destroy(gameObject);
    }
}
