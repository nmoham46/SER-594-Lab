using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxItems = 5;

    private List<string> items = new List<string>();

    public int ItemCount => items.Count;
    public int MaxItems => maxItems;

    public bool AddItem(string itemName)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        items.Add(itemName);

        Debug.Log(
            "Picked up: " + itemName +
            " | Inventory: " + items.Count + "/" + maxItems
        );

        return true;
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public bool RemoveItem(string itemName)
    {
        return items.Remove(itemName);
    }

    public string GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;

        return items[index];
    }

    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
            return false;

        items.RemoveAt(index);
        return true;
    }
}