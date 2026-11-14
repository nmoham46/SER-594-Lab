using UnityEngine;

public class BookPickup : MonoBehaviour
{
    private const string BookName = "The Book of Hogwarts";

    private void OnTriggerEnter(Collider other)
    {
        Inventory inventory = other.GetComponentInParent<Inventory>();

        if (inventory == null)
            return;

        bool added = inventory.AddItem(BookName);

        if (added)
        {
            Debug.Log("You found " + BookName + "!");
            Destroy(gameObject);
        }
    }
}