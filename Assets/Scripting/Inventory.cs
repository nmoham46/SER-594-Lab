using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxItems = 5;

    private List<string> items = new List<string>();

    public int ItemCount => items.Count;
    public int MaxItems => maxItems;

    // Fired whenever an item is added or removed (the HUD listens to this)
    public event Action OnInventoryChanged;

    // Fired when an item can't be picked up (the HUD shows it as a warning)
    public event Action<string> OnPickupFailed;

    public bool IsBook(string itemName)
    {
        return itemName != null && itemName.StartsWith("The Book of");
    }

    // Add an item to the inventory
    public bool AddItem(string itemName)
    {
        // Inventory cannot hold more than 5 items
        if (items.Count >= maxItems)
        {
            Debug.Log("Inventory is full!");
            OnPickupFailed?.Invoke("Inventory is full! Can't pick up " + itemName + ".");
            return false;
        }

        // Player can only carry one Book
        if (itemName.StartsWith("The Book of") && HasBook())
        {
            Debug.Log("You already have a Book!");
            OnPickupFailed?.Invoke("You can only carry one Book!");
            return false;
        }

        items.Add(itemName);
        OnInventoryChanged?.Invoke();

        Debug.Log(
            "Picked up: " + itemName +
            " | Inventory: " + items.Count + "/" + maxItems
        );

        return true;
    }

    // Check whether the player currently has a Book
    public bool HasBook()
    {
        foreach (string item in items)
        {
            if (item.StartsWith("The Book of"))
            {
                return true;
            }
        }

        return false;
    }

    // Check for a specific item
    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    // Remove a specific item
    public bool RemoveItem(string itemName)
    {
        bool removed = items.Remove(itemName);
        if (removed)
            OnInventoryChanged?.Invoke();
        return removed;
    }

    // Get an item using its inventory slot/index
    public string GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return null;
        }

        return items[index];
    }

    // Remove an item using its inventory slot/index
    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return false;
        }

        items.RemoveAt(index);
        OnInventoryChanged?.Invoke();
        return true;
    }
}