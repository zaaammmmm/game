using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    public Light flashlight;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float drainRate = 8f;

    [Header("Light Power")]
    public float maxIntensity = 3f;
    public float minIntensity = 0.5f;

    [Header("Reload")]
    public float reloadTime = 2f;

    private bool isOn = true;
    private bool isReloading = false;

    void Start()
    {
        currentBattery = maxBattery;
    }

    void Update()
    {
        HandleToggle();
        HandleDrain();
        HandleReload();
        UpdateLight();
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isReloading)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }

    void HandleDrain()
    {
        if (isOn && currentBattery > 0)
        {
            currentBattery -= drainRate * Time.deltaTime;

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isOn = false;
                flashlight.enabled = false;
            }
        }
    }

    void HandleReload()
    {
        if (Input.GetKey(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadBattery());
        }
    }

    System.Collections.IEnumerator ReloadBattery()
    {
        isReloading = true;

        flashlight.enabled = false;

        yield return new WaitForSeconds(reloadTime);

        currentBattery = maxBattery;
        isOn = true;
        flashlight.enabled = true;

        isReloading = false;
    }

    void UpdateLight()
    {
        if (!flashlight.enabled) return;

        float percent = currentBattery / maxBattery;

        flashlight.intensity = Mathf.Lerp(minIntensity, maxIntensity, percent);

        if(currentBattery < 20f && flashlight.enabled)
        {
            flashlight.enabled = Random.value > 0.05f;
        }
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}