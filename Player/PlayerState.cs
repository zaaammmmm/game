using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    public bool isFrozen = false;
    public bool isCrouching = false;
    public bool isRunning = false;

    void Awake()
    {
        Instance = this;
    }
}