using UnityEngine;
using System.Collections;

public class FlashlightSystem : MonoBehaviour
{
    public static FlashlightSystem Instance;

    [Header("References")]
    public Light flashLight;
    public AudioSource audioSource;
    public AudioClip toggleClip;
    public AudioClip reloadClip;
    public Transform noiseOrigin;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float drainRate = 7f;

    [Header("Intensity")]
    public float maxIntensity = 3f;
    public float minIntensity = 0.3f;
    public float criticalBattery = 20f;

    [Header("Reload")]
    public float reloadDuration = 2.2f;
    public float reloadNoiseRadius = 14f;

    [Header("Flicker")]
    public float flickerSpeed = 0.08f;

    private bool isOn = true;
    private bool isReloading = false;
    private float flickerTimer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        AutoAssign();
    }

    void Start()
    {
        currentBattery = maxBattery;
        ApplyLightState();
    }

    void Update()
    {
        HandleInput();
        DrainBattery();
        UpdateIntensity();
        HandleCriticalFlicker();
    }

    void AutoAssign()
    {
        // Cari Light otomatis
        if (flashLight == null)
            flashLight = GetComponentInChildren<Light>();

        // Cari AudioSource otomatis
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Jika tidak ada AudioSource, buat baru
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Noise origin default = object ini
        if (noiseOrigin == null)
            noiseOrigin = transform;

        // Warning jika Light tidak ketemu
        if (flashLight == null)
            Debug.LogWarning("FlashlightSystem: Light belum ditemukan.");
    }

    #region INPUT

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isReloading)
        {
            ToggleFlashlight();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    void ToggleFlashlight()
    {
        if (currentBattery <= 0f)
            return;

        isOn = !isOn;
        ApplyLightState();

        if (toggleClip != null && audioSource != null)
            audioSource.PlayOneShot(toggleClip);
    }

    #endregion

    #region BATTERY

    void DrainBattery()
    {
        if (!isOn || isReloading)
            return;

        currentBattery -= drainRate * Time.deltaTime;

        if (currentBattery <= 0f)
        {
            currentBattery = 0f;
            isOn = false;
            ApplyLightState();
        }
    }

    void UpdateIntensity()
    {
        if (flashLight == null)
            return;

        if (!flashLight.enabled)
            return;

        float percent = currentBattery / maxBattery;

        flashLight.intensity =
            Mathf.Lerp(minIntensity, maxIntensity, percent);
    }

    #endregion

    #region RELOAD

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        isOn = false;
        ApplyLightState();

        if (reloadClip != null && audioSource != null)
            audioSource.PlayOneShot(reloadClip);

        EmitNoise(reloadNoiseRadius);

        yield return new WaitForSeconds(reloadDuration);

        currentBattery = maxBattery;
        isOn = true;
        isReloading = false;

        ApplyLightState();
    }

    #endregion

    #region FLICKER

    void HandleCriticalFlicker()
    {
        if (!isOn || isReloading)
            return;

        if (currentBattery > criticalBattery)
            return;

        if (flashLight == null)
            return;

        flickerTimer -= Time.deltaTime;

        if (flickerTimer <= 0f)
        {
            flashLight.enabled = !flashLight.enabled;
            flickerTimer = flickerSpeed + Random.Range(0f, 0.05f);
        }
    }

    #endregion

    #region NOISE

    void EmitNoise(float radius)
    {
        GhostAI[] ghosts = FindObjectsOfType<GhostAI>();

        foreach (GhostAI ghost in ghosts)
        {
            if (ghost != null && noiseOrigin != null)
                ghost.HearSound(noiseOrigin.position);
        }
    }

    #endregion

    #region HELPERS

    void ApplyLightState()
    {
        if (flashLight == null)
            return;

        flashLight.enabled =
            isOn &&
            !isReloading &&
            currentBattery > 0f;
    }

    public bool IsOn()
    {
        return isOn;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public float BatteryPercent()
    {
        return currentBattery / maxBattery;
    }

    public float BatteryValue()
    {
        return currentBattery;
    }

    #endregion
}