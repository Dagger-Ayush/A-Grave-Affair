using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectHovering : MonoBehaviour
{
    public static ObjectHovering instance;

    [Header("Detection Settings")]
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float soundDelay = 3f;

    [Header("References")]
    [SerializeField] private AudioSource cursorAudioClip;
    public InspectionTutorial inspectionTutorial;

    private InteractClueManager interactClueManager;
    private bool isSoundPlayed = false;
    private Vector2 cursorHotspot;

    public static bool isRunning = false; // controlled by CursorHoverOverClue
    private static bool isBusy = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (InspectionTutorial.Instance != null)
        {
            if (!InspectionTutorial.Instance.isRotationComplete) return;
        }
        if (Camera.main == null || Mouse.current == null) return;
        ObjectDetecting();
    }

    private void ObjectDetecting()
    {
        // 🛑 Skip if UI clue system is running
        if (isRunning || !ObjectPickHandler.isCollected) return;

        if (ObjectPickHandler.isXrayEnabled) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Collider[] colliderArray = Physics.OverlapSphere(hit.point, radius);

            bool objectInRange = false;
            bool mouseHovering = false;

            foreach (Collider collider in colliderArray)
            {
                if (collider == null) continue;

                if (collider.TryGetComponent(out InteractClueManager clueManager) && clueManager != null && !clueManager.isFinished)
                {
                    interactClueManager = clueManager;

                    float minRange = clueManager.HoveringMinRange;
                    float maxRange = clueManager.HoveringMaxRange;
                    float distance = Vector3.Distance(hit.point, collider.transform.position);

                    if (distance <= maxRange && ObjectPickHandler.isCollected)
                    {
                        if (distance <= clueManager.HoveringRange)
                            mouseHovering = true;

                        objectInRange = true;
                        break;
                    }
                }
            }

            if (objectInRange)
            {
                interactClueManager?.StartRepelEffect();

                if (!isSoundPlayed && !isBusy && cursorAudioClip != null)
                {
                    cursorAudioClip.Play();
                    StartCoroutine(Delay());
                }
            }
            else
            {
                interactClueManager?.StopRepelEffect();
            }

            if (CursorManager.Instance != null)
            {
                if (mouseHovering)
                {
                    CursorManager.Instance.SetCursor(CursorState.Clue);

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (interactClueManager != null)
                            StartCoroutine(WordPicking(interactClueManager));
                    }
                }
                else
                {
                    if (!TabletManager.isTabletOpen)
                        CursorManager.Instance.SetCursor(CursorState.Normal);
                    else
                        CursorManager.Instance.SetCursor(CursorState.Tablet);
                }
            }
        }
    }

    private IEnumerator WordPicking(InteractClueManager interactClueManager)
    {
        if (interactClueManager == null)
        {
            isBusy = false;
            yield break;
        }

        isBusy = true;

        Cursor.lockState = CursorLockMode.Locked;
        ObjectPickHandler.isMouseLocked = true;

        interactClueManager.ClueIndication();

        yield return new WaitForSeconds(1.8f);

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetCursor(CursorState.Normal);

        ObjectPickHandler.isMouseLocked = false;
        Cursor.lockState = CursorLockMode.None;

        interactClueManager.isFinished = true;
        isBusy = false;
    }

    private IEnumerator Delay()
    {
        isSoundPlayed = true;
        yield return new WaitForSeconds(soundDelay);
        isSoundPlayed = false;
    }
}
