using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MotelLobby : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineCamera isoCam;
    public Transform body;        // Dummy that camera follows
    public Transform player;      // Start point of the camera
    public float lerpSpeed = 2f;  // Speed of the initial pan

    [Header("Gameplay")]
    public ObjectInteract motelStartDialogs;
    public ObjectInteract DialogsPhase_2;
    public ObjectInteract enablingInteract;
    public ObjectPickHandler[] enablingInspect;

    // --- private fields ---
    Vector3 targetPos;
    Quaternion targetRot;
    bool introPanDone = false;

    void Start()
    {
        // Save the real body position & rotation (final camera spot)
        targetPos = body.position;
        targetRot = body.rotation;

        // Start the dummy at the player’s location so camera starts there
        body.position = player.position;
        body.rotation = player.rotation;

        // Camera follows the dummy
        isoCam.Follow = body;

        motelStartDialogs.enabled = true;
    }

    void Update()
    {
        // ------------------- Initial camera pan -------------------
        if (!introPanDone)
        {
            // Smoothly move dummy from player to body
            body.position = Vector3.Lerp(
                body.position,
                targetPos,
                Time.deltaTime * lerpSpeed
            );

            body.rotation = Quaternion.Lerp(
                body.rotation,
                targetRot,
                Time.deltaTime * lerpSpeed
            );

            if (Vector3.Distance(body.position, targetPos) < 0.05f)
            {
                body.position = targetPos;
                body.rotation = targetRot;
                introPanDone = true;
            }
        }
        if (introPanDone && motelStartDialogs && motelStartDialogs.isAutoComplete)
        {
            // After phase 1, smoothly move camera dummy to follow player
            body.position = Vector3.Lerp(body.position, player.position, Time.deltaTime * lerpSpeed);
            body.rotation = Quaternion.Lerp(body.rotation, player.rotation, Time.deltaTime * lerpSpeed);

            DialogsPhase_2.enabled = true;
            motelStartDialogs.enabled = false;
        }

        if (DialogsPhase_2 && DialogsPhase_2.isAutoCompleteNearObject)
        {
            ImageFade.instance.FadeInOut();
            StartCoroutine(ObjectsEnable());
        }
    }
    IEnumerator ObjectsEnable()
    {
        yield return new WaitForSeconds(1.5f);
        enablingInteract.enabled = true;
        enablingInteract.shouldWork = true;

        enablingInspect[0].enabled = true;
        enablingInspect[0].shouldWork = true;
        enablingInspect[1].enabled = true;
        enablingInspect[1].shouldWork = true;

        DialogsPhase_2.enabled = false;
    }

    }
