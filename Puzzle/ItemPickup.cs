using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public string itemID = "Gasoline";
    public string pickupText = "[E] Ambil Item";

    public void Interact()
    {
        InventorySystem.Instance.AddItem(itemID);
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return pickupText;
    }
}