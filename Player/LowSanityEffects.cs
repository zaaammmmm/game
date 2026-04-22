using UnityEngine;

public class LowSanityEffects : MonoBehaviour
{
    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float sanity = SanitySystem.Instance.GetPercent();

        if (sanity < 0.35f)
        {
            float power = (1f - sanity) * 0.05f;

            transform.localPosition = startPos +
                Random.insideUnitSphere * power;
        }
        else
        {
            transform.localPosition = startPos;
        }
    }
}