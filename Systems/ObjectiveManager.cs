using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("Popup Objective (di luar TAB)")]
    public Text objectiveText;

    public int currentStep = 0;

    private string currentObjective = "";
    private Coroutine hideRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetObjective();
        ShowTemporary();
    }

    // TAB sudah ditangani oleh TabMenuUI
    // Jadi ObjectiveManager tidak perlu lagi handle TAB

    public void NextStep()
    {
        currentStep++;
        SetObjective();
        ShowTemporary();
    }

    void SetObjective()
    {
        switch (currentStep)
        {
            case 0:
                currentObjective = "Cari jalan keluar kamar...";
                break;

            case 1:
                currentObjective = "Temukan IntroKey";
                break;

            case 2:
                currentObjective = "Buka pintu kamar";
                break;

            case 3:
                currentObjective = "Cari MainKey";
                break;

            case 4:
                currentObjective = "Cari ExitKey";
                break;

            case 5:
                currentObjective = "Keluar dari kontrakan";
                break;

            case 6:
                currentObjective = "Cari GeneratorKey";
                break;

            case 7:
                currentObjective = "Nyalakan Generator";
                break;

            case 8:
                currentObjective = "Listrik Menyala...";
                break;
        }

        if (objectiveText != null)
            objectiveText.text = currentObjective;
    }

    void ShowTemporary()
    {
        if (objectiveText == null) return;

        objectiveText.gameObject.SetActive(true);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterSeconds());
    }

    IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(3f);

        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);
    }

    public string GetCurrentObjective()
    {
        return currentObjective;
    }
}