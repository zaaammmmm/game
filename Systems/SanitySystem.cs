using UnityEngine;
using System.Collections;

public class SanitySystem : MonoBehaviour
{
    public static SanitySystem Instance;

    public enum MentalState
    {
        Calm,
        Uneasy,
        Panic,
        Insane
    }

    [Header("Values")]
    public float maxSanity = 100f;
    public float currentSanity = 100f;

    [Header("Rates")]
    public float darkDrain = 5f;
    public float lightRecover = 8f;
    public float ghostSightDrain = 12f;
    public float ghostNearDrain = 18f;

    [Header("References")]
    public AudioSource audioSource;
    public AudioClip heartbeatClip;
    public AudioClip whisperClip;

    public MentalState currentState;

    private Transform player;
    private Camera mainCam;
    private Vector3 camStartLocalPos;
    private float whisperTimer;

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
        currentSanity = maxSanity;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (mainCam != null)
            camStartLocalPos = mainCam.transform.localPosition;
    }

    void Update()
    {
        if (player == null)
            TryFindPlayer();

        if (mainCam == null)
            mainCam = Camera.main;

        HandleEnvironment();
        HandleGhostPressure();
        ClampSanity();
        UpdateMentalState();
        HandleEffects();
        CheckDeath();
    }

    void AutoAssign()
    {
        // AudioSource otomatis
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Camera otomatis
        if (mainCam == null)
            mainCam = Camera.main;
    }

    void TryFindPlayer()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;
    }

    #region CORE

    void HandleEnvironment()
    {
        if (FlashlightSystem.Instance != null &&
            FlashlightSystem.Instance.IsOn())
        {
            Recover(lightRecover);
        }
        else
        {
            Drain(darkDrain);
        }
    }

    void HandleGhostPressure()
    {
        if (player == null)
            return;

        GhostAI[] ghosts = FindObjectsOfType<GhostAI>();

        foreach (GhostAI ghost in ghosts)
        {
            if (ghost == null) continue;

            float dist =
                Vector3.Distance(player.position, ghost.transform.position);

            if (dist < 8f)
                Drain(ghostNearDrain);

            if (dist < 14f && CanSeeGhost(ghost.transform))
                Drain(ghostSightDrain);
        }
    }

    bool CanSeeGhost(Transform ghost)
    {
        if (mainCam == null || ghost == null)
            return false;

        Vector3 dir =
            (ghost.position - mainCam.transform.position).normalized;

        if (Physics.Raycast(
            mainCam.transform.position,
            dir,
            out RaycastHit hit,
            20f))
        {
            if (hit.transform == ghost)
                return true;
        }

        return false;
    }

    #endregion

    #region STATES

    void UpdateMentalState()
    {
        if (currentSanity >= 70f)
            currentState = MentalState.Calm;
        else if (currentSanity >= 40f)
            currentState = MentalState.Uneasy;
        else if (currentSanity >= 15f)
            currentState = MentalState.Panic;
        else
            currentState = MentalState.Insane;
    }

    #endregion

    #region EFFECTS

    void HandleEffects()
    {
        switch (currentState)
        {
            case MentalState.Calm:
                StopHeartbeat();
                ResetCamera();
                break;

            case MentalState.Uneasy:
                PlayHeartbeat(0.3f);
                ResetCamera();
                break;

            case MentalState.Panic:
                PlayHeartbeat(0.6f);
                WhisperRandom();
                CameraShake(0.02f);
                break;

            case MentalState.Insane:
                PlayHeartbeat(1f);
                WhisperRandom();
                CameraShake(0.05f);
                HallucinationChance();
                break;
        }
    }

    void PlayHeartbeat(float volume)
    {
        if (audioSource == null || heartbeatClip == null)
            return;

        if (audioSource.clip != heartbeatClip)
        {
            audioSource.clip = heartbeatClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        audioSource.volume = volume;
    }

    void StopHeartbeat()
    {
        if (audioSource == null)
            return;

        if (audioSource.clip == heartbeatClip)
            audioSource.Stop();
    }

    void WhisperRandom()
    {
        if (audioSource == null || whisperClip == null)
            return;

        whisperTimer -= Time.deltaTime;

        if (whisperTimer <= 0f)
        {
            audioSource.PlayOneShot(whisperClip, 0.6f);
            whisperTimer = Random.Range(4f, 8f);
        }
    }

    void CameraShake(float power)
    {
        if (mainCam == null)
            return;

        mainCam.transform.localPosition =
            camStartLocalPos + Random.insideUnitSphere * power;
    }

    void ResetCamera()
    {
        if (mainCam == null)
            return;

        mainCam.transform.localPosition = camStartLocalPos;
    }

    void HallucinationChance()
    {
        if (Random.value < 0.002f)
        {
            Debug.Log("Fake ghost seen!");
        }
    }

    #endregion

    #region HELPERS

    public void Drain(float amount)
    {
        currentSanity -= amount * Time.deltaTime;
    }

    public void Recover(float amount)
    {
        currentSanity += amount * Time.deltaTime;
    }

    void ClampSanity()
    {
        currentSanity =
            Mathf.Clamp(currentSanity, 0f, maxSanity);
    }

    void CheckDeath()
    {
        if (currentSanity <= 0f)
        {
            Debug.Log("PLAYER LOST SANITY");
        }
    }

    public float Percent()
    {
        return currentSanity / maxSanity;
    }

    #endregion
}