using UnityEngine;

public class SanitySystem : MonoBehaviour
{
    public static SanitySystem Instance;

    public float maxSanity = 100f;
    public float currentSanity = 100f;

    [Header("Rates")]
    public float darkDrain = 6f;
    public float lightRecover = 10f;
    public float ghostDrain = 20f;

    [Header("References")]
    public FlashlightSystem flashlight;

    private Transform player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentSanity = maxSanity;
    }

    void Update()
    {
        HandleSanity();
        CheckDeath();
    }

    void HandleSanity()
    {
        if (flashlight != null && flashlight.GetComponent<Light>().enabled)
        {
            currentSanity += lightRecover * Time.deltaTime;
        }
        else
        {
            currentSanity -= darkDrain * Time.deltaTime;
        }

        GhostAI[] ghosts = FindObjectsOfType<GhostAI>();

        foreach (GhostAI ghost in ghosts)
        {
            float dist = Vector3.Distance(player.position, ghost.transform.position);

            if (dist < 6f)
            {
                currentSanity -= ghostDrain * Time.deltaTime;
            }
        }

        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
    }

    void CheckDeath()
    {
        if (currentSanity <= 0)
        {
            Debug.Log("PLAYER INSANE / DEAD");
        }
    }

    public float GetPercent()
    {
        return currentSanity / maxSanity;
    }
}