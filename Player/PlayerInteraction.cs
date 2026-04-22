using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public Text hintText;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        CheckInteract();
    }

    void CheckInteract()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                hintText.text = interactable.GetInteractText();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }

                return;
            }
        }

        hintText.text = "";
    }
}