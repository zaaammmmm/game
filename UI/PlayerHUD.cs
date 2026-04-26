using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;

    [Header("===== STATUS BAR =====")]
    public Image staminaFill;
    public Image sanityFill;

    [Header("===== BATTERY =====")]
    public TMP_Text batteryText;
    public TMP_Text batteryWarningText;
    public Image batteryIcon;

    [Header("===== TEXT =====")]
    public TMP_Text objectiveText;
    public TMP_Text promptText;
    public TMP_Text ghostAlertText;

    [Header("===== EFFECT =====")]
    public Image vignette;
    public Image damageFlash;

    [Header("===== JOURNAL =====")]
    public GameObject journalPanel;

    [Header("Key Icons")]
    public Image key1;
    public Image key2;
    public Image key3;
    public Image key4;

    [Header("Item Icons")]
    public Image gasolineIcon;
    public Image oilIcon;

    [Header("Colors")]
    public Color staminaNormal = Color.green;
    public Color staminaLow = Color.red;

    public Color sanityCalm = Color.cyan;
    public Color sanityUneasy = new Color(0.6f, 0.2f, 1f);
    public Color sanityDanger = Color.red;

    public Color lockedColor = Color.gray;
    public Color unlockedColor = new Color(1f, 0.8f, 0f);

    private PlayerMovementV2 player;
    private SanitySystem sanity;
    private FlashlightSystem flash;

    private bool journalOpen = false;

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
        FindSystems();

        if (promptText != null)
            promptText.text = "";

        if (ghostAlertText != null)
            ghostAlertText.gameObject.SetActive(false);

        if (batteryWarningText != null)
            batteryWarningText.gameObject.SetActive(false);

        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);

        if (journalPanel != null)
            journalPanel.SetActive(false);

        ResetInventoryIcons();
    }

    void Update()
    {
        FindSystems();

        UpdateStamina();
        UpdateSanity();
        UpdateBattery();
        UpdateEffects();
        HandleJournalInput();
    }

    void AutoAssign()
    {
        if (vignette != null)
            vignette.color = new Color(0, 0, 0, 0);

        if (damageFlash != null)
            damageFlash.color = new Color(1, 0, 0, 0);
    }

    void FindSystems()
    {
        if (player == null)
            player = FindObjectOfType<PlayerMovementV2>();

        if (sanity == null)
            sanity = FindObjectOfType<SanitySystem>();

        if (flash == null)
            flash = FindObjectOfType<FlashlightSystem>();
    }

    #region ================== STAMINA ==================

    void UpdateStamina()
    {
        if (player == null || staminaFill == null)
            return;

        float value = player.GetStaminaPercent();

        staminaFill.fillAmount =
            Mathf.Lerp(staminaFill.fillAmount, value, Time.deltaTime * 8f);

        if (value > 0.35f)
            staminaFill.color = staminaNormal;
        else
            staminaFill.color =
                Color.Lerp(staminaLow, Color.yellow, value * 2f);
    }

    #endregion

    #region ================== SANITY ==================

    void UpdateSanity()
    {
        if (sanity == null || sanityFill == null)
            return;

        float value = sanity.Percent();

        sanityFill.fillAmount =
            Mathf.Lerp(sanityFill.fillAmount, value, Time.deltaTime * 8f);

        if (value > 0.70f)
            sanityFill.color = sanityCalm;
        else if (value > 0.35f)
            sanityFill.color = sanityUneasy;
        else
            sanityFill.color = sanityDanger;
    }

    #endregion

    #region ================== BATTERY ==================

    void UpdateBattery()
    {
        if (flash == null)
            return;

        int percent =
            Mathf.RoundToInt(flash.BatteryPercent() * 100f);

        if (batteryText != null)
            batteryText.text = "BATTERY " + percent + "%";

        if (batteryWarningText != null)
        {
            bool low = percent <= 20;

            batteryWarningText.gameObject.SetActive(low);

            if (low)
            {
                float a = Mathf.PingPong(Time.time * 3f, 1f);
                batteryWarningText.alpha = a;
            }
        }

        if (batteryIcon != null)
        {
            if (percent <= 20)
                batteryIcon.color =
                    Color.Lerp(Color.red, Color.white,
                    Mathf.PingPong(Time.time * 4f, 1f));
            else
                batteryIcon.color = Color.white;
        }
    }

    #endregion

    #region ================== SCREEN EFFECT ==================

    void UpdateEffects()
    {
        if (sanity == null || vignette == null)
            return;

        float value = sanity.Percent();

        float alpha = (1f - value) * 0.45f;

        Color c = vignette.color;
        c.a = Mathf.Lerp(c.a, alpha, Time.deltaTime * 4f);
        vignette.color = c;
    }

    public void FlashDamage()
    {
        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        if (damageFlash == null)
            yield break;

        Color c = damageFlash.color;
        c.a = 0.65f;
        damageFlash.color = c;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            c.a = Mathf.Lerp(0.65f, 0f, t);
            damageFlash.color = c;
            yield return null;
        }
    }

    #endregion

    #region ================== OBJECTIVE ==================

    public void ShowObjective(string msg)
    {
        StartCoroutine(ObjectiveRoutine(msg));
    }

    IEnumerator ObjectiveRoutine(string msg)
    {
        if (objectiveText == null)
            yield break;

        objectiveText.gameObject.SetActive(true);
        objectiveText.text = msg;
        objectiveText.alpha = 1f;

        yield return new WaitForSeconds(3f);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime;
            objectiveText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        objectiveText.gameObject.SetActive(false);
    }

    #endregion

    #region ================== PROMPT ==================

    public void ShowPrompt(string text)
    {
        if (promptText == null)
            return;

        promptText.text = text;
    }

    public void HidePrompt()
    {
        if (promptText == null)
            return;

        promptText.text = "";
    }

    #endregion

    #region ================== GHOST ALERT ==================

    public void ShowGhostAlert(string msg = "SESUATU MENDEKAT...")
    {
        StartCoroutine(GhostAlertRoutine(msg));
    }

    IEnumerator GhostAlertRoutine(string msg)
    {
        if (ghostAlertText == null)
            yield break;

        ghostAlertText.gameObject.SetActive(true);
        ghostAlertText.text = msg;
        ghostAlertText.alpha = 1f;

        yield return new WaitForSeconds(2f);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime;
            ghostAlertText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        ghostAlertText.gameObject.SetActive(false);
    }

    #endregion

    #region ================== JOURNAL ==================

    void HandleJournalInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleJournal();
        }
    }

    public void ToggleJournal()
    {
        if (journalPanel == null)
            return;

        journalOpen = !journalOpen;
        journalPanel.SetActive(journalOpen);

        Cursor.lockState =
            journalOpen ? CursorLockMode.None : CursorLockMode.Locked;

        Cursor.visible = journalOpen;
    }

    #endregion

    #region ================== INVENTORY ICON ==================

    void ResetInventoryIcons()
    {
        SetImage(key1, lockedColor);
        SetImage(key2, lockedColor);
        SetImage(key3, lockedColor);
        SetImage(key4, lockedColor);

        SetImage(gasolineIcon, lockedColor);
        SetImage(oilIcon, lockedColor);
    }

    void SetImage(Image img, Color col)
    {
        if (img != null)
            img.color = col;
    }

    public void UnlockKey(int index)
    {
        switch (index)
        {
            case 1: SetImage(key1, unlockedColor); break;
            case 2: SetImage(key2, unlockedColor); break;
            case 3: SetImage(key3, unlockedColor); break;
            case 4: SetImage(key4, unlockedColor); break;
        }
    }

    public void UnlockGasoline()
    {
        SetImage(gasolineIcon, unlockedColor);
    }

    public void UnlockOil()
    {
        SetImage(oilIcon, unlockedColor);
    }

    #endregion
}