using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
    public List<Item> items;
    public int maxSize;
    public int currentSize;

    public void Initialize(int maxSize)
    {
        items = new List<Item>();

        this.maxSize = maxSize;

        for (int i = 0; i < maxSize; i++)
        {
            // Fill inventory with empty items
            items.Add(null);
        }

        // Set curr size to 0
        currentSize = 0;
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public Item this[int index]
    {
        get { return items[index]; }
    }

    public void SetItem(Item item, int index)
    {
        // Check bounds
        if (index < 0 || index > maxSize) throw new System.Exception("INDEX OUT OF BOUNDS: " + index);

        // Set item
        items[index] = item;

        // Change size
        if (item != null) currentSize++;
        else currentSize--;
    }

    public void AddItemToEnd(Item item)
    {
        // If inventory is full, debug
        if (currentSize == maxSize) throw new System.Exception("INVENTORY IS FULL: " + currentSize);

        // Set end to item
        items[currentSize] = item;

        // Increase size
        currentSize++;
    }

    public void RemoveItem(int index)
    {
        // Check bounds
        if (index < 0 || index > maxSize) throw new System.Exception("INDEX OUT OF BOUNDS: " + index);

        // Set item
        items[index] = null;

        // Decrease size
        currentSize--;
    }

    public Inventory Copy()
    {
        var copy = Instantiate(this);

        return copy;
    }
}
