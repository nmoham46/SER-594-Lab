using UnityEngine;

// Generic pickup for items that have no special effect (e.g. Golden Snitch, Magic Wand).
// Set the item name in the Inspector to make each item distinct.
public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName = "Golden Snitch";

    private bool pickupHandled;

    public string ItemName => itemName;

    private void OnTriggerEnter(Collider other)
    {
        if (pickupHandled)
            return;

        Inventory inventory = other.GetComponentInParent<Inventory>();
        if (inventory == null)
            return;

        if (inventory.AddItem(itemName))
        {
            pickupHandled = true;
            Debug.Log("You found a " + itemName + "!");
            Destroy(gameObject);
        }
    }
}
