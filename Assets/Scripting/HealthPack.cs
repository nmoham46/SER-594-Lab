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

        pickupHandled = true;
        if (inventory.AddItem("Health Pack"))
        {
            Destroy(gameObject);
        }
    }
}
