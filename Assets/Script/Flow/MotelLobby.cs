using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MotelLobby : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineCamera isoCam;
    public Transform body;
    public Transform player;
    public float lerpSpeed = 2f;

    [Header("Gameplay Phase_1")]
    public ObjectInteract motelStartDialogs;
    public ObjectInteract DialogsPhase_2;
    public ObjectInteract enablingInteract;
    public ObjectPickHandler[] enablingInspect;
    public ObjectInteract enablingInteractafterPhase_1;
    public ObjectInteract enablingInteractafterPhase_2;
    private bool isObjectEnabled_Phase_1 = false;

    [Header("Gameplay Phase_2")]
    public PuzzleProgression puzzleProgression;
    public ObjectInteract motelGatherDialogs;
    private bool isObjectEnabled_Phase_2 = false;

    [Header("Final Page")]
    public ObjectPickHandler orderBookPickHandler;
    public ObjectInteract[] LastInteractionObjects;

    [Header("ON Sentence Complete")]
    public ObjectInteract ONsentenceCompleteTrigger;
    public ObjectInteract ONsentenceCompleteGreg;
    public ObjectInteract ONsentenceCompleteInteract_1;
    public ObjectInteract ONsentenceCompleteInteract_2;
    
    [Header("Doors to activate")]
    public GameObject doorToNancyRoom;
    public GameObject doorToOutsideMotel;

    // Private camera + phase flags
    private Vector3 targetPos;
    private Quaternion targetRot;
    private bool introPanDone = false;
    private bool isFinalDialogComplete = false;

    // --- Track currently active phase ---
    private int currentPhase = 0;

    public ObjectInteract[] suspectDialogs;
    void Start()
    {
        // Camera setup
        targetPos = body.position;
        targetRot = body.rotation;
        body.position = player.position;
        body.rotation = player.rotation;
        isoCam.Follow = body;

        motelStartDialogs.enabled = true;
    }

    private void OnDisable()
    {
        isoCam.Follow = player;
    }

    void Update()
    {
        // === Stop all earlier phases when final is reached ===
        if (isFinalDialogComplete || ONsentenceCompleteTrigger.isAutoComplete)
        {
            isoCam.Follow = player;
        }

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
            
            if (currentPhase == 0)
            {
                DialogsPhase_2.enabled = true;
                motelStartDialogs.enabled = false;
            }
        }

        // === Phase progression logic ===
        if (introPanDone && motelStartDialogs && motelStartDialogs.isAutoComplete && currentPhase == 0)
        {
            StartPhase(1);
        }

        if (currentPhase == 1 && DialogsPhase_2 && DialogsPhase_2.isAutoCompleteNearObject && !isObjectEnabled_Phase_1)
        {
            StartCoroutine(ObjectsEnablePhase_1());
        }

        if (currentPhase == 2 && puzzleProgression.isDialogEnabled && !isObjectEnabled_Phase_2)
        {
            StartCoroutine(ObjectsEnablePhase_2());
        }

        if (currentPhase == 3 && motelGatherDialogs.isAutoComplete)
        {
            StartCoroutine(ObjectsEnablePhase_3());
        }

        if (currentPhase == 4 && ONsentenceCompleteTrigger.isAutoComplete)
        {
            StartCoroutine(ObjectsEnablePhase_4());
        }
        if (currentPhase == 5 && ONsentenceCompleteGreg.isInteractionComplete)
        {
            StartCoroutine(ObjectsEnablePhase_5());
        }
        if (currentPhase == 6 && ONsentenceCompleteInteract_1.isAutoComplete)
        {
            ObjectsEnablePhase_6();
        }
        if (currentPhase == 7 && ONsentenceCompleteInteract_2.isAutoComplete)
        {
           StartCoroutine(ObjectsEnablePhase_7_R_1());
        }
        if (currentPhase == 8 && suspectDialogs[0].isInteractionComplete)
        {
            ObjectsEnablePhase_8_R_2();
        }
        if (currentPhase == 9 && suspectDialogs[1].isInteractionComplete)
        {
            ObjectsEnablePhase_9_M_1();
        }
    }

    // --------------------------- PHASE CONTROL ---------------------------

    void StartPhase(int phaseNumber)
    {
        // Prevent multiple phases from overlapping
        if (phaseNumber <= currentPhase) return;
        currentPhase = phaseNumber;

        Debug.Log($"=== Starting Phase {phaseNumber} ===");

        switch (phaseNumber)
        {
            case 1:
                DialogsPhase_2.enabled = true;
                motelStartDialogs.enabled = false;
                break;

            case 2:
                // Disable phase1 interactions
                enablingInteract.enabled = false;
                foreach (var inspect in enablingInspect)
                    inspect.enabled = false;
                break;

            case 3:
                motelGatherDialogs.enabled = true;
                break;

        }
    }

    IEnumerator ObjectsEnablePhase_1()
    {
        currentPhase = 2; // Move to Phase 2
        Debug.Log("Phase_1 Started");
        ImageFade.instance.FadeInOut();

        yield return new WaitForSeconds(1.5f);
        PosandAnimationUpdate.Instance.UpdatePhase_1();

        enablingInteract.enabled = true;
        enablingInteract.shouldWork = true;

        foreach (var inspect in enablingInspect)
        {
            inspect.enabled = true;
            inspect.shouldWork = true;
        }

        DialogsPhase_2.enabled = false;
        DialogsPhase_2.gameObject.SetActive(false);

        isObjectEnabled_Phase_1 = true;
    }

    IEnumerator ObjectsEnablePhase_2()
    {
        currentPhase = 3; // Move to Phase 3
        Debug.Log("Phase_2 Started");
        ImageFade.instance.FadeInOut();

       
        yield return new WaitForSeconds(1.5f);
        PosandAnimationUpdate.Instance.UpdatePhase_2();

        motelGatherDialogs.enabled = true;
        motelGatherDialogs.shouldWork = true;

        // Disable earlier phase interactions
        enablingInteract.enabled = false;
        DialogsPhase_2.enabled = false;
        enablingInteractafterPhase_1.enabled = false;
        enablingInteractafterPhase_2.enabled = false;

        foreach (var inspect in enablingInspect)
            inspect.enabled = false;

        isObjectEnabled_Phase_2 = true;
    }

    IEnumerator ObjectsEnablePhase_3()
    {
        currentPhase = 4; // Move to Phase 4
        Debug.Log("Phase_3 Started");
        ImageFade.instance.FadeInOut();

        yield return new WaitForSeconds(1.5f);

        orderBookPickHandler.enabled = true;
        orderBookPickHandler.shouldWork = true;

        foreach (ObjectInteract objInt in LastInteractionObjects)
        {
            objInt.enabled = true;
            objInt.shouldWork = true;
        }
        foreach (var inspect in enablingInspect)
        {
            inspect.enabled = false;
            inspect.shouldWork = false;
        }
        PosandAnimationUpdate.Instance.UpdatePhase_3();
        motelGatherDialogs.enabled = false;

        isFinalDialogComplete = true;
    }
  
    IEnumerator ObjectsEnablePhase_4()
    {
        Debug.Log("Phase_4 Started");
        currentPhase = 5; // End phase
        ImageFade.instance.FadeInOut();

        yield return new WaitForSeconds(1.5f);
        foreach (ObjectInteract objInt in LastInteractionObjects)
        {
            objInt.enabled = false;
            objInt.shouldWork = false;
        }

        ONsentenceCompleteGreg.enabled = true;
        ONsentenceCompleteGreg.shouldWork = true;

    }
    IEnumerator ObjectsEnablePhase_5()
    {
        currentPhase = 6; // End phase
        Debug.Log("Phase_5 Started");
        ImageFade.instance.FadeInOut();

      
        yield return new WaitForSeconds(1.5f);

        ONsentenceCompleteInteract_1.enabled = true;
        ONsentenceCompleteInteract_1.shouldWork = true;

        PosandAnimationUpdate.Instance.UpdatePhase_5();
    }
    
    void ObjectsEnablePhase_6()
    {
        Debug.Log("Phase_6 Started");
        currentPhase = 7;
        ONsentenceCompleteInteract_2.enabled = true;
        ONsentenceCompleteInteract_2.shouldWork = true;
        PosandAnimationUpdate.Instance.UpdatePhase_6();
    }
    IEnumerator ObjectsEnablePhase_7_R_1()
    {
        Debug.Log("Phase_7 Started");
        currentPhase = 8; // End phase
        ImageFade.instance.FadeInOut();

        yield return new WaitForSeconds(1.5f);

        suspectDialogs[0].enabled = true;
        suspectDialogs[0].shouldWork = true;
        PosandAnimationUpdate.Instance.UpdatePhase_7();
    }
   
    void ObjectsEnablePhase_8_R_2()
    {
        Debug.Log("Phase_8 Started");
        currentPhase = 9;
        suspectDialogs[1].enabled = true;
        suspectDialogs[1].shouldWork = true;
    }
    void ObjectsEnablePhase_9_M_1()
    {
        Debug.Log("Phase_9 Started");
        currentPhase = 10;
        suspectDialogs[2].enabled = true;
        suspectDialogs[2].shouldWork = true;
    }
    public void EnableOnSentenceCompleteDialogs()
    {
        ONsentenceCompleteTrigger.enabled = true;
        ONsentenceCompleteTrigger.shouldWork = true;
    }
}
