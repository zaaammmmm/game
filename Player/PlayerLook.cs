using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 150f;

    float xRotation = 0f;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        // ESC buka cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        // Klik kiri kunci lagi
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX =
            Input.GetAxis("Mouse X") *
            sensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            sensitivity *
            Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}