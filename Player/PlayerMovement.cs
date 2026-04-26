using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementV2 : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float crouchSpeed = 2f;

    public float gravity = -9.81f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina;
    public float drainRate = 22f;
    public float recoverRate = 18f;

    [Header("Crouch")]
    public float normalHeight = 2f;
    public float crouchHeight = 1.2f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;
    }

    void Update()
    {
        if (PlayerState.Instance.isFrozen)
            return;

        HandleMove();
        HandleGravity();
        HandleStamina();
        HandleCrouch();
    }

    void HandleMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        bool moving = move.magnitude > 0.1f;

        bool wantsRun = Input.GetKey(KeyCode.LeftShift);
        bool canRun = stamina > 5f && !PlayerState.Instance.isCrouching;

        PlayerState.Instance.isRunning = moving && wantsRun && canRun;

        float speed = walkSpeed;

        if (PlayerState.Instance.isRunning)
            speed = runSpeed;
        else if (PlayerState.Instance.isCrouching)
            speed = crouchSpeed;

        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleStamina()
    {
        if (PlayerState.Instance.isRunning)
            stamina -= drainRate * Time.deltaTime;
        else
            stamina += recoverRate * Time.deltaTime;

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            PlayerState.Instance.isCrouching =
                !PlayerState.Instance.isCrouching;
        }

        controller.height = PlayerState.Instance.isCrouching ?
            crouchHeight : normalHeight;
    }

    public float GetStaminaPercent()
    {
        return stamina / maxStamina;
    }
}