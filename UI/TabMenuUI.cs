using UnityEngine;
using UnityEngine.UI;

public class TabMenuUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject tabPanel;

    [Header("Objective")]
    public Text objectiveText;

    [Header("Key Icons")]
    public Image introKeyIcon;
    public Image mainKeyIcon;
    public Image exitKeyIcon;
    public Image generatorKeyIcon;

    [Header("Other Item Icons")]
    public Image gasolineIcon;
    public Image oilIcon;

    [Header("Colors")]
    public Color ownedColor = new Color(1f, 0.84f, 0f, 1f); // emas
    public Color notOwnedColor = Color.gray;

    void Start()
    {
        tabPanel.SetActive(false);
    }

    void Update()
    {
        HandleTabInput();

        if (tabPanel.activeSelf)
        {
            UpdateObjectiveText();
            UpdateIcons();
        }
    }

    void HandleTabInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabPanel.SetActive(true);
            UpdateObjectiveText();
            UpdateIcons();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            tabPanel.SetActive(false);
        }
    }

    void UpdateObjectiveText()
    {
        if (ObjectiveManager.Instance != null)
        {
            objectiveText.text = ObjectiveManager.Instance.GetCurrentObjective();
        }
    }

    void UpdateIcons()
    {
        if (InventorySystem.Instance == null) return;

        SetIcon(introKeyIcon, InventorySystem.Instance.HasItem("IntroKey"));
        SetIcon(mainKeyIcon, InventorySystem.Instance.HasItem("MainKey"));
        SetIcon(exitKeyIcon, InventorySystem.Instance.HasItem("ExitKey"));
        SetIcon(generatorKeyIcon, InventorySystem.Instance.HasItem("GeneratorKey"));

        SetIcon(gasolineIcon, InventorySystem.Instance.HasItem("Gasoline"));
        SetIcon(oilIcon, InventorySystem.Instance.HasItem("Oil"));
    }

    void SetIcon(Image icon, bool owned)
    {
        if (icon == null) return;

        icon.color = owned ? ownedColor : notOwnedColor;
    }
}
