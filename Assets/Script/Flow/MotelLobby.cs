using System.Collections;
using System.Collections.Generic;
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
    public ObjectInteract puzzleProgressionInteract;
    public ObjectInteract motelGatherDialogs;
    private bool isObjectEnabled_Phase_2 = false;

    [Header("Final Page")]
    public ObjectPickHandler orderBookPickHandler;
    public ObjectInteract[] LastInteractionObjects;
    public Collider[] LastInteractionColliders;

    [Header("ON Sentence Complete")]
    public ObjectInteract ONsentence1CompleteTrigger;
    public ObjectInteract ONsentence2CompleteTrigger;
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

    //// --- Track currently active phase ---
    //private int currentPhase = 0;
    public bool savePhase;
    // Current phase now stored globally
    private int currentPhase
    {
        get => GamePhaseManager.MotelLobbyPhase;
        set => GamePhaseManager.MotelLobbyPhase = value;
    }

    public ObjectInteract[] suspectDialogs;
    public Collider[] suspectColliders;

    public List<PuzzleData> allPuzzles;
    //public bool DontsavePhase_3;
    void Start()
    {
        

        // Camera setup
        targetPos = body.position;
        targetRot = body.rotation;
        body.position = player.position;
        body.rotation = player.rotation;
        isoCam.Follow = body;

        //motelStartDialogs.enabled = true;

        if (doorToNancyRoom) doorToNancyRoom.SetActive(false);
        if (doorToOutsideMotel) doorToOutsideMotel.SetActive(false);

        Debug.LogWarning(currentPhase);

        //if(puzzleProgression != null && puzzleProgression.puzzle6and7Check())
        //{
        //    EnableOnSentenceCompleteDialogs();
        //}
        if (puzzleProgression != null)
        {
            puzzleProgression.OnPuzzle6Completed += EnableOnSentence1CompleteDialogs;
            puzzleProgression.OnPuzzle7Completed += EnableOnSentence2CompleteDialogs;
        }

        //// Start only initial dialog if at phase 0
        //if (currentPhase == 0)
        //{
        //    motelStartDialogs.enabled = true;
        //}
        if (savePhase)
        {
            //  Restore state based on saved phase
            switch (currentPhase)
            {
                case 0:
                    motelStartDialogs.enabled = true;
                    break;

                case 1:
                    DialogsPhase_2.enabled = true;
                    break;

                case 2:
                    // Simulate Phase 1 completion setup
                    StartCoroutine(ObjectsEnablePhase_1());
                    break;

                case 3:
                    // Simulate Phase 2 completion setup
                    StartCoroutine(ObjectsEnablePhase_2());
                    break;

                case 4:
                    StartCoroutine(ObjectsEnablePhase_3());
                    break;

                case 5:
                    StartCoroutine(ObjectsEnablePhase_4());
                    break;

                case 6:
                    StartCoroutine(ObjectsEnablePhase_5());
                    break;

                case 7:
                    ObjectsEnablePhase_6();
                    break;

                case 8:
                    StartCoroutine(ObjectsEnablePhase_7_R_1());
                    break;

                case 9:
                    ObjectsEnablePhase_8_M_1();
                    break;

                case 10:
                    ObjectsEnablePhase_9_R_2();
                    break;
            }
        }
        else
        {
            // ---- RESET EVERYTHING if savePhase is false ----
            currentPhase = 0;
            introPanDone = false;
            isFinalDialogComplete = false;

            // Disable all progression dialogs/interacts
            if (DialogsPhase_2) DialogsPhase_2.enabled = false;
            if (enablingInteract) enablingInteract.enabled = false;
            if (enablingInteractafterPhase_1) enablingInteractafterPhase_1.enabled = false;
            if (enablingInteractafterPhase_2) enablingInteractafterPhase_2.enabled = false;
            if (motelGatherDialogs) motelGatherDialogs.enabled = false;
            if (puzzleProgressionInteract) puzzleProgressionInteract.enabled = false;
            if (orderBookPickHandler) orderBookPickHandler.enabled = false;
            if (ONsentence1CompleteTrigger) ONsentence1CompleteTrigger.enabled = false;
            if (ONsentence2CompleteTrigger) ONsentence2CompleteTrigger.enabled = false;
            if (ONsentenceCompleteGreg) ONsentenceCompleteGreg.enabled = false;
            if (ONsentenceCompleteInteract_1) ONsentenceCompleteInteract_1.enabled = false;
            if (ONsentenceCompleteInteract_2) ONsentenceCompleteInteract_2.enabled = false;

            // Disable all suspect interactions
            foreach (var suspect in suspectDialogs)
                if (suspect) suspect.enabled = false;
            foreach (var col in suspectColliders)
                if (col) col.enabled = false;

            // Disable final interaction objects
            foreach (var obj in LastInteractionObjects)
                if (obj) obj.enabled = false;
            foreach (var col in LastInteractionColliders)
                if (col) col.enabled = false;

            // Disable puzzles
            if (puzzleProgression)
            {
                if (puzzleProgression.puzzle5) puzzleProgression.puzzle5.SetActive(false);
                if (puzzleProgression.puzzle6) puzzleProgression.puzzle6.SetActive(false);
                if (puzzleProgression.puzzle7) puzzleProgression.puzzle7.SetActive(false);
            }

            // Reset doors
            if (doorToNancyRoom) doorToNancyRoom.SetActive(false);
            if (doorToOutsideMotel) doorToOutsideMotel.SetActive(false);

            // Enable only the starting dialog
            if (motelStartDialogs)
            {
                motelStartDialogs.enabled = true;
                motelStartDialogs.gameObject.SetActive(true);
            }

            Debug.Log("MotelLobby reset: Starting fresh (Phase 0).");
            foreach (PuzzleData puzzle in allPuzzles)
            {
                puzzle.isCompleted = false;
            } // Reset dress interaction PlayerPref
        if (PlayerPrefs.HasKey("DressInteractionDone"))
        {
            PlayerPrefs.DeleteKey("DressInteractionDone");
        }

        // Reset outside door PlayerPref
        if (PlayerPrefs.HasKey("EnteredOutsideDoor"))
        {
            PlayerPrefs.DeleteKey("EnteredOutsideDoor");
        }

        // Save changes immediately
        PlayerPrefs.Save();

        Debug.Log("All puzzles, dress interaction, and outside door reset!");
    }

        // Disable intro pan & dialogs for higher phases
        if (currentPhase > 0)
        {
            introPanDone = true;
            isoCam.Follow = player;
            body.position = player.position;
            body.rotation = player.rotation;

            if (motelStartDialogs != null)
            {
                motelStartDialogs.enabled = false;
                motelStartDialogs.gameObject.SetActive(false);
            }
        }

        if (currentPhase >= 4)
        {
            if (doorToNancyRoom) doorToNancyRoom.SetActive(true);
        }
        if (currentPhase >= 9)
        {
            if (doorToOutsideMotel) doorToOutsideMotel.SetActive(true);
        }
        if(currentPhase == 11)
        {
            PosandAnimationUpdate.Instance.UpdatePhase_7();
        }
    }

    private void OnDisable()
    {
        isoCam.Follow = player;

        if(puzzleProgression != null)
        {
            puzzleProgression.OnPuzzle6Completed -= EnableOnSentence1CompleteDialogs;
            puzzleProgression.OnPuzzle7Completed -= EnableOnSentence2CompleteDialogs;
        }
    }

    void Update()
    {
        // === Stop all earlier phases when final is reached ===
        if (isFinalDialogComplete || ONsentence1CompleteTrigger.isAutoComplete || ONsentence2CompleteTrigger.isAutoComplete)
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
        if (currentPhase == 0 && introPanDone && motelStartDialogs && motelStartDialogs.isAutoComplete)
        {
            StartPhase(1);
        }

        if (currentPhase == 1 && DialogsPhase_2 && DialogsPhase_2.isAutoCompleteNearObject && !isObjectEnabled_Phase_1)
        {
            StartCoroutine(ObjectsEnablePhase_1());
        }

        if (currentPhase == 2 && puzzleProgressionInteract.isAutoComplete && !isObjectEnabled_Phase_2)
        {
            StartCoroutine(ObjectsEnablePhase_2());
        }

        if (currentPhase == 3 && motelGatherDialogs.isAutoComplete)
        {
            StartCoroutine(ObjectsEnablePhase_3());
        }

        if (currentPhase == 4 && GamePhaseManager.ONsentence1Complete && GamePhaseManager.ONsentence2Complete) // ONsentence1CompleteTrigger.isAutoComplete && ONsentence2CompleteTrigger.isAutoComplete)
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
            ObjectsEnablePhase_8_M_1();
        }
        if (currentPhase == 9 && suspectDialogs[1].isInteractionComplete)
        {
            ObjectsEnablePhase_9_R_2();
        }
        if (currentPhase == 10 && suspectDialogs[2].isInteractionComplete)
        {
            ObjectsEnablePhase_10();
        }
        if (suspectDialogs[2].isInteractionComplete)
        {
            // Only set the PlayerPref if it hasn't been set before
            if (PlayerPrefs.GetInt("DressInteractionDone", 0) == 0)
            {
                PlayerPrefs.SetInt("DressInteractionDone", 1); // 1 = true
                PlayerPrefs.Save();
                Debug.Log("Dress interaction completed and saved!");
            }
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

        if (puzzleProgression.puzzle5 != null)
            puzzleProgression.puzzle5.SetActive(true);
        Debug.LogWarning("Puzzle5");

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
        enablingInteractafterPhase_1.enabled = false;
        enablingInteractafterPhase_2.enabled = false;

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

        PuzzleUnlock();

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

        // Activate the doors
        if (doorToNancyRoom) doorToNancyRoom.SetActive(true);
        //if (doorToOutsideMotel) doorToOutsideMotel.SetActive(true);
        

        isFinalDialogComplete = true;
    }

    

    IEnumerator ObjectsEnablePhase_4()
    {
        Debug.Log("Phase_4 Started");
        currentPhase = 5; // End phase

        PosandAnimationUpdate.Instance.UpdatePhase_3();

        ImageFade.instance.FadeInOut();

        yield return new WaitForSeconds(1.5f);
        foreach (ObjectInteract objInt in LastInteractionObjects)
        {
            objInt.enabled = false;
            objInt.shouldWork = false;
        }
        foreach (Collider objCol in LastInteractionColliders)
        {
            objCol.enabled = false;
        }
        ONsentenceCompleteGreg.enabled = true;
        ONsentenceCompleteGreg.shouldWork = true;
        
    }
    IEnumerator ObjectsEnablePhase_5()
    {
       
        currentPhase = 6; // End phase
        Debug.Log("Phase_5 Started");
        ImageFade.instance.FadeInOut();

        ONsentenceCompleteGreg.enabled = false;
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
        DisableCompletedPuzzle();
    }
    IEnumerator ObjectsEnablePhase_7_R_1()
    {
        Debug.Log("Phase_7 Started");
        currentPhase = 8; // End phase
        ImageFade.instance.FadeInOut();

        yield return new WaitForSeconds(1.5f);

        suspectDialogs[0].enabled = true;
        suspectDialogs[0].shouldWork = true;
        suspectColliders[0].enabled = true;
        PosandAnimationUpdate.Instance.UpdatePhase_7();
    }
   
    void ObjectsEnablePhase_8_M_1()
    {
        
        Debug.Log("Phase_8 Started");
        currentPhase = 9;
        suspectDialogs[0].enabled = false;
        suspectDialogs[0].shouldWork = false;
        suspectColliders[0].enabled = false;

        suspectDialogs[1].enabled = true;
        suspectDialogs[1].shouldWork = true;
        suspectColliders[1].enabled = true;
    }
    void ObjectsEnablePhase_9_R_2()
    {
        Debug.Log("Phase_9 Started");
        currentPhase = 10;
       
        suspectDialogs[2].enabled = true;
        suspectDialogs[2].shouldWork = true;
        suspectColliders[2].enabled = true;


    }
    void ObjectsEnablePhase_10()
    {
        currentPhase = 11;
        // Activate the doors
        if (doorToNancyRoom) doorToNancyRoom.SetActive(true);
        if (doorToOutsideMotel) doorToOutsideMotel.SetActive(true);
        foreach (ObjectInteract interact in suspectDialogs)
        {
            interact.enabled = false;
            interact.shouldWork = false;
        }
        foreach(Collider col in suspectColliders)
        {
            col.enabled = false;
        }
    }
  
    public void EnableOnSentence1CompleteDialogs()
    {
        ONsentence1CompleteTrigger.enabled = true;
        ONsentence1CompleteTrigger.shouldWork = true;
        GamePhaseManager.ONsentence1Complete = true;
    }
    public void EnableOnSentence2CompleteDialogs()
    {
        ONsentence2CompleteTrigger.enabled = true;
        ONsentence2CompleteTrigger.shouldWork = true;
        GamePhaseManager.ONsentence2Complete = true;
    }
    public void PuzzleUnlock()
    {
        // Hide puzzle 5 if it exists
        if (puzzleProgression.puzzle5 != null)
            puzzleProgression.puzzle5.SetActive(false);

        // Only enable unsolved puzzles
        if (puzzleProgression.puzzleStateData != null)
        {
            if (!puzzleProgression.puzzleStateData.IsComplete(6))
                puzzleProgression.puzzle6?.SetActive(true);

            if (!puzzleProgression.puzzleStateData.IsComplete(7))
                puzzleProgression.puzzle7?.SetActive(true);
        }
        else
        {
            // Fallback (if no data)
            puzzleProgression.puzzle6?.SetActive(true);
            puzzleProgression.puzzle7?.SetActive(true);
        }
    }

    private void DisableCompletedPuzzle()
    {
        if (puzzleProgression.puzzle6 != null && puzzleProgression.puzzle6.activeSelf)
        {
            puzzleProgression.puzzle6.SetActive(false);
        }
        if (puzzleProgression.puzzle7 != null && puzzleProgression.puzzle7.activeSelf)
        {
            puzzleProgression.puzzle7.SetActive(false);
        }
    }
}
