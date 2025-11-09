using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPickHandler : MonoBehaviour
{
    public static ObjectPickHandler Instance;
    public static ObjectPickHandler activePickup;

    [Header("Object Identification")]
    [SerializeField] private string objectID = "1"; // ✅ Unique ID per object (e.g., 1, 2, 3...)

    public enum InspectType { Cigarette, Lighter, Letter_1, TutorialLetter, None }
    public InspectType type = InspectType.None;

    public enum MoveType { Movable, Static }
    public MoveType moveType = MoveType.Static;

    public enum XrayType { Xray, None }
    public XrayType xrayType = XrayType.None;

    private float time;
    private Vector3 offset;
    private Plane dragPlane;

    private Vector3 objectTransform;
    private Quaternion objectRotation;
    private Vector2 turn;

    [SerializeField] private Quaternion PickUpRotation;
    [SerializeField] private ObjectPickReferences pickReferences;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;

    [SerializeField] private GameObject XrayObject;
    [SerializeField] private GameObject XrayLetterMain;

    [SerializeField] private string[] clue;
    public float rotationSensitivity = 4f;
    public bool shouldWork = false;
    [SerializeField] private bool checkClue = false;

    public static bool isMouseLocked;
    [HideInInspector] public bool isPicked;
    public static bool isCollected;
    public static bool isXrayEnabled;

    private bool isVision = false;
    private bool isbusy = false;
    private bool isRotationbusy = false;
    [SerializeField] private bool doRotationInvert = false;

    private int clueCountStoring = 0;
    public int totalClues;

    private ObjectInteract objectInteractCigarette;
    private ObjectInteract objectInteract;
    private Camera mainCam;
    private KeyCode XrayToggle;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pickReferences.XrayCamara.SetActive(false);
        pickReferences.XrayOnImage.SetActive(false);
        pickReferences.XrayOfImage.SetActive(false);

        if (type == InspectType.Cigarette)
        {
            objectInteractCigarette = GetComponent<ObjectInteract>();
            objectInteractCigarette.enabled = false;
        }

        if (checkClue)
        {
            objectInteract = GetComponent<ObjectInteract>();
            objectInteract.enabled = false;

            if (type == InspectType.Lighter)
            {
                enabled = false;
            }
        }

        mainCam = pickReferences.inspectionCamara;
        XrayToggle = pickReferences.XrayToggle;

        // ✅ Ensure ClueProgressManager exists
        if (ClueProgressManager.Instance == null)
        {
            new GameObject("ClueProgressManager").AddComponent<ClueProgressManager>();
        }

        if (!string.IsNullOrEmpty(objectID))
        {
            if (!ClueProgressManager.WasJustReset)
            {
                clueCountStoring = PlayerPrefs.GetInt(objectID + "_ClueCount", 0);
                Debug.Log($"📦 Loaded {objectID} clue count: {clueCountStoring}");
            }
            else
            {
                clueCountStoring = 0;
                Debug.Log($"🧹 Reset mode active — skipping load for {objectID}");
            }
        }

    }

    private void Update()
    {
        if (activePickup != null && activePickup != this)
        {
            Avoid();
            return;
        }

        if (ObjectInteract.isInteracting && outRange != null && inRange != null)
        {
            XrayVisionDisable();
            outRange.alpha = 0;
            inRange.alpha = 0;
            return;
        }

        if (playerInteract.GetObjectPickHandler() != this && !isCollected)
        {
            Avoid();
            return;
        }

        imageDrag();
        ObjectHandler();

        // ✅ Display persistent clue count
        var clueCount = ClueProgressManager.Instance.clueCount;
        var clueCountMain = ClueProgressManager.Instance.clueCountMain;

        if (clueCount < clueCountMain)
        {
            pickReferences.currentClueCount.text =
                "Clues Found (" + clueCount + "/" + clueCountMain + ")";
        }
        else
        {
            pickReferences.currentClueCount.text =
                "Clue's Picked (" + clueCountMain + "/" + clueCountMain + ")";
        }
    }

    private void ObjectHandler()
    {
        if (!shouldWork) return;

        outRange.alpha = 0;

        if (inRange != null)
        {
            if (isCollected || ObjectInteract.isInteracting)
            {
                inRange.alpha = 0;
            }
        }

        if (isCollected && xrayType == XrayType.Xray &&
            (XrayTutorial.Instance == null || XrayTutorial.Instance.shouldShowIcon))
        {
            if (Input.GetKeyDown(XrayToggle))
            {
                if (!isVision) XrayVisionEnable();
                else XrayVisionDisable();
            }

            pickReferences.XrayOnImage.SetActive(isVision);
            pickReferences.XrayOfImage.SetActive(!isVision);
        }
        else if (!isCollected)
        {
            XrayVisionDisable();
        }

        if (isPicked && Input.GetKeyDown(KeyCode.E))
        {
            if (isbusy) return;
            if (InspectionClueFeedBack.Instance != null && InspectionClueFeedBack.Instance.isClueBusy) return;
            if (XrayTutorial.Instance != null && XrayTutorial.Instance.isRunning) return;
            if (InteractClueManager.instance != null && InteractClueManager.isClueShowing) return;
            if (pickReferences.inspectionTutorial != null && pickReferences.inspectionTutorial.isRunning) return;

            StartCoroutine(ObjectDrop());
        }

        if (playerInteract.GetObjectPickHandler() == this)
        {
            if (inRange != null)
            {
                if (!isCollected && !ObjectInteract.isInteracting)
                {
                    inRange.alpha = 1;
                }
                else
                {
                    inRange.alpha = 0;
                }
            }

            if (isbusy) return;
            if (isCollected || ObjectInteract.isInteracting) return;

            if (Input.GetKeyDown(KeyCode.E) && !isPicked)
            {
                StartCoroutine(ObjectPickUp());
            }
        }
        else if (playerInteract.GetObjectPickHandler() == null)
        {
            Avoid();
        }
    }

    public IEnumerator ObjectPickUp()
    {
        if (activePickup != null && activePickup != this) yield break;
        if (ObjectInteract.activeInteraction != null) yield break;

        isRotationbusy = false;
        activePickup = this;

        // ✅ Use persistent counters
        ClueProgressManager.Instance.clueCountMain = totalClues;
        ClueProgressManager.Instance.clueCount = clueCountStoring;

        isCollected = true;
        pickReferences.currentClue.SetActive(true);

        Vector3 target = transform.position;
        target.y = playerInteract.player.transform.position.y;
        playerInteract.player.transform.LookAt(target);

        isbusy = true;
        time = 0;
        isPicked = true;

        if (pickReferences.lights != null)
            pickReferences.lights.SetActive(false);

        if (XrayLetterMain != null) XrayLetterMain.SetActive(false);
        if (XrayObject != null) XrayObject.SetActive(true);

        pickReferences.SwitchCam();
        pickReferences.inspectionBackroundimage.SetActive(true);

        objectTransform = transform.position;
        objectRotation = transform.rotation;
        transform.parent = pickReferences.objectContainer.transform;
        transform.rotation = PickUpRotation;

        if (type == InspectType.Cigarette)
            pickReferences.eToExitimage.SetActive(false);
        else
            pickReferences.eToExitimage.SetActive(true);

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, time * 2);
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(0.2f);
        isbusy = false;

        if (type == InspectType.Letter_1 && isPicked && isCollected && isVision)
        {
            pickReferences.gameOverTrigger.SetActive(true);
        }
    }

    public IEnumerator ObjectDrop()
    {
        isRotationbusy = true;
        activePickup = null;

        XrayVisionDisable();

        if (pickReferences.lights != null)
            pickReferences.lights.SetActive(true);

        pickReferences.eToExitimage.SetActive(false);
        if (XrayLetterMain != null) XrayLetterMain.SetActive(true);
        if (XrayObject != null) XrayObject.SetActive(false);

        // ✅ Save progress for this object
        clueCountStoring = ClueProgressManager.Instance.clueCount;

        if (!string.IsNullOrEmpty(objectID))
        {
            PlayerPrefs.SetInt(objectID + "_ClueCount", clueCountStoring);
            PlayerPrefs.Save();
            Debug.Log($"💾 Saved {objectID} clue count: {clueCountStoring}");
        }

        // ✅ Save global progress
        ClueProgressManager.Instance.SaveAllClueProgress();

        pickReferences.currentClue.SetActive(false);

        transform.rotation = objectRotation;
        transform.parent = null;
        pickReferences.SwitchCam();
        isbusy = true;
        time = 0;

        pickReferences.inspectionBackroundimage.SetActive(false);

        if (type == InspectType.Cigarette)
        {
            objectInteractCigarette.enabled = true;
            objectInteractCigarette.shouldWork = true;
            enabled = false;
        }

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, objectTransform, time);
            yield return null;
        }

        transform.localPosition = objectTransform;

        isPicked = false;
        isCollected = false;

        if (checkClue)
        {
            int count = 0;

            foreach (string clues in clue)
            {
                if (ClueManager.Instance.ClueCheck(clues))
                {
                    count++;
                }
            }

            if (count == clue.Length)
            {
                if (objectInteract != null)
                {
                    enabled = false;
                    objectInteract.enabled = true;
                    objectInteract.StartInteraction();
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        isbusy = false;
    }

    private void XrayVisionEnable()
    {
        isXrayEnabled = true;
        isVision = true;

        pickReferences.XrayCamara.SetActive(true);

        if (type == InspectType.Letter_1 && isCollected)
            pickReferences.gameOverTrigger.SetActive(true);
    }

    private void XrayVisionDisable()
    {
        isVision = false;
        pickReferences.XrayCamara.SetActive(false);
        pickReferences.XrayOfImage.SetActive(false);
        pickReferences.XrayOnImage.SetActive(false);
        isXrayEnabled = false;
    }

    private bool isDragging = false;
    private void imageDrag()
    {
        if (!isPicked || isMouseLocked || isVision || isRotationbusy) return;
        if (InspectionClueFeedBack.Instance != null && InspectionClueFeedBack.Instance.isClueBusy) return;

        if (Input.GetMouseButtonDown(0) && isPicked && !isVision)
        {
            isDragging = true;
            dragPlane = new Plane(-mainCam.transform.forward, transform.position);

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                offset = transform.position - ray.GetPoint(enter);
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            if (moveType == MoveType.Movable)
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    transform.position = hitPoint + offset;
                }
            }
            else
            {
                turn.x = Input.GetAxis("Mouse X") * rotationSensitivity;
                turn.y = Input.GetAxis("Mouse Y") * rotationSensitivity;

                Vector3 right = Vector3.Cross(mainCam.transform.up, transform.position - mainCam.transform.position);
                Vector3 up = Vector3.Cross(transform.position - mainCam.transform.position, right);

                transform.rotation = Quaternion.AngleAxis(-turn.x, up) * transform.rotation;
                if (!doRotationInvert)
                    transform.rotation = Quaternion.AngleAxis(turn.y, right) * transform.rotation;
                else
                    transform.rotation = Quaternion.AngleAxis(-turn.y, right) * transform.rotation;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void Avoid()
    {
        if (shouldWork && !isCollected)
        {
            inRange.alpha = 0;
            outRange.alpha = 1;
            pickReferences.AvoidCam();
            XrayVisionDisable();
            pickReferences.XrayOfImage.SetActive(false);
        }
    }

    public bool InteractionCheck()
    {
        return isCollected;
    }
    public void AddClue()
    {
        clueCountStoring++;

        // ✅ Save to manager (persistent)
        ClueProgressManager.Instance.SaveClueCount(objectID, clueCountStoring);

        // ✅ Update global live counter for UI refresh
        ClueProgressManager.Instance.clueCount = clueCountStoring;

        // ✅ Update UI instantly
        if (pickReferences != null && pickReferences.currentClueCount != null)
        {
            pickReferences.currentClueCount.text =
                "Clues Found (" + clueCountStoring + "/" + totalClues + ")";
        }

        Debug.Log($"🧩 Clue added for {objectID}. Current count: {clueCountStoring}/{totalClues}");
    }


}
