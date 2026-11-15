using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private bool pickupHandled;

    private void OnTriggerEnter(Collider other)
    {
        if (pickupHandled)
            return;

        Inventory inventory = other.GetComponentInParent<Inventory>();
        if (inventory == null)
            return;

        if (inventory.AddItem("Health Pack"))
        {
            pickupHandled = true;
            Destroy(gameObject);
        }
    }
}
