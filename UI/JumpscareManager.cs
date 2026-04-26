using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpscareManager : MonoBehaviour
{
    public static JumpscareManager Instance;

    public Image bloodFlash;
    public Image blackFade;
    public GameObject gameOverPanel;

    public AudioSource screamAudio;

    void Awake()
    {
        Instance = this;
    }

    public void StartJumpscare(Transform ghost)
    {
        StartCoroutine(JumpscareSequence(ghost));
    }

    IEnumerator JumpscareSequence(Transform ghost)
    {
        PlayerFreeze(true);

        Camera cam = Camera.main;

        screamAudio.Play();

        float t = 0f;

        while (t < 1.2f)
        {
            t += Time.deltaTime;

            Vector3 dir = (ghost.position - cam.transform.position).normalized;

            Quaternion lookRot = Quaternion.LookRotation(dir);
            cam.transform.rotation =
                Quaternion.Slerp(cam.transform.rotation, lookRot, 8f * Time.deltaTime);

            ghost.position = Vector3.Lerp(
                ghost.position,
                cam.transform.position + cam.transform.forward * 0.7f,
                5f * Time.deltaTime);

            SetAlpha(bloodFlash, Mathf.PingPong(t * 2f, 0.7f));

            yield return null;
        }

        yield return StartCoroutine(FadeBlack());

        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator FadeBlack()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            SetAlpha(blackFade, t);
            yield return null;
        }
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    void PlayerFreeze(bool state)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();

            foreach(var s in scripts)
                s.enabled = !state;
        }
    }
}