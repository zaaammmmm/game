using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    private HashSet<string> items = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(string itemID)
    {
        items.Add(itemID);
        Debug.Log("Dapat Item: " + itemID);
    }

    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }
}