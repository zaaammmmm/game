using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Key Requirement")]
    public string requiredKey = "MainKey";

    [Header("Door Settings")]
    public bool opened = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Auto Progress")]
    public bool completeObjectiveWhenOpened = true;

    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isOpening = false;

    void Start()
    {
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );
    }

    void Update()
    {
        if (isOpening)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                openSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                isOpening = false;
            }
        }
    }

    public void Interact()
    {
        if (opened || isOpening) return;

        if (InventorySystem.Instance.HasItem(requiredKey))
        {
            opened = true;
            isOpening = true;

            Debug.Log("Pintu terbuka");

            if (completeObjectiveWhenOpened &&
                ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.NextStep();
            }
        }
        else
        {
            Debug.Log("Pintu Terkunci. Butuh key: " + requiredKey);
        }
    }

    public string GetInteractText()
    {
        if (opened)
            return "";

        if (InventorySystem.Instance.HasItem(requiredKey))
            return "[E] Buka Pintu";

        return "[E] Pintu Terkunci";
    }
}