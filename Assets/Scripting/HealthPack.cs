using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Inventory inventory = other.GetComponent<Inventory>();

        if (inventory != null)
        {
            bool added = inventory.AddItem("Health Pack");

            if (added)
            {
                Destroy(gameObject);
            }
        }
    }
}