using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ObjectHovering : MonoBehaviour
{
    float radius = 0.1f;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private Texture2D cursorTextureInRange;
    [SerializeField] private Texture2D cursorTextureOutRange;

    [SerializeField] private AudioSource cursorAudioClip;
    private bool isSoundPlayed = false;
    private InteractClueManager interactClueManager;
    private Vector2 cursorHotspot;

    void Update()
    {

        ObjectDectecting();

    }
    void ObjectDectecting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit))
        {
            float minRange = 0.09f;
            float maxRange = 0.1f;

            Collider[] colliderArray = Physics.OverlapSphere(hit.point, radius);

            bool objectInRange = false;
            bool mouseHovering = false;
            foreach (Collider collider in colliderArray)
            {
                if (collider.gameObject.CompareTag("TriggerObject") && !collider.GetComponent<InteractClueManager>().isFinished)
                {
                    float distance = Vector3.Distance(hit.point, collider.transform.position);
                    if (playerInteract.GetObjectPickHandler() == null) return;
                    if (distance >= minRange && distance <= maxRange && playerInteract.GetObjectPickHandler().isPicked)
                    {
                        objectInRange = true;
                        break;
                    }
                    if (distance <= 0.07f && playerInteract.GetObjectPickHandler().isPicked)
                    {
                        mouseHovering = true;
                        break;
                    }

                    interactClueManager = collider.GetComponent<InteractClueManager>();
                }


            }

            cursorHotspot = new Vector2(cursorTextureInRange.width / 2, cursorTextureInRange.height / 2);

            if (objectInRange)
            {
                if (!isSoundPlayed)
                {
                    cursorAudioClip.Play();
                    isSoundPlayed = true;
                }

            }
            else
            {
                isSoundPlayed = false;

            }
            if (mouseHovering)
            {

                Cursor.SetCursor(cursorTextureInRange, cursorHotspot, CursorMode.Auto);

                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(WordPicking(interactClueManager));
                }
            }
            else
            {
                Cursor.SetCursor(cursorTextureOutRange, cursorHotspot, CursorMode.Auto);
                Cursor.lockState = CursorLockMode.None;
                mouseHovering = false;
            }

        }

    }
    IEnumerator WordPicking(InteractClueManager interactClueManager)
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInteract.GetObjectPickHandler().isMouseLocked = true;
        interactClueManager.ClueIndication();
        yield return new WaitForSeconds(1.8f);
        Cursor.SetCursor(cursorTextureOutRange, cursorHotspot, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        playerInteract.GetObjectPickHandler().isMouseLocked = false;
        interactClueManager.isFinished = true;
    }

}
