using UnityEngine;

public class PlayerNoiseEmitter : MonoBehaviour
{
    public float walkNoise = 4f;
    public float runNoise = 9f;
    public float crouchNoise = 1.5f;

    float timer = 0f;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0) return;

        if (PlayerState.Instance.isRunning)
        {
            Emit(runNoise);
            timer = 0.4f;
        }
        else if (PlayerState.Instance.isCrouching)
        {
            Emit(crouchNoise);
            timer = 0.8f;
        }
        else
        {
            Emit(walkNoise);
            timer = 0.6f;
        }
    }

    void Emit(float radius)
    {
        GhostAI[] ghosts = FindObjectsOfType<GhostAI>();

        foreach(var g in ghosts)
            g.HearSound(transform.position);
    }
}